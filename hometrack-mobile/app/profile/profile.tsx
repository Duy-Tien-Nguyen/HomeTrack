import React, { useState, useEffect } from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Image,
  Alert,
  Modal,
  ScrollView,
} from "react-native";
import * as ImagePicker from "expo-image-picker";
import { useRouter } from "expo-router";

import AppHeader from "../components/layout/AppHeader";
import Button from "../components/common/Button";
import InputField from "../components/common/InputField";
import { logout, getMyProfile, resetPassword, fetchWithAuth } from "../api/api";

export default function ProfileScreen() {
  const router = useRouter();

  // State avatar
  const [avatarUri, setAvatarUri] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  // Modal và state form
  const [modalVisible, setModalVisible] = useState(false);
  const [currentForm, setCurrentForm] = useState<
    null | "info" | "password" | "upgrade" | "privacy"
  >(null);

  // Thông tin cá nhân mẫu
  const [name, setName] = useState("Nguyễn Văn An");
  const [email, setEmail] = useState("nguyenvanan@email.com");
  const [phone, setPhone] = useState("0123456789");

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await getMyProfile();
        const data = await response.json();
        if (response.ok) {
          setName(`${data.firstName || ""} ${data.lastName || ""}`.trim());
          setEmail(data.email || "");
          setPhone(data.phone || "");
        } else {
          Alert.alert("Lỗi", data.message || "Không thể tải thông tin hồ sơ.");
        }
      } catch (error) {
        console.error("Lỗi khi tải hồ sơ:", error);
        Alert.alert("Lỗi", "Không thể kết nối đến máy chủ.");
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, []);

  // Mật khẩu
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  // Hàm chọn ảnh avatar
  const pickImage = async () => {
    let result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsEditing: true,
      quality: 0.7,
    });
    if (!result.canceled && result.assets?.length) {
      setAvatarUri(result.assets[0].uri);
    }
  };

  // Xử lý mở form tương ứng
  const openForm = (formName: typeof currentForm) => {
    setCurrentForm(formName);
    setModalVisible(true);
  };

  // Đóng modal
  const closeForm = () => {
    setModalVisible(false);
    setCurrentForm(null);
  };

  // Xác nhận đăng xuất
  const handleLogout = () => {
    Alert.alert("Xác nhận", "Bạn có chắc muốn đăng xuất?", [
      { text: "Huỷ", style: "cancel" },
      {
        text: "Đăng xuất",
        style: "destructive",
        onPress: async () => {
          await callLogout();
          router.replace("/Auth/login");
        },
      },
    ]);
  };

  const callLogout = async () => {
    try {
      const res = await fetch(logout, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      });
      console.info("Logout response:", res);
    } catch (err) {}
  };

  // Form nội dung theo loại
  const renderFormContent = () => {
    switch (currentForm) {
      case "info":
        return (
          <>
            <Text style={styles.modalTitle}>Cập nhật thông tin cá nhân</Text>
            <InputField label="Tên" value={name} onChangeText={setName} />
            <InputField label="Email" value={email} onChangeText={setEmail} />
            <Button
              title="Lưu"
              onPress={() => {
                closeForm(); /* Thêm logic lưu info */
              }}
            />
          </>
        );
      case "password":
        return (
          <>
            <Text style={styles.modalTitle}>Đổi mật khẩu</Text>
            <InputField
              label="Mật khẩu cũ"
              value={oldPassword}
              onChangeText={setOldPassword}
              secureTextEntry
            />
            <InputField
              label="Mật khẩu mới"
              value={newPassword}
              onChangeText={setNewPassword}
              secureTextEntry
            />
            <InputField
              label="Xác nhận mật khẩu"
              value={confirmPassword}
              onChangeText={setConfirmPassword}
              secureTextEntry
            />
            <Button
              title="Lưu"
              onPress={() => {
                if (newPassword !== confirmPassword) {
                  Alert.alert("Lỗi", "Mật khẩu mới và xác nhận không khớp");
                  return;
                }
                handleChangePassword();
              }}
            />
          </>
        );
      case "upgrade":
        return (
          <>
            <Text style={styles.modalTitle}>Nâng cấp tài khoản</Text>

            <Button
              title="Nâng cấp"
              onPress={() => {
                closeForm();
                router.push("/upgrade/upgradePackage");
              }}
            />
          </>
        );
      case "privacy":
        return (
          <>
            <Text style={styles.modalTitle}>Quyền riêng tư</Text>
            <Text>Thêm các tùy chọn cài đặt bảo mật, quyền riêng tư ở đây</Text>
            <Button title="Đóng" onPress={closeForm} />
          </>
        );
      default:
        return null;
    }
  };

  const handleChangePassword = async () => {
    try {
      const response = await fetchWithAuth(resetPassword, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          oldPassword,
          newPassword,
        }),
      });
      const data = await response.json();
      if (response.ok) {
        Alert.alert("Thành công", data.message || "Mật khẩu đã được đổi thành công.");
        setOldPassword("");
        setNewPassword("");
        setConfirmPassword("");
        closeForm();
      } else {
        Alert.alert("Lỗi", data.message || "Đổi mật khẩu thất bại.");
      }
    } catch (error) {
      console.error("Lỗi khi đổi mật khẩu:", error);
      Alert.alert("Lỗi", "Không thể kết nối đến máy chủ để đổi mật khẩu.");
    }
  };

  return (
    <View style={styles.container}>
      <AppHeader
        title="Thông tin cá nhân"
        showBackButton
        onBackPress={() => router.back()}
      />

      <ScrollView contentContainerStyle={styles.content}>
        <View style={styles.avatarContainer}>
          <View style={styles.avatarBox}>
            {avatarUri ? (
              <Image source={{ uri: avatarUri }} style={styles.avatar} />
            ) : (
              <View style={styles.defaultAvatar}>
                <Text style={{ fontSize: 48, color: "#999" }}>👤</Text>
              </View>
            )}
            <TouchableOpacity style={styles.cameraButton} onPress={pickImage}>
              <Text style={{ color: "#fff", fontWeight: "700" }}>📷</Text>
            </TouchableOpacity>
          </View>
          <Text style={styles.name}>{name}</Text>
          <Text style={styles.email}>{email}</Text>
        </View>

        {loading ? (
          <Text style={{ textAlign: 'center', marginTop: 20 }}>Đang tải...</Text>
        ) : (
          <>
            <TouchableOpacity
              style={styles.menuItem}
              onPress={() => openForm("info")}
            >
              <Text style={styles.menuTitle}>Cập nhật thông tin</Text>
              <Text style={styles.menuDesc}>
                Chỉnh sửa tên, email, số điện thoại
              </Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.menuItem}
              onPress={() => openForm("password")}
            >
              <Text style={styles.menuTitle}>Thay đổi mật khẩu</Text>
              <Text style={styles.menuDesc}>Cập nhật mật khẩu bảo mật</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.menuItem}
              onPress={() => openForm("upgrade")}
            >
              <Text style={styles.menuTitle}>Nâng cấp tài khoản</Text>
              <Text style={styles.menuDesc}>Mở khóa tính năng Premium</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.menuItem}
              onPress={() => openForm("privacy")}
            >
              <Text style={styles.menuTitle}>Quyền riêng tư</Text>
              <Text style={styles.menuDesc}>Cài đặt bảo mật và riêng tư</Text>
            </TouchableOpacity>

            <Button
              title="Đăng xuất"
              variant="secondary"
              style={{ marginTop: 40 }}
              onPress={handleLogout}
            />
          </>
        )}
      </ScrollView>

      <Modal
        visible={modalVisible}
        animationType="fade"
        transparent={true}
        onRequestClose={closeForm}
      >
        <TouchableOpacity
          style={styles.modalOverlay}
          activeOpacity={1}
          onPressOut={closeForm}
        >
          <View style={styles.modalContent}>
            <ScrollView>{renderFormContent()}</ScrollView>
          </View>
        </TouchableOpacity>
      </Modal>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#fff" },
  content: { padding: 20 },
  avatarContainer: { alignItems: "center", marginBottom: 30 },
  avatarBox: {
    position: "relative",
    width: 120,
    height: 120,
    borderRadius: 20,
    backgroundColor: "#ddd",
    justifyContent: "center",
    alignItems: "center",
    marginBottom: 12,
  },
  avatar: { width: 120, height: 120, borderRadius: 20, resizeMode: "cover" },
  defaultAvatar: {
    width: 120,
    height: 120,
    borderRadius: 20,
    backgroundColor: "#ccc",
    justifyContent: "center",
    alignItems: "center",
  },
  cameraButton: {
    position: "absolute",
    right: 0,
    bottom: 0,
    backgroundColor: "#4b44f6",
    width: 36,
    height: 36,
    borderRadius: 18,
    justifyContent: "center",
    alignItems: "center",
    borderWidth: 2,
    borderColor: "#fff",
  },
  name: {
    fontWeight: "700",
    fontSize: 20,
  },
  email: {
    fontWeight: "400",
    fontSize: 14,
    color: "#666",
  },
  menuItem: {
    padding: 16,
    borderWidth: 1,
    borderColor: "#eee",
    borderRadius: 12,
    marginBottom: 16,
    backgroundColor: "#fafafa",
  },
  menuTitle: {
    fontWeight: "600",
    fontSize: 16,
  },
  menuDesc: {
    fontWeight: "400",
    fontSize: 13,
    color: "#666",
    marginTop: 4,
  },
  modalOverlay: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "rgba(0, 0, 0, 0.5)",
  },
  modalContent: {
    backgroundColor: "#fff",
    borderRadius: 10,
    padding: 20,
    width: "90%",
    maxHeight: "80%",
    elevation: 5,
  },
  modalTitle: {
    fontSize: 18,
    fontWeight: "700",
    marginBottom: 16,
  },
});
