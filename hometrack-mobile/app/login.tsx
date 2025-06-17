import React, { useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  Keyboard,
  TouchableWithoutFeedback,
  ScrollView,
  Alert,
} from "react-native";
import { useRouter } from "expo-router";
import AsyncStorage from "@react-native-async-storage/async-storage";

import Button from "./components/Button";
import LinkText from "./components/LinkText";
import Logo from "./components/Logo";
import InputField from "./components/InputField";
import { login, fetchWithAuth } from "./api";

export default function Login() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");
  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [loading, setLoading] = useState(false);

  const validateEmail = (email: string) => {
    const re = /\S+@\S+\.\S+/;
    return re.test(email);
  };

  const onEmailChange = (text: string) => {
    setEmail(text);
    if (!text) {
      setEmailError("");
    } else if (!validateEmail(text)) {
      setEmailError("Email không đúng, vui lòng nhập lại.");
    } else {
      setEmailError("");
    }
  };

  const onPasswordChange = (text: string) => {
    setPassword(text);
    if (!text) {
      setPasswordError("");
    } else if (text.length < 8) {
      setPasswordError("Mật khẩu phải có ít nhất 8 ký tự");
    } else {
      setPasswordError("");
    }
  };

  const loginApi = async (email: string, password: string) => {
    try {
      setLoading(true);
      const response = await fetchWithAuth(login, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      const data = await response.json();
      setLoading(false);

      if (!response.ok || !data.accessToken) {
        Alert.alert("Lỗi", data.message || "Đăng nhập thất bại.");
        return false;
      }

      await AsyncStorage.setItem("accessToken", data.accessToken);
      await AsyncStorage.setItem("refreshToken", data.refreshToken);

      return true;
    } catch (error) {
      setLoading(false);
      Alert.alert("Lỗi", "Đăng nhập không thành công.");
      return false;
    }
  };

  const onLoginPress = async () => {
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

    const success = await loginApi(email, password);
    if (success) {
      router.push("/dashboard");
    }
  };

  return (
    <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
      <ScrollView
        contentContainerStyle={styles.container}
        keyboardShouldPersistTaps="handled"
      >
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
        <Button title="Đăng nhập" onPress={onLoginPress} loading={loading} />
        <View style={styles.registerContainer}>
          <Text>Chưa có tài khoản? </Text>
          <LinkText
            text="Đăng ký tài khoản mới"
            onPress={() => router.push("/register")}
          />
        </View>
      </ScrollView>
    </TouchableWithoutFeedback>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 24,
    backgroundColor: "#fff",
    flexGrow: 1,
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
