import React, { useState } from "react";
import { View, Text, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import Button from "./components/Button";
import LinkText from "./components/LinkText";
import Logo from "./components/Logo";
import InputField from "./components/InputField";

export default function Login() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");
  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const validateEmail = (email: string) => {
    const re = /\S+@\S+\.\S+/;
    return re.test(email);
  };

  const onEmailChange = (text: string) => {
    setEmail(text);
    if (text.length === 0) {
      setEmailError("");
    } else if (!validateEmail(text)) {
      setEmailError("Email không đúng, vui lòng nhập lại.");
    } else {
      setEmailError("");
    }
  };

  const onPasswordChange = (text: string) => {
    setPassword(text);
    if (text.length === 0) {
      setPasswordError("");
    } else if (text.length < 8) {
      setPasswordError("Mật khẩu phải có ít nhất 8 ký tự");
    } else {
      setPasswordError("");
    }
  };

  const onLoginPress = () => {
    let valid = true;

    if (!validateEmail(email)) {
      setEmailError("Email không đúng, vui lòng nhập lại.");
      valid = false;
    }

    if (password.length < 8) {
      setPasswordError("Mật khẩu phải có ít nhất 8 ký tự");
      valid = false;
    }

    if (!valid) return;

    // Sửa đường dẫn điều hướng ở đây:
    router.push("/dashboard");
  };

  return (
    <View style={styles.container}>
      <Logo />
      <InputField
        label="Email"
        placeholder="Nhập email của bạn"
        value={email}
        onChangeText={onEmailChange}
        error={emailError}
      />
      <InputField
        label="Mật khẩu"
        placeholder="Nhập mật khẩu"
        value={password}
        onChangeText={onPasswordChange}
        secureTextEntry
        error={passwordError}
      />
      <LinkText
        text="Quên mật khẩu?"
        onPress={() => router.push("/ForgotPassword")}
        style={styles.forgot}
      />
      <Button title="Đăng nhập" onPress={onLoginPress} />
      
      <View style={styles.registerContainer}>
        <Text>Chưa có tài khoản? </Text>
        <LinkText
          text="Đăng ký tài khoản mới"
          onPress={() => router.push("/register")}
        />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 24,
    backgroundColor: "#fff",
    justifyContent: "center",
  },
  forgot: {
    alignSelf: "flex-end",
    marginVertical: 8,
  },
  registerContainer: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 20,
  },
});
