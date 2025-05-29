import React from "react";
import { Stack } from "expo-router";

export default function Layout() {
  return (
    <Stack
      screenOptions={{
        headerShown: false, // Ẩn thanh header mặc định cho tất cả màn hình
      }}
    >
      <Stack.Screen name="login" />
      <Stack.Screen name="register" />
      <Stack.Screen name="ForgotPassword" />
      <Stack.Screen name="reset-password" />
      <Stack.Screen name="dashboard" />
      <Stack.Screen name="search-screen" />
      <Stack.Screen name="add-item" />
      <Stack.Screen name="location-manager" />
      <Stack.Screen name="product-detail" />
    </Stack>
  );
}
