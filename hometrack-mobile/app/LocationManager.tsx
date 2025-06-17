import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet, FlatList, Modal, TouchableOpacity, Alert } from "react-native";
import { useRouter, useFocusEffect } from "expo-router";
import AsyncStorage from "@react-native-async-storage/async-storage";

import AppHeader from "./components/AppHeader";
import Button from "./components/Button";
import InputField from "./components/InputField";
import BottomNavigation from "./components/BottomNavigation";
import { locationsCreate, fetchWithAuth, locationsGetAll, locationsDelete, itemsDelete, itemsByLocation } from "./api";

// Định nghĩa type cho location
interface LocationType {
  id: number;
  name: string;
  parentLocationId: number | null;
  description: string;
  items: { id: number; name: string; quantity: number }[];
}

// Định nghĩa type cho item trong LocationManager
interface ItemType {
  id: number;
  name: string;
  description: string; // Thêm trường description
  quantity: number;
  tags: string[]; // Thêm trường tags
  location: { id: number, name: string }; // Cập nhật cấu trúc location
  imageUrl: string | null; // Thêm trường imageUrl
}

// Xóa initialLocations vì dữ liệu sẽ được tải từ API
// const initialLocations: LocationType[] = [
//   { id: "1", name: "Phòng ngủ", parentLocationId: null, description: "Mô tả phòng ngủ", items: [] },
//   { id: "2", name: "Phòng khách", parentLocationId: null, description: "Mô tả phòng khách", items: [] },
// ];

