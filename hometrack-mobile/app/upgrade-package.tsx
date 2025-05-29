import React from "react";
import { View, Text, StyleSheet, ScrollView, TouchableOpacity } from "react-native";
import { useRouter } from "expo-router";
import Button from "./components/Button";
import AppHeader from "./components/AppHeader";

export default function UpgradePackage() {
  const router = useRouter();

  return (
    <View style={styles.container}>
      <AppHeader
        title="Nâng cấp gói"
        showBackButton
        onBackPress={() => router.back()}
      />

      <ScrollView contentContainerStyle={styles.content}>
        <Text style={styles.title}>Nâng cấp lên Premium</Text>
        <Text style={styles.subtitle}>Mở khóa tất cả tính năng và trải nghiệm đầy đủ HomeTrack</Text>

        <View style={styles.packagesContainer}>
          <View style={styles.packageBox}>
            <Text style={styles.packageTitle}>Basic</Text>
            <Text style={styles.packagePrice}>Miễn phí</Text>
            <View style={styles.featuresList}>
              <Text style={styles.featureItem}>✓ Tối đa 50 đồ vật</Text>
              <Text style={styles.featureItem}>✓ Tối đa 5 phòng</Text>
              <Text style={styles.featureItem}>✓ Tìm kiếm cơ bản</Text>
              <Text style={[styles.featureItem, styles.featureDisabled]}>✗ Bộ lọc nâng cao</Text>
              <Text style={[styles.featureItem, styles.featureDisabled]}>✗ Gợi ý AI</Text>
            </View>
            <View style={styles.buttonPlaceholder}>
              <Text style={{ color: "#999" }}>Gói hiện tại</Text>
            </View>
          </View>

          <View style={[styles.packageBox, styles.packageBoxPremium]}>
            <Text style={[styles.packageTitle, styles.premiumText]}>Premium</Text>
            <Text style={[styles.packagePrice, styles.premiumText]}>99k / năm</Text>
            <View style={styles.featuresList}>
              <Text style={styles.featureItem}>✓ Không giới hạn đồ vật</Text>
              <Text style={styles.featureItem}>✓ Không giới hạn phòng</Text>
              <Text style={styles.featureItem}>✓ Tìm kiếm nâng cao</Text>
              <Text style={styles.featureItem}>✓ Bộ lọc nâng cao</Text>
              <Text style={styles.featureItem}>✓ Gợi ý AI</Text>
            </View>
            <Button
              title="Nâng cấp ngay"
              onPress={() => alert("Nâng cấp thành công!")}
              style={{ marginTop: 12 }}
            />
          </View>
        </View>

        <Text style={styles.benefitsTitle}>Lợi ích thêm với Premium</Text>

        <View style={styles.benefitItem}>
          <Text style={styles.benefitIcon}>🔍</Text>
          <View>
            <Text style={styles.benefitTitle}>Tìm kiếm nâng cao</Text>
            <Text style={styles.benefitDesc}>Lọc theo tag, màu sắc, thời gian sử dụng</Text>
          </View>
        </View>

        <View style={styles.benefitItem}>
          <Text style={styles.benefitIcon}>🤖</Text>
          <View>
            <Text style={styles.benefitTitle}>Gợi ý AI thông minh</Text>
            <Text style={styles.benefitDesc}>Tự động nhận diện và gợi ý tag cho đồ vật</Text>
          </View>
        </View>

        <View style={styles.benefitItem}>
          <Text style={styles.benefitIcon}>💾</Text>
          <View>
            <Text style={styles.benefitTitle}>Không giới hạn lưu trữ</Text>
            <Text style={styles.benefitDesc}>Lưu trữ không giới hạn đồ vật và phòng</Text>
          </View>
        </View>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#fff" },
  content: { padding: 20 },
  title: {
    fontSize: 22,
    fontWeight: "700",
    marginBottom: 6,
    textAlign: "center",
  },
  subtitle: {
    fontSize: 14,
    color: "#666",
    marginBottom: 20,
    textAlign: "center",
  },
  packagesContainer: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginBottom: 24,
  },
  packageBox: {
    flex: 1,
    backgroundColor: "#fff",
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 16,
    padding: 16,
    marginHorizontal: 6,
  },
  packageBoxPremium: {
    borderColor: "#6c63ff",
    backgroundColor: "#f5f4ff",
    shadowColor: "#6c63ff",
    shadowOffset: { width: 0, height: 0 },
    shadowOpacity: 0.5,
    shadowRadius: 10,
    elevation: 5,
  },
  packageTitle: {
    fontWeight: "700",
    fontSize: 18,
    marginBottom: 4,
    textAlign: "center",
  },
  packagePrice: {
    fontWeight: "600",
    fontSize: 14,
    marginBottom: 12,
    textAlign: "center",
  },
  premiumText: {
    color: "#6c63ff",
  },
  featuresList: {
    marginBottom: 12,
  },
  featureItem: {
    fontSize: 14,
    marginVertical: 2,
    color: "#333",
  },
  featureDisabled: {
    color: "#aaa",
    textDecorationLine: "line-through",
  },
  buttonPlaceholder: {
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 8,
    paddingVertical: 8,
    alignItems: "center",
  },
  benefitsTitle: {
    fontWeight: "700",
    fontSize: 18,
    marginBottom: 12,
  },
  benefitItem: {
    flexDirection: "row",
    marginBottom: 12,
  },
  benefitIcon: {
    fontSize: 28,
    marginRight: 16,
  },
  benefitTitle: {
    fontWeight: "700",
    fontSize: 16,
    marginBottom: 4,
  },
  benefitDesc: {
    color: "#555",
  },
});
