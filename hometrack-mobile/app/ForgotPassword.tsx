import React, { useState } from "react";
import { View, Text, StyleSheet, TextInput, TouchableOpacity, Alert, ScrollView } from "react-native";
import { useRouter } from "expo-router";

import Button from "./components/Button";
import LinkText from "./components/LinkText";

export default function ForgotPassword() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");
  const [code, setCode] = useState("");
  const [codeError, setCodeError] = useState("");
  const [loading, setLoading] = useState(false);

  const validateEmail = (email: string) => {
    const re = /\S+@\S+\.\S+/;
    return re.test(email);
  };

  const onSendCode = async () => {
    if (!validateEmail(email)) {
      setEmailError("Email không hợp lệ");
      return;
    }
    setEmailError("");
    setLoading(true);
    try {
      // Gọi API gửi mã xác nhận
      // Ví dụ:
      // await api.sendResetCode(email);

      Alert.alert("Thành công", "Mã xác nhận đã được gửi tới email của bạn.");
    } catch (error) {
      Alert.alert("Lỗi", "Gửi mã xác nhận thất bại, vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  const onSubmitCode = async () => {
    if (code.trim() === "") {
      setCodeError("Mã xác nhận không được để trống");
      return;
    }
    setCodeError("");

    try {
      // Gọi API xác nhận mã code
      // Ví dụ:
      // const valid = await api.verifyCode(email, code);
      const valid = true; // Giả sử hợp lệ

      if (valid) {
        Alert.alert("Thành công", "Mã xác nhận hợp lệ, bạn có thể đặt lại mật khẩu.");
        // Điều hướng tới màn hình đặt lại mật khẩu
        // router.push("/reset-password");
      } else {
        setCodeError("Mã xác nhận không đúng");
      }
    } catch {
      Alert.alert("Lỗi", "Không thể xác nhận mã, vui lòng thử lại.");
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <TouchableOpacity style={styles.backButton} onPress={() => router.back()}>
        <Text style={styles.backText}>←</Text>
      </TouchableOpacity>
      <Text style={styles.title}>Quên mật khẩu?</Text>
      <Text style={styles.subtitle}>Đừng lo lắng! Chúng tôi sẽ gửi hướng dẫn đặt lại mật khẩu qua email của bạn</Text>

      <View style={styles.inputGroup}>
        <Text>Email</Text>
        <TextInput
          style={[styles.input, emailError ? styles.inputError : null]}
          placeholder="Nhập địa chỉ email của bạn"
          value={email}
          onChangeText={setEmail}
          keyboardType="email-address"
          autoCapitalize="none"
        />
        {emailError ? <Text style={styles.errorText}>{emailError}</Text> : null}
        <TouchableOpacity onPress={onSendCode}>
          <Text style={styles.sendCodeText}>gửi</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.inputGroup}>
        <Text>Mã xác nhận</Text>
        <TextInput
          style={[styles.input, codeError ? styles.inputError : null]}
          placeholder="Nhập mã xác nhận từ email"
          value={code}
          onChangeText={setCode}
          autoCapitalize="none"
        />
        {codeError ? <Text style={styles.errorText}>{codeError}</Text> : null}
      </View>

      <Button title="Gửi hướng dẫn" onPress={onSubmitCode} disabled={loading} />

      <View style={styles.footer}>
        <Text>Đã nhớ mật khẩu? </Text>
        <LinkText text="Đăng nhập" onPress={() => router.push("/login")} />
      </View>
      <Text style={styles.supportText}>HOẶC</Text>
      <LinkText text="Liên hệ với chúng tôi" onPress={() => Alert.alert("Hỗ trợ", "Mở form hỗ trợ hoặc chuyển hướng")} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 24,
    backgroundColor: "#fff",
    flexGrow: 1,
  },
  backButton: {
    marginBottom: 20,
  },
  backText: {
    fontSize: 24,
  },
  title: {
    fontSize: 26,
    fontWeight: "bold",
    marginBottom: 8,
  },
  subtitle: {
    marginBottom: 20,
    color: "#666",
  },
  inputGroup: {
    marginBottom: 16,
  },
  input: {
    borderWidth: 1,
    borderColor: "#ddd",
    paddingHorizontal: 12,
    paddingVertical: 10,
    borderRadius: 6,
    marginTop: 6,
  },
  inputError: {
    borderColor: "red",
  },
  errorText: {
    color: "red",
    marginTop: 4,
  },
  sendCodeText: {
    color: "#4b44f6",
    marginTop: 6,
    fontWeight: "bold",
  },
  footer: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 24,
  },
  supportText: {
    textAlign: "center",
    marginVertical: 12,
    fontWeight: "bold",
  },
});