export default function LocationManager() {
  const router = useRouter();

  const [locations, setLocations] = useState<LocationType[]>([]); // Khởi tạo rỗng
  const [selectedLocationId, setSelectedLocationId] = useState<number | null>(null);
  const [itemsForSelectedLocation, setItemsForSelectedLocation] = useState<ItemType[]>([]);
  const [showAddLocationModal, setShowAddLocationModal] = useState(false);
  const [newLocationName, setNewLocationName] = useState("");
  const [newLocationDescription, setNewLocationDescription] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Hàm để tải danh sách các món đồ cho một vị trí cụ thể
  const fetchItemsForLocation = async (locationId: number) => {
    try {
      const response = await fetchWithAuth(itemsByLocation(locationId));
      if (!response.ok) {
        const data = await response.json().catch(() => null);
        throw new Error(data?.message || `Failed to fetch items for location: ${response.status}`);
      }
      const data = await response.json();
      setItemsForSelectedLocation(data || []);
    } catch (err: any) {
      // Có thể hiển thị lỗi cho người dùng nếu cần
    }
  };

  // Hàm để tải danh sách vị trí từ API
  const loadLocations = async () => {
    try {
      setLoading(true);
      const response = await fetchWithAuth(locationsGetAll);
      if (!response.ok) {
        const data = await response.json().catch(() => null);
        throw new Error(data?.message || `Failed to fetch locations: ${response.status}`);
      }
      const data: LocationType[] = await response.json();
      setLocations(data);
      if (data.length > 0) {
        setSelectedLocationId(data[0].id); // Chọn vị trí đầu tiên nếu có
        fetchItemsForLocation(data[0].id); // Tải món đồ cho vị trí đầu tiên
      } else {
        setItemsForSelectedLocation([]); // Không có vị trí nào thì không có món đồ nào
      }
    } catch (err: any) {
      console.error("Error loading locations:", err);
      setError(err.message || "Không thể tải danh sách vị trí");
    } finally {
      setLoading(false);
    }
  };

  // useEffect để gọi loadLocations khi component mount
  useFocusEffect(
    React.useCallback(() => {
      loadLocations();
    }, [])
  );

  // useEffect để tải món đồ khi selectedLocationId thay đổi
  useEffect(() => {
    if (selectedLocationId !== null) {
      fetchItemsForLocation(selectedLocationId);
    } else {
      setItemsForSelectedLocation([]); // Clear items if no location is selected
    }
  }, [selectedLocationId]);

  const handleDeleteLocation = async (locationId: number) => {
    Alert.alert("Xác nhận", "Bạn có chắc muốn xóa vị trí này?", [
      { text: "Huỷ", style: "cancel" },
      { text: "Xóa", style: "destructive", onPress: async () => {
          try {
            const response = await fetchWithAuth(locationsDelete(locationId), {
              method: "DELETE",
            });
            if (!response.ok) {
              const data = await response.json().catch(() => null);
              throw new Error(data?.message || `Failed to delete location: ${response.status}`);
            }
            // Re-fetch locations to update the list
            await loadLocations();
            Alert.alert("Thông báo", "Vị trí đã được xóa thành công.");
          } catch (err: any) {
            console.error("Error deleting location:", err);
            Alert.alert("Lỗi", err.message || "Không thể xóa vị trí.");
          }
        }
      }
    ]);
  };

  const handleAddLocation = async () => {
    if (newLocationName.trim() === "") {
      Alert.alert("Lỗi", "Tên vị trí không được để trống");
      return;
    }

    // Kiểm tra tên vị trí đã tồn tại trong danh sách từ API
    const exists = locations.some(loc => loc.name.toLowerCase() === newLocationName.trim().toLowerCase());
    if (exists) {
      Alert.alert("Lỗi", "Vị trí đã tồn tại");
      return;
    }

    const newLoc = {
      name: newLocationName.trim(),
      parentLocationId: null,
      description: newLocationDescription.trim(),
    };

    try {
      const response = await fetchWithAuth(locationsCreate, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newLoc),
      });

      if (!response.ok) {
        const data = await response.json().catch(() => null);
        Alert.alert("Lỗi", data?.message || "Không thể tạo vị trí mới");
        return;
      }

      // Nếu thành công, tải lại danh sách vị trí từ backend
      // Thay vì thêm vào state local để đảm bảo dữ liệu luôn đồng bộ với server
      setNewLocationName("");
      setNewLocationDescription("");
      setShowAddLocationModal(false);

      // Re-fetch locations to update the list and display the newly created one
      await loadLocations(); // Gọi lại hàm tải vị trí
    } catch (error) {
      console.error("API Connection Error:", error);
      Alert.alert("Lỗi", "Không thể kết nối tới máy chủ. Vui lòng kiểm tra kết nối mạng hoặc thử lại sau.");
    }
  };

  const handleItemPress = (itemId: string) => {
    router.push(`/product-detail?id=${itemId}`);
  };

  const handleDeleteItem = async (itemId: number) => {
    Alert.alert("Xác nhận", "Bạn có chắc muốn xóa món đồ này?", [
      { text: "Huỷ", style: "cancel" },
      { text: "Xóa", style: "destructive", onPress: async () => {
          try {
            const response = await fetchWithAuth(itemsDelete(itemId), {
              method: "DELETE",
            });
            if (response.ok) {
              Alert.alert("Thông báo", "Món đồ đã được xóa thành công.");
              // Sau khi xóa thành công, tải lại danh sách món đồ cho vị trí hiện tại
              if (selectedLocationId !== null) {
                fetchItemsForLocation(selectedLocationId);
              }
            } else {
              const errorData = await response.json();
              Alert.alert("Lỗi", errorData.message || "Không thể xóa món đồ.");
            }
          } catch (err: any) {
            console.error("Error deleting item:", err);
            Alert.alert("Lỗi", err.message || "Không thể xóa món đồ.");
          }
        }
      }
    ]);
  };

  const handleTabPress = (index: number) => {
    switch(index) {
      case 0: router.push("/dashboard"); break;
      case 1: router.push("/Search Screen"); break;
      case 2: /* đang ở đây */ break;
      case 3: router.push("/profile"); break;
    }
  };

  if (loading) {
    return (
      <View style={styles.container}>
        <AppHeader title="Quản lý vị trí" />
        <Text style={styles.loadingText}>Đang tải vị trí...</Text>
        <BottomNavigation
          activeTab={2}
          onTabPress={handleTabPress}
          onAddPress={() => router.push("/add-item")}
        />
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.container}>
        <AppHeader title="Quản lý vị trí" />
        <Text style={styles.errorText}>Lỗi: {error}</Text>
        <BottomNavigation
          activeTab={2}
          onTabPress={handleTabPress}
          onAddPress={() => router.push("/add-item")}
        />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <AppHeader title="Quản lý vị trí" />

      {/* Location Selector */}
      <View style={styles.selectorContainer}>
        <Text style={styles.label}>Danh sách vị trí</Text>
        <View style={styles.locationList}>
          {locations.length > 0 ? (
            locations.map(loc => (
              <TouchableOpacity
                key={loc.id.toString()}
                style={[
                  styles.locationChip,
                  loc.id === selectedLocationId && styles.locationChipSelected
                ]}
                onPress={() => setSelectedLocationId(loc.id)}
              >
                <Text style={[
                  styles.locationChipText,
                  loc.id === selectedLocationId && styles.locationChipTextSelected
                ]}>
                  {loc.name}
                </Text>
                <TouchableOpacity
                  style={styles.deleteLocationChipButton}
                  onPress={() => handleDeleteLocation(loc.id)}
                >
                  <Text style={styles.deleteLocationChipButtonText}>X</Text>
                </TouchableOpacity>
              </TouchableOpacity>
            ))
          ) : (
            <Text style={styles.emptyText}>Chưa có vị trí nào. Hãy thêm một phòng mới!</Text>
          )}
        </View>
      </View>

      {/* Items Section */}
      <Text style={styles.sectionTitle}>Món đồ trong {selectedLocationId !== null ? locations.find(loc => loc.id === selectedLocationId)?.name || "(Chưa chọn vị trí)" : "(Chưa chọn vị trí)"}</Text>

      <View style={styles.itemsContainer}>
        {itemsForSelectedLocation.length > 0 ? (
          itemsForSelectedLocation.map((item) => (
            <View key={item.id.toString()} style={styles.itemRow}>
              <View style={styles.itemInfo}>
                <Text style={styles.itemName}>{item.name}</Text>
                <Text style={styles.itemQuantity}>{item.quantity}</Text>
              </View>
              <View style={styles.buttonContainer}>
                <TouchableOpacity
                  style={styles.detailButton}
                  onPress={() => handleItemPress(item.id.toString())}
                >
                  <Text style={styles.detailButtonText}>Chi tiết</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  style={styles.deleteButton}
                  onPress={() => handleDeleteItem(item.id)}
                >
                  <Text style={styles.deleteButtonText}>Xóa</Text>
                </TouchableOpacity>
              </View>
            </View>
          ))
        ) : (
          <Text style={styles.emptyText}>Không có món đồ nào</Text>
        )}
      </View>

      {/* Add Location Button */}
      <TouchableOpacity
        style={styles.addButton}
        onPress={() => setShowAddLocationModal(true)}
      >
        <Text style={styles.addButtonText}>+ Thêm phòng mới</Text>
      </TouchableOpacity>

      {/* Add Location Modal */}
      <Modal
        visible={showAddLocationModal}
        transparent
        animationType="slide"
        onRequestClose={() => setShowAddLocationModal(false)}
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Thêm phòng mới</Text>
            <InputField
              label="Tên phòng"
              value={newLocationName}
              onChangeText={setNewLocationName}
              placeholder="Nhập tên phòng"
            />
            <InputField
              label="Mô tả"
              value={newLocationDescription}
              onChangeText={setNewLocationDescription}
              placeholder="Nhập mô tả"
            />
            <View style={styles.modalButtons}>
              <Button
                title="Huỷ"
                variant="secondary"
                onPress={() => setShowAddLocationModal(false)}
                style={styles.modalButton}
              />
              <Button
                title="Thêm"
                onPress={handleAddLocation}
                style={styles.modalButton}
              />
            </View>
          </View>
        </View>
      </Modal>

      <BottomNavigation
        activeTab={2}
        onTabPress={handleTabPress}
        onAddPress={() => router.push("/add-item")}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#ffffff",
    paddingHorizontal: 20,
    paddingTop: 20,
    paddingBottom: 80  // đủ khoảng cách cho bottom navigation
  },
  selectorContainer: {
    marginVertical: 20
  },
  label: {
    fontWeight: "600",
    fontSize: 18,
    color: "#000000",
    marginBottom: 10,
  },
  locationList: {
    flexDirection: "row",
    flexWrap: "wrap",
  },
  locationChip: {
    backgroundColor: "#e0e0e0",
    borderRadius: 20,
    paddingVertical: 8,
    paddingHorizontal: 15,
    marginRight: 10,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
  },
  locationChipSelected: {
    backgroundColor: "#007bff",
  },
  locationChipText: {
    color: "#000000",
    fontSize: 16,
    marginRight: 5,
  },
  locationChipTextSelected: {
    color: "#ffffff",
  },
  sectionTitle: {
    fontWeight: "600",
    fontSize: 18,
    color: "#000000",
    marginTop: 20,
    marginBottom: 10,
  },
  itemsContainer: {
    flex: 1,
  },
  itemRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundColor: "#f0f0f0",
    padding: 10,
    borderRadius: 8,
    marginBottom: 10,
  },
  itemInfo: {
    flexDirection: "row", // Ensure name and quantity are in a row
    alignItems: "center",
  },
  itemName: {
    fontSize: 16,
    fontWeight: "bold",
    marginRight: 10,
  },
  itemQuantity: {
    fontSize: 16,
    color: "#555",
  },
  buttonContainer: {
    flexDirection: "row",
  },
  detailButton: {
    backgroundColor: "#28a745",
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 5,
    marginRight: 5,
  },
  detailButtonText: {
    color: "#ffffff",
    fontSize: 14,
    fontWeight: "bold",
  },
  deleteButton: {
    backgroundColor: "#dc3545",
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 5,
  },
  deleteButtonText: {
    color: "#ffffff",
    fontSize: 14,
    fontWeight: "bold",
  },
  addButton: {
    backgroundColor: "#007bff",
    padding: 15,
    borderRadius: 10,
    alignItems: "center",
    marginTop: 20,
  },
  addButtonText: {
    color: "#ffffff",
    fontSize: 18,
    fontWeight: "bold",
  },
  modalOverlay: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "rgba(0, 0, 0, 0.5)",
  },
  modalContent: {
    backgroundColor: "#ffffff",
    padding: 20,
    borderRadius: 10,
    width: "80%",
    alignItems: "center",
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 20,
  },
  modalButtons: {
    flexDirection: "row",
    justifyContent: "space-around",
    width: "100%",
    marginTop: 20,
  },
  modalButton: {
    width: "45%",
  },
  loadingText: {
    textAlign: "center",
    marginTop: 50,
    fontSize: 16,
    color: "#555",
  },
  errorText: {
    textAlign: "center",
    marginTop: 50,
    fontSize: 16,
    color: "red",
  },
  emptyText: {
    textAlign: "center",
    marginTop: 20,
    fontSize: 16,
    color: "#888",
  },
  deleteLocationChipButton: {
    marginLeft: 5,
    backgroundColor: 'red',
    borderRadius: 10,
    width: 20,
    height: 20,
    justifyContent: 'center',
    alignItems: 'center',
  },
  deleteLocationChipButtonText: {
    color: 'white',
    fontSize: 12,
    fontWeight: 'bold',
  },
});
