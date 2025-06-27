import React from "react";
import { Stack } from "expo-router";

export default function Layout() {
  return (
    <Stack
      screenOptions={{
        headerShown: false,
      }}
    >
      <Stack.Screen name="index" />
      <Stack.Screen name="Auth/login" />
      <Stack.Screen name="Auth/register" />
      <Stack.Screen name="Auth/ForgotPassword" />
      <Stack.Screen name="Auth/resetPassword" />
      <Stack.Screen name="dashboard/dashboard" />
      <Stack.Screen name="searchScreen" />
      <Stack.Screen name="dashboard/addItem" />
      <Stack.Screen name="LocationManager" />
      <Stack.Screen name="product/productDetail" />
      <Stack.Screen name="profile/profile" />
      <Stack.Screen name="upgrade/upgradePackage" />
      <Stack.Screen name="Auth/VerifyOtpScreen" />
    </Stack>
  );
}
