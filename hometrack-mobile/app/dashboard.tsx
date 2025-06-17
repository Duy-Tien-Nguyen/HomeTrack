import React, { useState, useEffect, useCallback } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  Alert,
} from "react-native";
import { useRouter } from "expo-router";

import AppHeader from "./components/AppHeader";
import StatsCard from "./components/StatsCard";
import ItemCard from "./components/ItemCard";
import BottomNavigation from "./components/BottomNavigation";
import { getUsageStatistics, itemsDelete, fetchWithAuth, itemsGetAll } from "./api";
import { MaterialIcons } from "@expo/vector-icons";

interface ItemType {
  id: string;
  name: string;
  description: string;
  tags: string[];
  location: string | null;
  locationId: number;
  imageUrl: string | null;
  quantity: number;
}

export default function Dashboard() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState(0);
  const [statistics, setStatistics] = useState({
    totalItems: 0,
    totalLocations: 0,
    itemsNeedingUpdate: 0,
    recentItemsAdded: 0,
  });
  const [recentItems, setRecentItems] = useState<ItemType[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchRecentItems = async () => {
    try {
      const response = await fetchWithAuth(itemsGetAll, {
        method: "GET",
      });

      const responseText = await response.text();

      if (response.status === 204 || response.headers.get('content-length') === '0') {
        setRecentItems([]);
        return 0; // Return 0 if no content
      }

      let data = null;
      try {
        data = JSON.parse(responseText);
      } catch (e) {
        setError("Failed to parse items data.");
        setRecentItems([]);
        return 0; // Return 0 on error
      }

      if (response.ok) {
        const fetchedItems = data.data || data;

        if (!Array.isArray(fetchedItems)) {
          setError("Invalid data format for items.");
          setRecentItems([]);
          return 0; // Return 0 on invalid data
        }
        
        setRecentItems(fetchedItems.map((item: any) => ({
          id: item.id.toString(),
          icon: "inventory" as keyof typeof MaterialIcons.glyphMap,
          name: item.name,
          location: item.location?.name || '',
          description: item.description,
          tags: item.tags,
          locationId: item.locationId,
          imageUrl: item.imageUrl,
          quantity: item.quantity
        })));
        return fetchedItems.length; // Return the count of items
      } else {
        setError(data.message || "Failed to fetch recent items");
        setRecentItems([]);
        return 0; // Return 0 on API error
      }
    } catch (err: any) {
      setError(err.message || "Failed to fetch recent items");
      setRecentItems([]);
      return 0; // Return 0 on catch error
    }
  };

  const fetchStatistics = async () => {
    try {
      const response = await getUsageStatistics("monthly");

      if (response.status === 204 || response.headers.get('content-length') === '0') {
        return { totalLocations: 0, createdCount: 0 }; // Return 0 if no content
      }

      const data = await response.json();
      if (response.ok) {
        return {
          totalLocations: data.totalLocations || 0,
          createdCount: data.ActionCounts?.Created || 0,
        }; // Return statistics data
      } else {
        setError(data.message || "Failed to fetch statistics");
        return null; // Return null on API error
      }
    } catch (err: any) {
      setError(err.message || "Failed to fetch statistics");
      return null; // Return null on catch error
    }
  };

  useEffect(() => {
    const loadDashboardData = async () => {
      setLoading(true);
      setError(null);
      try {
        const [recentItemsCount, statsResult] = await Promise.all([
          fetchRecentItems(),
          fetchStatistics(),
        ]);

        setStatistics(prevStats => ({
          ...prevStats,
          totalItems: recentItemsCount,
          totalLocations: statsResult?.totalLocations || 0,
          recentItemsAdded: statsResult?.createdCount || recentItemsCount || 0, // Fallback to total items if 'Created' is 0
          itemsNeedingUpdate: 0, // Still hardcoded
        }));
      } catch (err: any) {
        setError(err.message || "Failed to load dashboard data");
      } finally {
        setLoading(false);
      }
    };
    loadDashboardData();
  }, []);

  const handleDeleteItem = async (itemId: string) => {
    Alert.alert(
      "Xác nhận xóa",
      "Bạn có chắc chắn muốn xóa món đồ này?",
      [
        {
          text: "Hủy",
          style: "cancel",
        },
        {
          text: "Xóa",
          onPress: async () => {
            try {
              const response = await fetchWithAuth(itemsDelete(Number(itemId)), {
                method: "DELETE",
              });
              if (response.ok) {
                Alert.alert("Thành công", "Món đồ đã được xóa.");
                fetchRecentItems(); // Refresh the list after deletion
                // fetchStatistics(); // Không cần gọi lại, totalItems đã được cập nhật bởi fetchRecentItems
              } else {
                const errorData = await response.json();
                Alert.alert("Lỗi", errorData.message || "Không thể xóa món đồ.");
              }
            } catch (error: any) {
              Alert.alert("Lỗi", error.message || "Đã xảy ra lỗi khi xóa món đồ.");
            }
          },
        },
      ],
      { cancelable: true }
    );
  };

  const handleDetailPress = (itemId: string) => {
    router.push(`/product-detail?id=${itemId}`);
  };

  const statsData = [
    { label: "Tổng số đồ vật", value: statistics.totalItems },
    { label: "Phòng / khu vực", value: statistics.totalLocations },
    { label: "Cần cập nhật", value: statistics.itemsNeedingUpdate },
    { label: "Thêm gần đây", value: statistics.recentItemsAdded },
  ];

  const handleTabPress = (index: number) => {
    setActiveTab(index);
    switch (index) {
      case 0:
        // đang ở dashboard rồi
        break;
      case 1:
        router.push("/Search Screen");
        break;
      case 2:
        router.push("/LocationManager");
        break;
      case 3:
        router.push("/profile"); // hoặc đổi thành trang khác nếu có
        break;
    }
  };

  const handleAddPress = () => {
    router.push("/add-item");
  };

  const handleAvatarPress = () => {
    router.push("/profile");
  };

  return (
    <View style={styles.container}>
      <AppHeader 
        title="Trang chủ"
        onAvatarPress={handleAvatarPress}
      />

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {loading ? (
          <Text style={styles.loadingText}>Đang tải số liệu thống kê...</Text>
        ) : error ? (
          <Text style={styles.errorText}>Lỗi: {error}</Text>
        ) : (
          <View style={styles.statsContainer}>
            {statsData.map((stat, index) => (
              <StatsCard key={index} label={stat.label} value={stat.value} />
            ))}
          </View>
        )}

        <Text style={styles.sectionTitle}>Đồ vật gần đây</Text>

        <View style={styles.itemsContainer}>
          {recentItems.length > 0 ? (
            recentItems.map(item => (
              <ItemCard
                key={item.id}
                id={item.id}
                icon={item.icon}
                name={item.name}
                location={item.location}
                onDetailPress={handleDetailPress}
                onDeletePress={handleDeleteItem}
              />
            ))
          ) : (
            <Text style={styles.noItemsText}>Không có món đồ nào gần đây.</Text>
          )}
        </View>
      </ScrollView>

      <BottomNavigation 
        activeTab={activeTab} 
        onTabPress={handleTabPress} 
        onAddPress={handleAddPress} 
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#f5f5f7" },
  content: { flex: 1 },
  statsContainer: {
    flexDirection: "row",
    flexWrap: "wrap",
    paddingHorizontal: 20,
    paddingTop: 20,
    paddingBottom: 10,
    justifyContent: "space-between"
  },
  sectionTitle: {
    fontWeight: "600",
    fontSize: 18,
    marginBottom: 16,
    paddingHorizontal: 20,
    color: "#000"
  },
  itemsContainer: {
    paddingHorizontal: 20,
    paddingBottom: 20,
  },
  loadingText: {
    fontSize: 16,
    fontWeight: "bold",
    textAlign: "center",
    marginTop: 20,
  },
  errorText: {
    fontSize: 16,
    fontWeight: "bold",
    textAlign: "center",
    marginTop: 20,
    color: "red",
  },
  noItemsText: {
    fontSize: 16,
    textAlign: "center",
    marginTop: 20,
    color: "#666",
  },
});
