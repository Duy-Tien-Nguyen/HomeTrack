import React, { useState } from "react";
import { 
  View, Text, StyleSheet, FlatList, Modal, TouchableOpacity, Alert 
} from "react-native";
import { useRouter } from "expo-router";

import AppHeader from "./components/AppHeader";
import Button from "./components/Button";
import InputField from "./components/InputField";
import BottomNavigation from "./components/BottomNavigation";

const initialLocations = [
  { id: "1", name: "Phòng ngủ", items: [
    { id: "a1", name: "Quần dài", quantity: 2 },
    { id: "a2", name: "Váy", quantity: 1 },
    { id: "a3", name: "Áo khoác", quantity: 0 },
  ]},
  { id: "2", name: "Phòng khách", items: [] },
];

export default function LocationManager() {
  const router = useRouter();

  const [locations, setLocations] = useState(initialLocations);
  const [selectedLocationId, setSelectedLocationId] = useState(locations[0].id);
  const [showAddLocationModal, setShowAddLocationModal] = useState(false);
  const [newLocationName, setNewLocationName] = useState("");

  const selectedLocation = locations.find(loc => loc.id === selectedLocationId);

  const handleDeleteItem = (itemId: string) => {
    Alert.alert("Xác nhận", "Bạn có chắc muốn xóa món đồ này?", [
      { text: "Huỷ", style: "cancel" },
      { text: "Xóa", style: "destructive", onPress: () => {
          setLocations(locations.map(loc => {
            if (loc.id === selectedLocationId) {
              return {
                ...loc,
                items: loc.items.filter(item => item.id !== itemId)
              };
            }
            return loc;
          }));
        }
      }
    ]);
  };

  const handleAddLocation = () => {
    if (newLocationName.trim() === "") {
      Alert.alert("Lỗi", "Tên phòng không được để trống");
      return;
    }
    const exists = locations.some(loc => loc.name.toLowerCase() === newLocationName.trim().toLowerCase());
    if (exists) {
      Alert.alert("Lỗi", "Phòng đã tồn tại");
      return;
    }
    const newLoc = {
      id: Date.now().toString(),
      name: newLocationName.trim(),
      items: [],
    };
    setLocations([...locations, newLoc]);
    setSelectedLocationId(newLoc.id);
    setNewLocationName("");
    setShowAddLocationModal(false);
  };

  const handleItemPress = (itemId: string) => {
    router.push(`/add-item`);
  };

  const handleTabPress = (index: number) => {
    switch(index) {
      case 0: router.push("/dashboard"); break;
      case 1: router.push("/Search Screen"); break;
      case 2: /* đang ở đây */ break;
      case 3: router.push("/profile"); break;
    }
  };

  return (
    <View style={styles.container}>
      <AppHeader title="Quản lý vị trí" />

      {/* Location Selector */}
      <View style={styles.selectorContainer}>
        <Text style={styles.label}>Danh sách vị trí</Text>
        <View style={styles.locationList}>
          {locations.map(loc => (
            <TouchableOpacity
              key={loc.id}
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
            </TouchableOpacity>
          ))}
        </View>
      </View>

      {/* Items Section */}
      <Text style={styles.sectionTitle}>Món đồ trong {selectedLocation?.name}</Text>

      <View style={styles.itemsContainer}>
        {selectedLocation?.items?.map((item) => (
          <View key={item.id} style={styles.itemRow}>
            <View style={styles.itemInfo}>
              <Text style={styles.itemName}>{item.name}</Text>
              <Text style={styles.itemQuantity}>Số lượng: {item.quantity}</Text>
            </View>
            <View style={styles.buttonContainer}>
              <TouchableOpacity 
                style={styles.detailButton}
                onPress={() => handleItemPress(item.id)}
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
        )) || <Text style={styles.emptyText}>Không có món đồ nào</Text>}
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
    marginBottom: 15 
  },
  locationList: { 
    flexDirection: "row", 
    flexWrap: "wrap",
    gap: 12
  },
  locationChip: {
    paddingHorizontal: 20,
    paddingVertical: 12,
    borderRadius: 25,
    backgroundColor: "#e8e8e8",
  },
  locationChipSelected: {
    backgroundColor: "#6366f1",
  },
  locationChipText: {
    fontSize: 16,
    fontWeight: "500",
    color: "#333333",
  },
  locationChipTextSelected: {
    color: "#ffffff",
  },
  
  sectionTitle: {
    fontWeight: "600",
    fontSize: 18,
    color: "#000000",
    marginBottom: 20,
    marginTop: 10,
  },
  
  itemsContainer: {
    flex: 1,
  },
  
  itemRow: {
    backgroundColor: "#ffffff",
    padding: 20,
    marginBottom: 1,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  itemInfo: {
    flex: 1,
  },
  itemName: { 
    fontSize: 16, 
    fontWeight: "600", 
    color: "#000000",
    marginBottom: 4
  },
  itemQuantity: { 
    fontSize: 14, 
    color: "#666666"
  },
  buttonContainer: {
    flexDirection: "row",
    gap: 8,
  },
  detailButton: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 20,
    backgroundColor: "#e1f5fe",
    borderWidth: 1,
    borderColor: "#03a9f4",
  },
  detailButtonText: {
    fontSize: 14,
    color: "#03a9f4",
    fontWeight: "500",
  },
  deleteButton: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 20,
    backgroundColor: "#ffebee",
    borderWidth: 1,
    borderColor: "#f44336",
  },
  deleteButtonText: {
    fontSize: 14,
    color: "#f44336",
    fontWeight: "500",
  },
  
  emptyText: { 
    textAlign: "center", 
    color: "#666666", 
    fontStyle: "italic", 
    marginTop: 40,
    fontSize: 16
  },
  
  addButton: {
    backgroundColor: "#6366f1",
    borderRadius: 30,
    paddingVertical: 16,
    alignItems: "center",
    marginBottom: 5,
    marginHorizontal: 20,
    position: "absolute",
    bottom: 90,
    left: 0,
    right: 0,
  },
  addButtonText: {
    color: "#ffffff",
    fontSize: 16,
    fontWeight: "600",
  },
  
  modalOverlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.5)",
    justifyContent: "center",
    paddingHorizontal: 20,
  },
  modalContent: {
    backgroundColor: "#ffffff",
    borderRadius: 16,
    padding: 24,
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: "#000000",
    textAlign: "center",
    marginBottom: 20,
  },
  modalButtons: {
    flexDirection: "row",
    justifyContent: "space-between",
    gap: 12,
    marginTop: 20,
  },
  modalButton: {
    flex: 1,
  },
});
