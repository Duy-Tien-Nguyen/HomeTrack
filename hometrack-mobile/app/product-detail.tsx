import React, { useState } from "react";
import { View, ScrollView, StyleSheet, Text, Image, TouchableOpacity } from "react-native";
import { useLocalSearchParams, useRouter } from "expo-router";
import * as ImagePicker from "expo-image-picker";
import AppHeader from "./components/AppHeader";
import InputField from "./components/InputField";
import Button from "./components/Button";

const mockItems = [
  {
    id: "1",
    name: "Máy tính xách tay",
    description: "Máy để học và làm việc.",
    tags: ["Điện tử"],
    location: "Phòng Ngủ",
    image: null,
  }
];

export default function ProductDetail() {
  const router = useRouter();
  const { id } = useLocalSearchParams();
  const item = mockItems.find(i => i.id === String(id));

  const [name, setName] = useState(item?.name ?? "");
  const [description, setDescription] = useState(item?.description ?? "");
  const [tags, setTags] = useState(item?.tags ?? []);
  const [editingTagIdx, setEditingTagIdx] = useState<number | null>(null);
  const [editingTagText, setEditingTagText] = useState("");
  const [location, setLocation] = useState(item?.location ?? "");
  const [image, setImage] = useState<string | null>(item?.image ?? null);

  const [isEditing, setIsEditing] = useState(false);

  const [nameError, setNameError] = useState("");
  const [tagError, setTagError] = useState("");
  const [locationError, setLocationError] = useState("");
  const [showSuccess, setShowSuccess] = useState(false);

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
  const handleSaveTag = () => {
    if (editingTagIdx !== null && editingTagText.trim()) {
      const newTags = [...tags];
      newTags[editingTagIdx] = editingTagText.trim();
      setTags(newTags);
      setEditingTagIdx(null);
      setEditingTagText("");
    }
  };

  const handleAddTag = () => {
    setTags([...tags, "Tag mới"]);
    setEditingTagIdx(tags.length);
    setEditingTagText("Tag mới");
  };

  const handleRemoveTag = (idx: number) => {
    setTags(tags.filter((_, i) => i !== idx));
    setEditingTagIdx(null);
    setEditingTagText("");
  };

  const onSave = () => {
    let valid = true;
    setNameError("");
    setTagError("");
    setLocationError("");
    setShowSuccess(false);

    if (!name.trim()) {
      setNameError("Không được để trống tên đồ vật");
      valid = false;
    }
    if (!tags || tags.length === 0) {
      setTagError("Thiếu tag");
      valid = false;
    }
    if (!location.trim()) {
      setLocationError("Thiếu vị trí");
      valid = false;
    }
    if (!valid) return;

    setShowSuccess(true);
    setTimeout(() => {
      setShowSuccess(false);
      setIsEditing(false);
      router.replace("/Search Screen"); 
    }, 1000);
  };

  if (!item) {
    return (
      <View style={styles.centerContainer}>
        <AppHeader title="Chi tiết đồ dùng" showBackButton onBackPress={() => router.back()} />
        <View style={{ padding: 20 }}>
          <Text style={{ textAlign: "center" }}>Không tìm thấy đồ vật!</Text>
        </View>
      </View>
    );
  }

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
              {tags.map((tag, idx) =>
                isEditing && editingTagIdx === idx ? (
                  <View key={idx} style={styles.tagEditBox}>
                    <InputField
                      value={editingTagText}
                      onChangeText={setEditingTagText}
                      style={{ minWidth: 60, fontSize: 12, paddingVertical: 2, paddingHorizontal: 6 }}
                    />
                    <TouchableOpacity onPress={handleSaveTag} style={styles.tagActionBtn}>
                      <Text style={{ color: "#1976d2", fontWeight: "bold" }}>✔</Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={() => handleRemoveTag(idx)} style={styles.tagActionBtn}>
                      <Text style={{ color: "#dc3545", fontWeight: "bold" }}>✘</Text>
                    </TouchableOpacity>
                  </View>
                ) : (
                  <TouchableOpacity
                    key={idx}
                    style={styles.tagBadge}
                    disabled={!isEditing}
                    onLongPress={() => isEditing && handleEditTag(idx)}
                  >
                    <Text style={styles.tagText}>{tag}</Text>
                    {isEditing && (
                      <TouchableOpacity onPress={() => handleEditTag(idx)} style={styles.tagActionBtn}>
                        <Text style={{ fontSize: 10, color: "#888" }}>✎</Text>
                      </TouchableOpacity>
                    )}
                  </TouchableOpacity>
                )
              )}
            </View>
          ) : (
            <Text style={styles.noTagText}>Chưa có tag</Text>
          )}
          {isEditing && (
            <Button title="Thêm tag" onPress={handleAddTag} size="small" />
          )}
          {tagError ? <Text style={styles.errorText}>{tagError}</Text> : null}
        </View>

        <InputField
          label="Vị trí"
          value={location}
          onChangeText={setLocation}
          editable={isEditing}
          error={locationError}
          placeholder="Nhập vị trí của đồ dùng"
        />

        {showSuccess && (
          <View style={styles.successBox}>
            <Text style={styles.successText}>Lưu thành công</Text>
          </View>
        )}

        <View style={styles.spacer} />
      </ScrollView>

      <View style={styles.bottomButtonContainer}>
        <View style={styles.buttonRow}>
          <Button
            title={isEditing ? "Huỷ" : "Chỉnh sửa"}
            onPress={() => {
              if (isEditing) {
                setName(item?.name ?? "");
                setDescription(item?.description ?? "");
                setTags(item?.tags ?? []);
                setLocation(item?.location ?? "");
                setImage(item?.image ?? null);
                setNameError(""); setTagError(""); setLocationError("");
                setEditingTagIdx(null); setEditingTagText("");
              }
              setIsEditing(!isEditing);
              setShowSuccess(false);
            }}
            variant="secondary"
            style={styles.button}
          />
          <Button
            title="Lưu"
            onPress={onSave}
            variant="primary"
            style={styles.button}
            disabled={!isEditing}
          />
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#fff" },
  centerContainer: { flex: 1, backgroundColor: "#fff", justifyContent: "center", alignItems: "center" },
  scrollContainer: { flex: 1 },
  scrollContent: { padding: 20, paddingBottom: 100 },
  imageBox: {
    width: "100%", height: 120, borderRadius: 10,
    backgroundColor: "#f8f9fa", marginBottom: 18,
    justifyContent: "center", alignItems: "center",
    borderWidth: 1, borderColor: "#e9ecef", borderStyle: "dashed",
    position: "relative"
  },
  image: { width: "100%", height: "100%", borderRadius: 10, resizeMode: "cover" },
  imageIcon: { fontSize: 32, color: "#adb5bd" },
  uploadText: { fontSize: 12, color: "#4b44f6", position: "absolute", bottom: 8 },
  label: { fontSize: 14, fontWeight: "600", color: "#333", marginBottom: 6 },
  tagContainer: { marginBottom: 14 },
  tagRow: { flexDirection: "row", flexWrap: "wrap", gap: 6, marginBottom: 8, alignItems: "center" },
  tagBadge: {
    backgroundColor: "#e3f2fd", borderRadius: 12, paddingHorizontal: 10, paddingVertical: 4,
    marginRight: 6, flexDirection: "row", alignItems: "center"
  },
  tagEditBox: {
    flexDirection: "row", alignItems: "center", backgroundColor: "#fffbe7",
    borderRadius: 8, borderWidth: 1, borderColor: "#ffe082", paddingHorizontal: 6, paddingVertical: 2, marginRight: 6
  },
  tagActionBtn: { marginLeft: 4, padding: 2 },
  tagText: { color: "#1976d2", fontWeight: "500", fontSize: 12 },
  noTagText: { color: "#6c757d", fontSize: 14, fontStyle: "italic", marginBottom: 8 },
  errorText: { color: "#dc3545", fontSize: 12, marginTop: 4 },
  successBox: { backgroundColor: "#d4edda", borderColor: "#c3e6cb", borderWidth: 1, borderRadius: 8, padding: 12, alignItems: "center", marginBottom: 16 },
  successText: { color: "#155724", fontWeight: "600", fontSize: 14 },
  spacer: { height: 24 },
  bottomButtonContainer: {
    backgroundColor: "#fff", paddingHorizontal: 20, paddingTop: 16, paddingBottom: 20,
    borderTopWidth: 1, borderTopColor: "#f0f0f0",
    position: 'absolute', bottom: 0, left: 0, right: 0,
  },
  buttonRow: { flexDirection: "row", gap: 12 },
  button: { flex: 1 },
});
