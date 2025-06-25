import React, { useState } from "react";
import { View, Text, StyleSheet, Alert, ScrollView } from "react-native";
import { useRouter } from "expo-router";
import { useLocalSearchParams } from "expo-router";

import InputField from "./components/InputField";
import Button from "./components/Button";
import Logo from "./components/Logo";
import LinkText from "./components/LinkText";
import { forgotPassword } from "./api";

export default function ResetPassword() {
  const router = useRouter();

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmError, setConfirmError] = useState("");
  const params = useLocalSearchParams();
  const email = params.email;
  const otp = params.code;

  const onResetPress = async () => {
    let valid = true;

    if (newPassword.length < 8) {
      setPasswordError("Mật khẩu phải có ít nhất 8 ký tự");
      valid = false;
    } else {
      setPasswordError("");
    }

    if (confirmPassword !== newPassword) {
      setConfirmError("Mật khẩu xác nhận không khớp");
      valid = false;
    } else {
      setConfirmError("");
    }

    if (!valid) return;

    try {
      const res = await fetch(forgotPassword, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: email,
          token: otp,
          newPassword: newPassword,
          repeatPassword: confirmPassword,
        }),
      });

      const data = await res.json();
      if (!res.ok) {
        Alert.alert("Lỗi", data.message || "Đặt lại mật khẩu thất bại");
        return;
      }
      Alert.alert("Thành công", "Mật khẩu của bạn đã được đặt lại", [
        { text: "OK", onPress: () => router.push("/login") },
      ]);
    } catch (e) {
      Alert.alert("Lỗi", "Không thể đặt lại mật khẩu, vui lòng thử lại.");
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Logo />
      <Text style={styles.title}>Đặt lại mật khẩu</Text>
      <Text style={styles.subtitle}>
        Hãy nhập mật khẩu mới để bảo vệ tài khoản của bạn
      </Text>

      <InputField
        label="Mật khẩu mới"
        value={newPassword}
        onChangeText={setNewPassword}
        secureTextEntry
        error={passwordError}
      />
      <InputField
        label="Xác nhận mật khẩu"
        value={confirmPassword}
        onChangeText={setConfirmPassword}
        secureTextEntry
        error={confirmError}
      />

      <Button title="Đặt lại mật khẩu" onPress={onResetPress} />

      <View style={styles.footer}>
        <Text>Đã nhớ mật khẩu? </Text>
        <LinkText text="Đăng nhập" onPress={() => router.push("/login")} />
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 24,
    backgroundColor: "#fff",
    flexGrow: 1,
  },
  title: {
    fontWeight: "bold",
    fontSize: 26,
    marginBottom: 4,
  },
  subtitle: {
    color: "#666",
    marginBottom: 20,
  },
  footer: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 24,
  },
});
