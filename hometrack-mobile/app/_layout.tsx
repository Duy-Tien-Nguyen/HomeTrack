import React from "react";
import { Stack } from "expo-router";

export default function Layout() {
  return (
    <Stack
      screenOptions={{
        headerShown: false,
      }}
    >
      <Stack.Screen name="login" />
      <Stack.Screen name="register" />
      <Stack.Screen name="forgotPassword" />
      <Stack.Screen name="resetPassword" />
      <Stack.Screen name="dashboard" />
      <Stack.Screen name="searchScreen" />
      <Stack.Screen name="addItem" />
      <Stack.Screen name="locationManager" />
      <Stack.Screen name="productDetail" />
      <Stack.Screen name="profile" />
      <Stack.Screen name="upgradePackage" />
    </Stack>
  );
}
