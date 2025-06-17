import React, { useEffect, useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  Alert,
  ScrollView,
} from "react-native";
import { useRouter, useLocalSearchParams } from "expo-router";

import Button from "./components/Button";
import { verifyOtp, register, sendOtp } from "./api";

export default function VerifyOtpScreen() {
  const router = useRouter();
  const params = useLocalSearchParams();
  // Lấy lại các trường truyền từ Register
  const email = params.email;
  const firstName = params.firstName;
  const lastName = params.lastName;
  const password = params.password;

  const [otp, setOtp] = useState("");
  const [otpError, setOtpError] = useState("");
  const [loading, setLoading] = useState(false);
  const [sending, setSending] = useState(false);

  const resendOtp = async () => {
    setSending(true);
    try {
      const res = await fetch(sendOtp, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });
      console.log("Resend OTP to:", email);
      const data = await res.json();
      if (!res.ok) {
        Alert.alert("Lỗi", data.message || "Không gửi được mã OTP");
      } else {
        Alert.alert("Thông báo", "Mã OTP mới đã được gửi về email của bạn");
      }
    } catch (err) {
      Alert.alert("Lỗi", "Không thể gửi OTP, vui lòng thử lại");
    }
    setSending(false);
  };

  const onVerifyPress = async () => {
    if (otp.trim() === "") {
      setOtpError("Vui lòng nhập mã OTP");
      return;
    }
    setOtpError("");
    setLoading(true);

    // 1. Gọi verifyOtp
    try {
      const verifyRes = await fetch(verifyOtp, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ token: otp, email }),
      });
      const verifyData = await verifyRes.json();
      console.info("Verify OTP response:", verifyData);
      if (!verifyRes.ok) {
        setOtpError(verifyData.message || "Xác thực OTP thất bại");
        setLoading(false);
        return;
      }

      // Nếu xác thực OTP thành công, chuyển hướng về trang đăng nhập
      Alert.alert("Thông báo", "Xác thực OTP thành công!");
      setLoading(false);
      router.push("/login");  // Chuyển hướng về trang đăng nhập sau khi xác minh OTP thành công
    } catch (err) {
      setOtpError("Không thể xác thực OTP, vui lòng thử lại");
      setLoading(false);
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.title}>Xác thực mã OTP</Text>
      <Text style={styles.infoText}>
        Chúng tôi đã gửi mã xác thực về email{" "}
        <Text style={{ fontWeight: "bold" }}>{email}</Text>
      </Text>
      <TextInput
        style={[styles.input, otpError && styles.inputError]}
        value={otp}
        onChangeText={setOtp}
        placeholder="Nhập mã OTP"
        keyboardType="number-pad"
        editable={!loading}
      />
      {otpError ? <Text style={styles.errorText}>{otpError}</Text> : null}
      <Button
        style={{ marginTop: 20 }}
        title={loading ? "Đang xử lý..." : "Xác thực & Đăng ký"}
        onPress={onVerifyPress}
        disabled={loading}
      />
      <View style={{ marginTop: 18 }}>
        <Text style={{ textAlign: "center", color: "#666", fontSize: 13 }}>
          Không nhận được mã?{" "}
        </Text>
        <Button
          title={sending ? "Đang gửi..." : "Gửi lại OTP"}
          onPress={resendOtp}
          disabled={sending || loading}
          variant="secondary"
        />
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 24,
    backgroundColor: "#fff",
    flexGrow: 1,
    justifyContent: "center",
  },
  title: {
    fontWeight: "bold",
    fontSize: 24,
    marginBottom: 20,
    textAlign: "center",
  },
  infoText: {
    marginBottom: 16,
    color: "#666",
    textAlign: "center",
  },
  input: {
    borderWidth: 1,
    borderColor: "#ccc",
    padding: 12,
    borderRadius: 8,
    marginTop: 16,
  },
  inputError: {
    borderColor: "red",
  },
  errorText: {
    color: "red",
    marginTop: 4,
    textAlign: "center",
  },
});
