import React, { useState } from "react";
import { View, Text, StyleSheet, ScrollView } from "react-native";
import { useRouter } from "expo-router";

import InputField from "./components/InputField";
import Button from "./components/Button";
import LinkText from "./components/LinkText";
import Logo from "./components/Logo";

export default function Register() {
  const router = useRouter();

  const [fullName, setFullName] = useState("");
  const [fullNameError, setFullNameError] = useState("");
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");
  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");
  const [agree, setAgree] = useState(false);

  const validateEmail = (email: string) => {
    const re = /\S+@\S+\.\S+/;
    return re.test(email);
  };
//dây là điểu kiện khi có lỗi sẽ khong hiện sang trang login của trang đăng ký
  const onCreateAccountPress = () => {
  let valid = true;

  if (fullName.trim() === "") {
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

  // Nếu có lỗi thì không chuyển trang
  if (!valid) return;

  // Nếu hợp lệ thì chuyển sang login
  router.push("/login");
};

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Logo />
      <Text style={styles.title}>đăng ký</Text>
      <Text style={styles.subtitle}>Sign up to get started</Text>

      <InputField
        label="Full Name"
        value={fullName}
        onChangeText={setFullName}
        error={fullNameError}
      />
      <InputField
        label="Email"
        value={email}
        onChangeText={setEmail}
        error={emailError}
      />
      <InputField
        label="Password"
        value={password}
        onChangeText={setPassword}
        secureTextEntry
        error={passwordError}
      />
      <InputField
        label="Confirm Password"
        value={confirmPassword}
        onChangeText={setConfirmPassword}
        secureTextEntry
        error={confirmPasswordError}
      />

      {/* Checkbox có thể thêm ở đây nếu cần */}

      <Button title="Create Account" onPress={onCreateAccountPress} />

      <View style={styles.registerTextContainer}>
        <Text>Already have an account? </Text>
        <LinkText
          text="Sign In"
          onPress={() => router.push("/login")}
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
  registerTextContainer: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 20,
  },
});
