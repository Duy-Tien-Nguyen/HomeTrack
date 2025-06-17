import React, { useState, useEffect } from "react";
import { View, ScrollView, StyleSheet, Text, Image, TouchableOpacity, Alert, Modal, FlatList, Keyboard } from "react-native";
import { useLocalSearchParams, useRouter } from "expo-router";
import * as ImagePicker from "expo-image-picker";
import AppHeader from "./components/AppHeader";
import InputField from "./components/InputField";
import Button from "./components/Button";
import { fetchWithAuth, itemsGetById, itemsUpdate, locationsGetById, baseUrl, port, locationsGetAll } from "./api";

interface LocationType {
  id: number;
  name: string;
  parentLocationId: number | null;
  description: string;
  items: { id: number; name: string; quantity: number }[];
}

interface ItemType {
  id: number;
  name: string;
  description: string;
  tags: string[];
  location: string;
  locationId: number;
  imageUrl: string | null;
  quantity: number;
}

// Helper function to recursively parse deeply nested JSON strings
function parseDeeplyNestedJson(value: any): any {
  if (typeof value === 'string') {
    try {
      let parsed = JSON.parse(value);
      // Recursively parse the result if it's still stringified JSON
      return parseDeeplyNestedJson(parsed);
    } catch (e) {
      // If parsing fails, it's not a JSON string, so return the original value
      return value;
    }
  } else if (Array.isArray(value)) {
    // If it's an array, recursively parse each element
    return value.map(parseDeeplyNestedJson);
  } else if (typeof value === 'object' && value !== null) {
    // If it's an object, recursively parse each property
    for (const key in value) {
      if (Object.prototype.hasOwnProperty.call(value, key)) {
        value[key] = parseDeeplyNestedJson(value[key]);
      }
    }
    return value;
  }
  return value; // Return as is if not string, array, or object
}

