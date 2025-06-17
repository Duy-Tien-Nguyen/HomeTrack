import React, { useState, useEffect } from "react";
import { View, ScrollView, StyleSheet, Text, TouchableOpacity, Image, Alert, Modal, FlatList } from "react-native";
import { useRouter } from "expo-router";
import * as ImagePicker from "expo-image-picker";

import AppHeader from "./components/AppHeader";
import InputField from "./components/InputField";
import Button from "./components/Button";
import { itemsCreate, fetchWithAuth, locationsGetAll } from "./api";

interface LocationType {
  id: string;
  name: string;
  parentLocationId: number | null;
  description: string;
  items: { id: string; name: string; quantity: number }[];
}

export default function AddItemScreen() {
  const router = useRouter();

  const [imageUri, setImageUri] = useState<string | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [tags, setTags] = useState<string[]>([]);
  const [locations, setLocations] = useState<LocationType[]>([]);
  const [selectedLocation, setSelectedLocation] = useState<LocationType | null>(null);
  const [showLocationModal, setShowLocationModal] = useState(false);
  const [color, setColor] = useState(""); // Added color state

  // State lỗi
  const [nameError, setNameError] = useState("");
  const [locationError, setLocationError] = useState("");
  const [colorError, setColorError] = useState(""); // Added error state for color
  const [saveSuccess, setSaveSuccess] = useState(false);

  useEffect(() => {
    const loadLocations = async () => {
      try {
        const response = await fetchWithAuth(locationsGetAll);
        const responseText = await response.text();

        let data = null;
        try {
          data = JSON.parse(responseText);
        } catch (e) {
          Alert.alert("Lỗi", "Không thể parse dữ liệu vị trí. Vui lòng kiểm tra server response.");
          return; // Dừng lại nếu parse lỗi
        }

        if (response.ok) {
          setLocations(data);
        } else {
          Alert.alert("Lỗi", data?.message || responseText || "Không thể tải danh sách vị trí.");
        }
      } catch (error) {
        Alert.alert("Lỗi", "Không thể kết nối để tải danh sách vị trí.");
      }
    };
    loadLocations();
  }, []);

  const pickImage = async () => {
    let result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsEditing: true,
      quality: 0.7,
    });

    if (!result.canceled) {
      const assets = (result as any).assets;
      if (assets?.length) setImageUri(assets[0].uri);
    }
  };

  const handleSubmit = async () => {
    let valid = true;

    // Validation checks
    if (!name.trim()) {
      setNameError("Tên không được để trống");
      valid = false;
    } else {
      setNameError("");
    }

    if (!selectedLocation) {
      setLocationError("Thêm vị trí");
      valid = false;
    } else {
      setLocationError("");
    }

    if (!color.trim()) {
      setColorError("Màu sắc không được để trống");
      valid = false; // Added color validation
    } else {
      setColorError("");
    }

    if (!valid) return;

    // Preparing data for API request
    const formData = new FormData();
    formData.append("name", name);
    formData.append("description", description);
    formData.append("tags", JSON.stringify(tags)); // Reverting to original: Convert tags to JSON string
    formData.append("locationId", selectedLocation?.id || ""); // Gửi ID của vị trí đã chọn
    formData.append("color", color);

    // If image is selected, append it to form data
    if (imageUri) {
      formData.append(
        "imageFile",
        {
          uri: imageUri,
          name: "item-image.jpg",
          type: "image/jpeg",
        } as any
      );
    }

    try {
      // Send POST request to create the item
      const response = await fetchWithAuth(itemsCreate, {
        method: "POST",
        headers: {
        },
        body: formData,
      });

      const responseText = await response.text();

      let data = null;
      try {
        data = JSON.parse(responseText);
      } catch (e) {
        // Nếu không parse được JSON, vẫn tiếp tục với thông báo lỗi chung
      }

      if (response.ok) {
        setSaveSuccess(true);
        setTimeout(() => {
          setSaveSuccess(false);
          router.push('/dashboard');
        }, 2000);
      } else {
        Alert.alert("Lỗi", data?.message || responseText || "Có lỗi xảy ra khi tạo đồ vật.");
      }
    } catch (error) {
      Alert.alert("Lỗi", "Không thể kết nối tới máy chủ.");
    }
  };

  return (
    <View style={styles.container}>
      <AppHeader
        title="Thêm đồ vật"
        showBackButton
        onBackPress={() => router.back()}
      />

      <ScrollView contentContainerStyle={styles.content}>
        <TouchableOpacity
          style={styles.imageBox}
          onPress={pickImage}
          activeOpacity={0.7}
        >
          {imageUri ? (
            <Image source={{ uri: imageUri }} style={styles.image} />
          ) : (
            <Text style={styles.addImageText}>Thêm ảnh</Text>
          )}
        </TouchableOpacity>

        <InputField
          label="Tên đồ vật"
          placeholder="Nhập tên đồ vật"
          value={name}
          onChangeText={setName}
          error={nameError}
        />

        {saveSuccess && (
          <Text style={styles.successText}>Lưu Thành công</Text>
        )}

        <InputField
          label="Mô tả"
          placeholder="Nhập mô tả"
          value={description}
          onChangeText={setDescription}
          multiline={true}
        />

        <InputField
          label="Tag"
          placeholder="Nhập tag, cách nhau dấu phẩy"
          value={tags.join(", ")}
          onChangeText={(text) =>
            setTags(text.split(",").map((t) => t.trim()))
          }
        />

        <TouchableOpacity
          onPress={() => setShowLocationModal(true)}
          activeOpacity={0.7}
          style={styles.inputContainer}
        >
          <InputField
            label="Vị trí"
            placeholder="Chọn vị trí"
            value={selectedLocation?.name || ""}
            onChangeText={() => {}}
            error={locationError}
          />
        </TouchableOpacity>

        <InputField
          label="Màu sắc" // New field for color
          placeholder="Nhập màu sắc"
          value={color}
          onChangeText={setColor}
          error={colorError}
        />

        <View style={styles.buttonRow}>
          <Button variant="secondary" title="Hủy" onPress={() => router.back()} />
          <Button title="Lưu" onPress={handleSubmit} />
        </View>
      </ScrollView>

      <Modal
        visible={showLocationModal}
        transparent={true}
        animationType="slide"
        onRequestClose={() => setShowLocationModal(false)}
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn Vị trí</Text>
            <FlatList
              data={locations}
              keyExtractor={(item) => item.id}
              renderItem={({ item }) => (
                <TouchableOpacity
                  style={styles.locationItem}
                  onPress={() => {
                    setSelectedLocation(item);
                    setShowLocationModal(false);
                    setLocationError("");
                  }}
                >
                  <Text style={styles.locationItemText}>{item.name}</Text>
                </TouchableOpacity>
              )}
            />
            <Button title="Đóng" onPress={() => setShowLocationModal(false)} />
          </View>
        </View>
      </Modal>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#fff" },
  content: {
    padding: 20,
  },
  imageBox: {
    height: 120,
    backgroundColor: "#f0f0f5",
    borderRadius: 8,
    marginBottom: 20,
    justifyContent: "center",
    alignItems: "center",
    borderWidth: 1,
    borderColor: "#ddd",
  },
  addImageText: {
    color: "#999",
    fontSize: 16,
  },
  image: {
    width: "100%",
    height: "100%",
    borderRadius: 8,
  },
  buttonRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginTop: 20,
  },
  successText: {
    color: "#4caf50",
    backgroundColor: "#e8f5e9",
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 6,
    marginBottom: 12,
    fontWeight: "600",
  },
  inputContainer: {
    marginBottom: 20,
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.5)",
    justifyContent: "center",
    alignItems: "center",
  },
  modalContent: {
    backgroundColor: "white",
    padding: 20,
    borderRadius: 10,
    width: "80%",
    maxHeight: "70%",
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 15,
    textAlign: "center",
  },
  locationItem: {
    padding: 15,
    borderBottomWidth: 1,
    borderBottomColor: "#eee",
  },
  locationItemText: {
    fontSize: 18,
  },
});
