import React, { useState } from "react";
import { View, ScrollView, StyleSheet, Text, TouchableOpacity, Image } from "react-native";
import { useRouter } from "expo-router";
import * as ImagePicker from "expo-image-picker";

import AppHeader from "./components/AppHeader";
import InputField from "./components/InputField";
import Button from "./components/Button";

export default function AddItemScreen() {
  const router = useRouter();

  const [imageUri, setImageUri] = useState<string | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [tags, setTags] = useState<string[]>([]);
  const [location, setLocation] = useState("");

  // State lỗi
  const [nameError, setNameError] = useState("");
  const [locationError, setLocationError] = useState("");
  const [saveSuccess, setSaveSuccess] = useState(false);

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

  const onSave = () => {
    let valid = true;

    if (!name.trim()) {
      setNameError("Tên không được để trống");
      valid = false;
    } else {
      setNameError("");
    }

    if (!location.trim()) {
      setLocationError("Thêm vị trí");
      valid = false;
    } else {
      setLocationError("");
    }

    if (!valid) return;

    setSaveSuccess(true);

    setTimeout(() => {
      setSaveSuccess(false);
      router.back();
    }, 2000);
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
          multiline
        />

        <InputField
          label="Tag"
          placeholder="Nhập tag, cách nhau dấu phẩy"
          value={tags.join(", ")}
          onChangeText={(text) =>
            setTags(text.split(",").map((t) => t.trim()))
          }
        />

        <InputField
          label="Vị trí"
          placeholder="Nhập vị trí"
          value={location}
          onChangeText={setLocation}
          error={locationError}
        />

        <View style={styles.buttonRow}>
          <Button variant="secondary" title="Hủy" onPress={() => router.back()} />
          <Button title="Lưu" onPress={onSave} />
        </View>
      </ScrollView>
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
});