export default function ProductDetail() {
  const router = useRouter();
  const { id } = useLocalSearchParams();
  const itemId = parseInt(String(id).trim());

  console.log("ProductDetail - Item ID received:", itemId);

  const [item, setItem] = useState<ItemType | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isPremium, setIsPremium] = useState(false);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [tags, setTags] = useState<string[]>([]);
  const [editingTagIdx, setEditingTagIdx] = useState<number | null>(null);
  const [editingTagText, setEditingTagText] = useState("");
  const [location, setLocation] = useState("");
  const [locationId, setLocationId] = useState<number | null>(null);
  const [image, setImage] = useState<string | null>(null);

  const [isEditing, setIsEditing] = useState(false);
  const [showLocationModal, setShowLocationModal] = useState(false);
  const [locations, setLocations] = useState<LocationType[]>([]);

  const [nameError, setNameError] = useState("");
  const [tagError, setTagError] = useState("");
  const [locationError, setLocationError] = useState("");
  const [showSuccess, setShowSuccess] = useState(false);

  const loadItem = async () => {
    if (isNaN(itemId)) {
      setError("Không có ID đồ vật hợp lệ");
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const response = await fetchWithAuth(itemsGetById(itemId.toString()));
      if (!response.ok) {
        const data = await response.json().catch(() => null);
        throw new Error(data?.message || `Failed to fetch item: ${response.status}`);
      }
      const data: ItemType = await response.json();
      console.log("ProductDetail - Item data received:", data);
      setItem(data);
      setName(data.name);
      setDescription(data.description);
      console.log("Raw tags from API:", data.tags);
      let tagsToSet = data.tags;
      try {
        tagsToSet = parseDeeplyNestedJson(tagsToSet);
        // Ensure tagsToSet is a flat array of strings
        if (Array.isArray(tagsToSet)) {
          tagsToSet = tagsToSet.flat().filter(tag => typeof tag === 'string');
        }
      } catch (e) {
        console.error("Error with parseDeeplyNestedJson for tags:", e);
        tagsToSet = [];
      }
      setTags(tagsToSet || []);
      if (data.locationId) {
        console.log("ProductDetail - locationId:", data.locationId);
        const locationResponse = await fetchWithAuth(locationsGetById(data.locationId));
        console.log("ProductDetail - locationResponse:", locationResponse);
        if (locationResponse.ok) {
          const locationData = await locationResponse.json();
          console.log("ProductDetail - locationData:", locationData);
          setLocation(locationData.name);
        }
      } else {
        setLocation("");
      }
      setLocationId(data.locationId);
      if (data.imageUrl) {
        setImage(`${baseUrl}:${port}${data.imageUrl}`);
      } else {
        setImage(null);
      }
    } catch (err: any) {
      console.error("Error loading item:", err);
      setError(err.message || "Không thể tải thông tin đồ vật");
    } finally {
      setLoading(false);
    }
  };

  const loadLocations = async () => {
    try {
      const response = await fetchWithAuth(locationsGetAll);
      if (!response.ok) {
        const data = await response.json().catch(() => null);
        throw new Error(data?.message || `Failed to fetch locations: ${response.status}`);
      }
      const data: LocationType[] = await response.json();
      setLocations(data);
    } catch (err: any) {
      console.error("Error loading locations:", err);
      Alert.alert("Lỗi", err.message || "Không thể tải danh sách vị trí.");
    }
  };

  const loadUserSubscription = async () => {
    try {
      const response = await fetchWithAuth('/api/subscriptions/current');
      if (response.ok) {
        const data = await response.json();
        setIsPremium(data.packageName === 'Premium');
      }
    } catch (err) {
      console.error("Error loading subscription:", err);
    }
  };

  useEffect(() => {
    loadItem();
    loadLocations();
    loadUserSubscription();
  }, [itemId]);

  const pickImage = async () => {
    let result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsEditing: true,
      quality: 0.7,
    });

    const assets = (result as any).assets;
    if (
      !result.canceled &&
      Array.isArray(assets) &&
      assets.length > 0 &&
      assets[0].uri
    ) {
      setImage(assets[0].uri);
    }
  };

  const handleEditTag = (idx: number) => {
    setEditingTagIdx(idx);
    setEditingTagText(tags[idx]);
  };
  const handleSaveTag = (savedIdx: number) => {
    console.log(`handleSaveTag: Đang cố gắng lưu hoặc xóa tag. Index được lưu: ${savedIdx}, current editingTagIdx: ${editingTagIdx}`);
    if (editingTagIdx === savedIdx) { // Only clear if this was the one being edited
      const newTags = [...tags];
      if (editingTagText.trim()) {
        newTags[savedIdx] = editingTagText.trim(); // Use savedIdx for accuracy
      } else {
        newTags.splice(savedIdx, 1);
      }
      setTags(newTags);
      setEditingTagIdx(null); // Clear editing state only if it was this tag
      setEditingTagText("");
      console.log("handleSaveTag: Tags sau khi xử lý:", newTags);
    } else {
      console.log("handleSaveTag: Không xóa trạng thái chỉnh sửa vì một tag khác đang hoạt động.");
    }
  };

  const handleAddTag = () => {
    console.log("handleAddTag: Bắt đầu. editingTagIdx hiện tại:", editingTagIdx);
    let updatedTags = [...tags];

    // If there was a tag being edited, finalize its content first.
    if (editingTagIdx !== null) {
      if (editingTagText.trim()) {
        updatedTags[editingTagIdx] = editingTagText.trim();
        console.log(`handleAddTag: Đã cập nhật tag ${editingTagIdx} thành '${editingTagText.trim()}'`);
      } else {
        // If the text was empty, remove the tag that was being edited
        updatedTags.splice(editingTagIdx, 1);
        console.log(`handleAddTag: Đã xóa tag trống tại ${editingTagIdx}`);
      }
    }

    // Check tag limit based on subscription
    if (!isPremium && updatedTags.length >= 1) {
      Alert.alert(
        "Giới hạn nhãn",
        "Tài khoản cơ bản chỉ được phép sử dụng 1 nhãn. Vui lòng nâng cấp lên Premium để sử dụng nhiều nhãn hơn.",
        [
          { text: "Đóng", style: "cancel" },
          { 
            text: "Nâng cấp", 
            onPress: () => router.push("/upgrade-package")
          }
        ]
      );
      return;
    }

    // Add a new empty tag
    updatedTags.push("");
    console.log("handleAddTag: Đã thêm tag trống mới.");

    // Update all relevant states
    setTags(updatedTags);
    console.log("handleAddTag: Đã gọi setTags. New tags:", updatedTags);
    setEditingTagIdx(updatedTags.length - 1); // Set the new tag as the one being edited
    console.log("handleAddTag: Đã gọi setEditingTagIdx. New editingTagIdx:", updatedTags.length - 1);
    setEditingTagText(""); // Clear the input for the new tag
    console.log("handleAddTag: Đã gọi setEditingTagText.");
  };

  const handleRemoveTag = (idx: number) => {
    setTags(tags.filter((_, i) => i !== idx));
    setEditingTagIdx(null);
    setEditingTagText("");
  };

  const onSave = async () => {
    console.log("onSave: Bắt đầu quá trình lưu.");
    console.log("onSave: Giá trị Name:", name);
    console.log("onSave: Giá trị Description:", description);
    console.log("onSave: Giá trị Tags:", tags);
    console.log("onSave: Giá trị LocationId:", locationId);
    console.log("onSave: Giá trị Image:", image ? "(có ảnh)" : "(không có ảnh)");

    // Filter out any empty tags before validation and saving
    const filteredTags = tags.filter(tag => tag.trim() !== '');
    
    // Check tag limit based on subscription
    if (!isPremium && filteredTags.length > 1) {
      Alert.alert(
        "Giới hạn nhãn",
        "Tài khoản cơ bản chỉ được phép sử dụng 1 nhãn. Vui lòng nâng cấp lên Premium để sử dụng nhiều nhãn hơn.",
        [
          { text: "Đóng", style: "cancel" },
          { 
            text: "Nâng cấp", 
            onPress: () => router.push("/upgrade-package")
          }
        ]
      );
      return;
    }

    setTags(filteredTags); // Update state to reflect filtered tags

    let valid = true;
    setNameError("");
    setTagError("");
    setLocationError("");
    setShowSuccess(false);

    if (!name.trim()) {
      setNameError("Không được để trống tên đồ vật");
      valid = false;
      console.log("onSave: Lỗi xác thực - Tên đồ vật trống.");
    }
    if (!tags || tags.length === 0 || tags.some(tag => tag.trim() === '')) {
      setTagError("Vui lòng nhập ít nhất một tag hợp lệ.");
      valid = false;
      console.log("onSave: Lỗi xác thực - Tag không hợp lệ hoặc trống.");
    }
    if (!locationId) {
      setLocationError("Thiếu vị trí");
      valid = false;
      console.log("onSave: Lỗi xác thực - Thiếu vị trí.");
    }
    if (!valid) {
      console.log("onSave: Xác thực thất bại, không tiến hành lưu.");
      return;
    }

    if (!item) {
        Alert.alert("Lỗi", "Không có dữ liệu đồ vật để lưu.");
        console.log("onSave: Lỗi - Không có dữ liệu đồ vật để lưu.");
        return;
    }

    try {
        console.log("onSave: Bắt đầu gửi dữ liệu lên API.");
        const formData = new FormData();
        formData.append("name", name);
        formData.append("description", description);
        formData.append("tags", JSON.stringify(tags));
        formData.append("locationId", locationId.toString());

        // Handle image upload if changed
        if (image && image.startsWith('file://')) {
            const imageUri = image;
            const filename = imageUri.split('/').pop();
            const match = /\.(\w+)$/.exec(filename || '');
            const type = match ? `image/${match[1]}` : 'image/jpeg';
            
            formData.append('imageFile', {
                uri: imageUri,
                name: filename,
                type
            } as any);
            console.log("onSave: Đã thêm hình ảnh vào FormData.");
        }

        const response = await fetchWithAuth(itemsUpdate(item.id.toString()), {
            method: "PUT",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'multipart/form-data',
            },
            body: formData,
        });

        console.log("onSave: Phản hồi API Status:", response.status);
        const responseText = await response.text();
        console.log("onSave: Phản hồi API Text:", responseText);

        if (!response.ok) {
            let errorMessage = `Failed to update item: ${response.status}`;
            try {
              const data = JSON.parse(responseText);
              errorMessage = data?.message || errorMessage;
            } catch (jsonError) {
              console.error("onSave: Lỗi khi phân tích phản hồi JSON không thành công:", jsonError);
            }
            throw new Error(errorMessage);
        }

        setShowSuccess(true);
        console.log("onSave: Lưu thành công, chuyển hướng sau 1 giây.");
        setTimeout(() => {
            setShowSuccess(false);
            setIsEditing(false);
            router.replace("/Search Screen");
        }, 1000);
    } catch (err: any) {
        console.error("onSave: Lỗi khi lưu đồ vật:", err);
        Alert.alert("Lỗi", err.message || "Không thể lưu thông tin đồ vật");
    }
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <AppHeader title="Chi tiết đồ dùng" showBackButton onBackPress={() => router.back()} />
        <Text style={{ textAlign: "center", marginTop: 20 }}>Đang tải đồ vật...</Text>
      </View>
    );
  }

  if (error || !item) {
    return (
      <View style={styles.centerContainer}>
        <AppHeader title="Chi tiết đồ dùng" showBackButton onBackPress={() => router.back()} />
        <View style={{ padding: 20 }}>
          <Text style={{ textAlign: "center" }}>{error || "Không tìm thấy đồ vật!"}</Text>
        </View>
      </View>
    );
  }

  console.log("ProductDetail - Image state value:", image);

  return (
    <View style={styles.container}>
      <AppHeader title="Chi tiết đồ dùng" showBackButton onBackPress={() => router.back()} />

      <ScrollView style={styles.scrollContainer} contentContainerStyle={styles.scrollContent}>
        <TouchableOpacity
          style={styles.imageBox}
          onPress={isEditing ? pickImage : undefined}
          activeOpacity={isEditing ? 0.7 : 1}
        >
          {image ? (
            <Image source={{ uri: image }} style={styles.image} />
          ) : (
            <Text style={styles.imageIcon}>📷</Text>
          )}
          {isEditing && <Text style={styles.uploadText}>Chọn/tải ảnh lên</Text>}
        </TouchableOpacity>

        <InputField
          label="Tên đồ vật"
          value={name}
          onChangeText={setName}
          editable={isEditing}
          error={nameError}
          placeholder="Nhập tên đồ vật"
        />

        <InputField
          label="Mô tả"
          value={description}
          onChangeText={setDescription}
          editable={isEditing}
          placeholder="Thêm mô tả"
          multiline
        />

        <View style={styles.tagContainer}>
          <Text style={styles.label}>Tag</Text>
          {tags.length > 0 ? (
            <View style={styles.tagRow}>
              {tags.map((tag, idx) => {
                console.log(`Rendering Tag - Index: ${idx}, Tag: '${tag}', Trimmed Length: ${tag.trim().length}, Is Editing Index: ${editingTagIdx === idx}, Current Editing Tag Index: ${editingTagIdx}`);
                return (
                  // Only render the tag chip if it's being edited OR if the tag has content
                  (editingTagIdx === idx || tag.trim().length > 0) && (
                    <View key={idx} style={styles.tagChip}>
                      {editingTagIdx === idx ? (
                        <InputField
                          key={`tag-input-${idx}`}
                          value={editingTagText}
                          onChangeText={(text) => {
                            console.log(`InputField onChangeText: Nhận được text: '${text}'`);
                            setEditingTagText(text);
                          }}
                          onBlur={() => handleSaveTag(idx)}
                          autoFocus
                          style={styles.editingTagInput}
                          placeholder="Nhập tag"
                        />
                      ) : (
                        <Text style={styles.tagText} onPress={() => isEditing && handleEditTag(idx)}>
                          {tag}
                        </Text>
                      )}
                      {isEditing && (
                        <TouchableOpacity onPress={() => handleRemoveTag(idx)} style={styles.removeTagButton}>
                          <Text style={styles.removeTagButtonText}>x</Text>
                        </TouchableOpacity>
                      )}
                    </View>
                  )
                )
              })}
            </View>
          ) : (
            <Text style={styles.emptyText}>Chưa có tag nào</Text>
          )}
          {isEditing && (
            <Button title="Thêm tag" onPress={handleAddTag} size="small" />
          )}
          {tagError ? <Text style={styles.errorText}>{tagError}</Text> : null}
        </View>

        <TouchableOpacity
          onPress={() => isEditing && setShowLocationModal(true)}
          style={styles.inputContainer}
        >
          <InputField
            label="Vị trí"
            placeholder="Chọn vị trí của đồ dùng"
            value={location}
            onChangeText={() => {}}
            editable={isEditing}
            error={locationError}
          />
        </TouchableOpacity>

        {showSuccess && (
          <View style={styles.successBox}>
            <Text style={styles.successText}>Lưu thành công</Text>
          </View>
        )}

        <View style={styles.spacer} />
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
              keyExtractor={(item) => item.id.toString()}
              renderItem={({ item: loc }) => (
                <TouchableOpacity
                  style={styles.locationItem}
                  onPress={() => {
                    setLocation(loc.name);
                    setLocationId(loc.id);
                    setShowLocationModal(false);
                    setLocationError("");
                  }}
                >
                  <Text style={styles.locationItemText}>{loc.name}</Text>
                </TouchableOpacity>
              )}
            />
            <Button title="Đóng" onPress={() => setShowLocationModal(false)} />
          </View>
        </View>
      </Modal>

      <View style={styles.bottomButtonContainer}>
        <View style={styles.buttonRow}>
          <Button
            title={isEditing ? "Huỷ" : "Chỉnh sửa"}
            onPress={() => {
              if (isEditing) {
                if (item) {
                  setName(item.name);
                  setDescription(item.description);
                  setTags(item.tags || []);
                  setLocation(item.location);
                  setLocationId(item.locationId);
                  setImage(item.imageUrl ? `${baseUrl}:${port}${item.imageUrl}` : null);
                }
                setNameError(""); setTagError(""); setLocationError("");
                setEditingTagIdx(null); setEditingTagText("");
              }
              setIsEditing(!isEditing);
              setShowSuccess(false);
            }}
            variant="secondary"
            style={styles.button}
          />
          {isEditing && (
            <Button title="Lưu" onPress={onSave} style={styles.button} />
          )}
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#ffffff",
  },
  centerContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#ffffff",
  },
  scrollContainer: {
    flex: 1,
    paddingHorizontal: 20,
    paddingTop: 20,
  },
  scrollContent: {
    paddingBottom: 80,
  },
  imageBox: {
    width: "100%",
    height: 200,
    backgroundColor: "#e0e0e0",
    borderRadius: 10,
    justifyContent: "center",
    alignItems: "center",
    marginBottom: 20,
    overflow: "hidden",
  },
  image: {
    width: "100%",
    height: "100%",
    resizeMode: "cover",
  },
  imageIcon: {
    fontSize: 80,
    color: "#888",
  },
  uploadText: {
    position: "absolute",
    bottom: 10,
    color: "#555",
    fontSize: 14,
  },
  tagContainer: {
    marginBottom: 20,
  },
  label: {
    fontSize: 16,
    fontWeight: "bold",
    marginBottom: 8,
    color: "#333",
  },
  tagRow: {
    flexDirection: "row",
    flexWrap: "wrap",
    marginBottom: 10,
  },
  tagChip: {
    flexDirection: "row",
    backgroundColor: "#e0e0e0",
    borderRadius: 20,
    paddingVertical: 8,
    paddingHorizontal: 15,
    marginRight: 10,
    marginBottom: 10,
    alignItems: "center",
  },
  tagText: {
    color: "#333",
    fontSize: 14,
  },
  editingTagInput: {
    paddingVertical: 0,
    paddingHorizontal: 0,
    margin: 0,
    minWidth: 50,
    textAlign: "center",
    borderWidth: 0,
    backgroundColor: 'transparent',
  },
  removeTagButton: {
    marginLeft: 8,
    backgroundColor: "#ff4d4d",
    borderRadius: 10,
    width: 20,
    height: 20,
    justifyContent: "center",
    alignItems: "center",
  },
  removeTagButtonText: {
    color: "#ffffff",
    fontSize: 12,
    fontWeight: "bold",
  },
  inputContainer: {
    marginBottom: 20,
  },
  successBox: {
    backgroundColor: "#d4edda",
    padding: 10,
    borderRadius: 5,
    marginTop: 20,
    alignItems: "center",
  },
  successText: {
    color: "#155724",
    fontWeight: "bold",
  },
  spacer: {
    height: 50,
  },
  bottomButtonContainer: {
    position: "absolute",
    bottom: 0,
    left: 0,
    right: 0,
    backgroundColor: "#ffffff",
    paddingHorizontal: 20,
    paddingVertical: 15,
    borderTopWidth: 1,
    borderTopColor: "#e0e0e0",
    shadowColor: "#000",
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 5,
  },
  buttonRow: {
    flexDirection: "row",
    justifyContent: "space-around",
  },
  button: {
    flex: 1,
    marginHorizontal: 5,
  },
  emptyText: {
    color: "#888",
    fontStyle: "italic",
    textAlign: "center",
    marginTop: 10,
  },
  modalOverlay: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "rgba(0, 0, 0, 0.5)",
  },
  modalContent: {
    backgroundColor: "#ffffff",
    borderRadius: 10,
    padding: 20,
    width: "80%",
    maxHeight: "80%",
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 15,
    textAlign: "center",
  },
  locationItem: {
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: "#eee",
  },
  locationItemText: {
    fontSize: 16,
    color: "#333",
  },
  errorText: {
    color: "red",
    fontSize: 12,
    marginTop: 5,
  },
});
