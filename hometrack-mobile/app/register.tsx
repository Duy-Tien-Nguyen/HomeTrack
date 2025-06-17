import React, { useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Alert,
} from "react-native";
import { useRouter } from "expo-router";

import Button from "./components/Button";
import LinkText from "./components/LinkText";
import Logo from "./components/Logo";
import { register, sendOtp, verifyOtp } from "./api";

export default function Register() {
  const router = useRouter();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [fullNameError, setFullNameError] = useState("");
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");
  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");

  // const [otp, setOtp] = useState("");
  // const [otpError, setOtpError] = useState("");
  // const [otpSent, setOtpSent] = useState(false);

  const validateEmail = (email: string) => {
    const re = /\S+@\S+\.\S+/;
    return re.test(email);
  };

  // const onSendOtpPress = async () => {
  //   if (!validateEmail(email)) {
  //     setEmailError("Email không đúng, vui lòng nhập lại");
  //     return;
  //   }
  //   setEmailError("");
  //   try {
  //     const res = await fetch(sendOtp, {
  //       method: "POST",
  //       headers: { "Content-Type": "application/json" },
  //       body: JSON.stringify({ email }),
  //     });
  //     const data = await res.json();
  //     if (!res.ok) {
  //       Alert.alert("Lỗi", data.message || "Gửi OTP thất bại");
  //       return;
  //     }
  //     setOtpSent(true);
  //     Alert.alert("Thông báo", "Đã gửi OTP về email của bạn");
  //   } catch (err) {
  //     Alert.alert("Lỗi", "Không thể kết nối máy chủ");
  //   }
  // };

  const onCreateAccountPress = async () => {
    let valid = true;

    if (firstName.trim() === "" || lastName.trim() === "") {
      setFullNameError("Tên không được để trống");
      valid = false;
    } else {
      setFullNameError("");
    }

    if (!validateEmail(email)) {
      setEmailError("Email không đúng, vui lòng nhập lại");
      valid = false;
    } else {
      setEmailError("");
    }

    if (password.length < 8) {
      setPasswordError("Mật khẩu phải có ít nhất 8 ký tự");
      valid = false;
    } else {
      setPasswordError("");
    }

    if (confirmPassword !== password) {
      setConfirmPasswordError("Mật khẩu không trùng khớp");
      valid = false;
    } else {
      setConfirmPasswordError("");
    }

    if (!valid) return;

    try {
      const registerRes = await fetch(register, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email,
          password,
          firstName,
          lastName,
        }),
      });
      const res = await registerRes.json();
      if (!registerRes.ok) {
        Alert.alert("Lỗi", res.message || "Tài khoản đã tồn tại");
        return;
      }
      Alert.alert("Thông báo", "Đã gửi OTP về email của bạn");
    } catch (err) {
      Alert.alert("Lỗi", "Không thể kết nối máy chủ");
      return;
    }
    resendOtp();

    // Chuyển sang màn hình nhập OTP, truyền đầy đủ params
    router.push({
      pathname: "/VerifyOtpScreen", // Đúng route bạn đã tạo!
      params: {
        email,
        firstName,
        lastName,
        password,
      },
    });
  };

  const resendOtp = async () => {
    try {
      const res = await fetch(sendOtp, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });
      // console.log("Resend OTP to:", email);
      const data = await res.json();
      if (!res.ok) {
        Alert.alert("Lỗi", data.message || "Không gửi được mã OTP");
      } else {
        Alert.alert("Thông báo", "Mã OTP mới đã được gửi về email của bạn");
      }
    } catch (err) {
      Alert.alert("Lỗi", "Không thể gửi OTP, vui lòng thử lại");
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Logo />
      <Text style={styles.title}>Đăng ký</Text>
      <Text style={styles.subtitle}>Sign up to get started</Text>

      <View style={{ marginBottom: 16 }}>
        <Text style={styles.label}>First Name</Text>
        <TextInput
          style={[styles.input, fullNameError && styles.inputError]}
          value={firstName}
          onChangeText={setFirstName}
          placeholder="First Name"
        />
        {fullNameError ? (
          <Text style={styles.errorText}>{fullNameError}</Text>
        ) : null}
      </View>

      <View style={{ marginBottom: 16 }}>
        <Text style={styles.label}>Last Name</Text>
        <TextInput
          style={[styles.input, fullNameError && styles.inputError]}
          value={lastName}
          onChangeText={setLastName}
          placeholder="Last Name"
        />
        {fullNameError ? (
          <Text style={styles.errorText}>{fullNameError}</Text>
        ) : null}
      </View>

      <View style={{ marginBottom: 16 }}>
        <Text style={styles.label}>Email</Text>
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          <TextInput
            style={[
              styles.input,
              emailError && styles.inputError,
              { flex: 1, marginRight: 8 },
            ]}
            value={email}
            onChangeText={setEmail}
            placeholder="Email"
            keyboardType="email-address"
            autoCapitalize="none"
          />
          {/* <TouchableOpacity style={styles.sendOtpBtn} onPress={onSendOtpPress}>
            <Text style={styles.sendOtpBtnText}>Gửi OTP</Text>
          </TouchableOpacity> */}
        </View>
        {emailError ? <Text style={styles.errorText}>{emailError}</Text> : null}

        {/* {otpSent && (
          <>
            <Text style={[styles.label, { marginTop: 16 }]}>Nhập mã OTP</Text>
            <TextInput
              style={[styles.input, otpError && styles.inputError]}
              value={otp}
              onChangeText={setOtp}
              placeholder="Nhập mã OTP"
              keyboardType="number-pad"
            />
            {otpError ? <Text style={styles.errorText}>{otpError}</Text> : null}
          </>
        )} */}
      </View>

      <View style={{ marginBottom: 16 }}>
        <Text style={styles.label}>Password</Text>
        <TextInput
          style={[styles.input, passwordError && styles.inputError]}
          value={password}
          onChangeText={setPassword}
          placeholder="Password"
          secureTextEntry
        />
        {passwordError ? (
          <Text style={styles.errorText}>{passwordError}</Text>
        ) : null}
      </View>

      <View style={{ marginBottom: 16 }}>
        <Text style={styles.label}>Confirm Password</Text>
        <TextInput
          style={[styles.input, confirmPasswordError && styles.inputError]}
          value={confirmPassword}
          onChangeText={setConfirmPassword}
          placeholder="Confirm Password"
          secureTextEntry
        />
        {confirmPasswordError ? (
          <Text style={styles.errorText}>{confirmPasswordError}</Text>
        ) : null}
      </View>

      <Button title="Create Account" onPress={onCreateAccountPress} />

      <View style={styles.registerTextContainer}>
        <Text>Already have an account? </Text>
        <LinkText text="Sign In" onPress={() => router.push("/login")} />
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
    color: "#888",
    marginBottom: 20,
  },
  label: {
    marginBottom: 6,
    fontWeight: "600",
  },
  input: {
    borderWidth: 1,
    borderColor: "#ccc",
    padding: 12,
    borderRadius: 8,
  },
  inputError: {
    borderColor: "red",
  },
  errorText: {
    color: "red",
    marginTop: 4,
  },
  sendOtpBtn: {
    backgroundColor: "#3366FF",
    paddingVertical: 12,
    paddingHorizontal: 16,
    borderRadius: 8,
  },
  sendOtpBtnText: {
    color: "white",
    fontWeight: "bold",
  },
  registerTextContainer: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 20,
  },
});
