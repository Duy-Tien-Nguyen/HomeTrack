import React, { useEffect, useState } from "react";
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, ActivityIndicator, Alert } from "react-native";
import { useRouter } from "expo-router";
import Button from "./components/Button";
import AppHeader from "./components/AppHeader";
import { packagesGetAll, subscriptionsGetMy, subscriptionsRegister, fetchWithAuth } from "./api";

interface Package {
  id: number;
  name: string;
  description: string;
  price: number;
  durationDays: number;
  isActive: boolean;
}

interface Subscription {
  id: number;
  userId: number;
  packageId: number;
  package: Package;
  startDate: string;
  endDate: string;
  status: string;
}

export default function UpgradePackage() {
  const router = useRouter();
  const [packages, setPackages] = useState<Package[]>([]);
  const [currentSubscription, setCurrentSubscription] = useState<Subscription | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        // Fetch packages
        const packagesResponse = await fetchWithAuth(packagesGetAll);
        if (!packagesResponse.ok) {
          throw new Error("Failed to fetch packages");
        }
        const packagesData = await packagesResponse.json();
        setPackages(packagesData);

        // Fetch current subscription
        const subscriptionResponse = await fetchWithAuth(subscriptionsGetMy);
        if (!subscriptionResponse.ok) {
          throw new Error("Failed to fetch current subscription");
        }
        const subscriptionData = await subscriptionResponse.json();
        if (subscriptionData && subscriptionData.length > 0) {
          setCurrentSubscription(subscriptionData[0]); // Assuming one active subscription
        }
      } catch (err: any) {
        setError(err.message);
        Alert.alert("Lỗi", `Không thể tải dữ liệu: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleUpgrade = async (packageId: number) => {
    try {
      setLoading(true);
      const response = await fetchWithAuth(subscriptionsRegister, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ packageId }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to register subscription");
      }

      Alert.alert("Thành công", "Đăng ký gói thành công!");
      // Optionally refetch current subscription to update UI
      const subscriptionResponse = await fetchWithAuth(subscriptionsGetMy);
      if (subscriptionResponse.ok) {
        const subscriptionData = await subscriptionResponse.json();
        if (subscriptionData && subscriptionData.length > 0) {
          setCurrentSubscription(subscriptionData[0]);
        }
      }
    } catch (err: any) {
      Alert.alert("Lỗi", `Không thể đăng ký gói: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={[styles.container, styles.loadingContainer]}>
        <ActivityIndicator size="large" color="#6c63ff" />
        <Text>Đang tải gói dịch vụ...</Text>
      </View>
    );
  }

  if (error) {
    return (
      <View style={[styles.container, styles.loadingContainer]}>
        <Text style={styles.errorText}>Đã xảy ra lỗi: {error}</Text>
        <Button title="Thử lại" onPress={() => { /* Implement retry logic or re-call fetchData */ }} />
      </View>
    );
  }

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
          {packages.map((pkg) => (
            <View
              key={pkg.id}
              style={[
                styles.packageBox,
                currentSubscription?.packageId === pkg.id && styles.packageBoxPremium,
              ]}
            >
              <Text
                style={[
                  styles.packageTitle,
                  currentSubscription?.packageId === pkg.id && styles.premiumText,
                ]}
              >
                {pkg.name}
              </Text>
              <Text
                style={[
                  styles.packagePrice,
                  currentSubscription?.packageId === pkg.id && styles.premiumText,
                ]}
              >
                {pkg.price === 0 ? "Miễn phí" : `${pkg.price}k / ${pkg.durationDays} ngày`}
              </Text>
              <View style={styles.featuresList}>
                <Text style={styles.featureItem}>✓ {pkg.description}</Text>
                {/* You might want to parse description into features or add specific feature props to Package model */}
                <Text style={styles.featureItem}>✓ Tối đa {pkg.name === "Basic" ? "50" : "Không giới hạn"} đồ vật</Text>
                <Text style={styles.featureItem}>✓ Tối đa {pkg.name === "Basic" ? "5" : "Không giới hạn"} phòng</Text>
                <Text style={styles.featureItem}>✓ Tìm kiếm {pkg.name === "Basic" ? "cơ bản" : "nâng cao"}</Text>
                <Text style={[styles.featureItem, pkg.name === "Basic" && styles.featureDisabled]}>
                  {pkg.name === "Basic" ? "✗" : "✓"} Bộ lọc nâng cao
                </Text>
                <Text style={[styles.featureItem, pkg.name === "Basic" && styles.featureDisabled]}>
                  {pkg.name === "Basic" ? "✗" : "✓"} Gợi ý AI
                </Text>
              </View>
              {currentSubscription?.packageId === pkg.id ? (
                <View style={styles.buttonPlaceholder}>
                  <Text style={{ color: "#999" }}>Gói hiện tại</Text>
                </View>
              ) : (
                <Button
                  title="Nâng cấp ngay"
                  onPress={() => handleUpgrade(pkg.id)}
                  style={{ marginTop: 12 }}
                />
              )}
            </View>
          ))}
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
  loadingContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  errorText: {
    color: "red",
    marginBottom: 20,
    textAlign: "center",
  },
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
