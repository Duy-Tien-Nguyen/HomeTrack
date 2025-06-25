import React, { useState, useCallback } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  Alert,
} from "react-native";
import { useRouter, useFocusEffect } from "expo-router";

import AppHeader from "./components/AppHeader";
import StatsCard from "./components/StatsCard";
import ItemCard from "./components/ItemCard";
import BottomNavigation from "./components/BottomNavigation";
import { itemsDelete, fetchWithAuth, itemsGetAll, locationsGetAll, itemsByLocation } from "./api";
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
  createdAt: string;
  moderationStatus?: number; // 0: Pending, 1: Approved, 2: Reject
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

      if (response.status === 204 || response.headers.get('content-length') === '0') {
        setRecentItems([]);
        return { items: [], count: 0 };
      }

      let data = null;
      try {
        data = JSON.parse(await response.text());
      } catch (e) {
        setError("Failed to parse items data.");
        setRecentItems([]);
        return { items: [], count: 0 };
      }

      if (response.ok) {
        const fetchedItems = data.data || data;

        if (!Array.isArray(fetchedItems)) {
          setError("Invalid data format for items.");
          setRecentItems([]);
          return { items: [], count: 0 };
        }
        const mappedItems = fetchedItems.map((item: any) => ({
          id: item.id.toString(),
          icon: "inventory" as keyof typeof MaterialIcons.glyphMap,
          name: item.name,
          location: item.location?.name || '',
          description: item.description,
          tags: item.tags,
          locationId: item.locationId,
          imageUrl: item.imageUrl,
          quantity: item.quantity,
          createdAt: item.createdAt,
          moderationStatus: item.moderationStatus,
        }));

        setRecentItems(mappedItems);
        return { items: mappedItems, count: mappedItems.length };
      } else {
        setError(data.message || "Failed to fetch recent items");
        setRecentItems([]);
        return { items: [], count: 0 };
      }
    } catch (err: any) {
      setError(err.message || "Failed to fetch recent items");
      setRecentItems([]);
      return { items: [], count: 0 };
    }
  };

  const loadDashboardData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      // 1. Lấy tất cả các vị trí
      const locationsResponse = await fetchWithAuth(locationsGetAll);
      if (!locationsResponse.ok) {
        throw new Error("Không thể tải danh sách vị trí.");
      }
      const locations = await locationsResponse.json();
      const totalLocations = Array.isArray(locations) ? locations.length : 0;

      // 2. Lấy tất cả các mục từ mỗi vị trí
      const itemPromises = locations.map((loc: any) =>
        fetchWithAuth(itemsByLocation(loc.id)).then(res => res.ok ? res.json() : [])
      );
      const itemsByLocationArrays = await Promise.all(itemPromises);
      const allItems = itemsByLocationArrays.flat().filter(Boolean); // Lọc ra các giá trị null/undefined

      // 3. Cập nhật state cho danh sách "Đồ vật gần đây"
      const mappedItems = allItems.map((item: any) => ({
        id: item.id.toString(),
        icon: "inventory" as keyof typeof MaterialIcons.glyphMap,
        name: item.name,
        location: item.location?.name || '',
        description: item.description,
        tags: item.tags,
        locationId: item.locationId,
        imageUrl: item.imageUrl,
        quantity: item.quantity,
        createdAt: item.createdAt,
        moderationStatus: item.moderationStatus,
      }));
      setRecentItems(mappedItems);

      // 4. Tính toán các số liệu thống kê
      const totalItemsCount = allItems.length;

      // Tính toán "Thêm gần đây" từ dữ liệu item thực tế
      const oneMonthAgo = new Date();
      oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);
      const recentItemsAdded = allItems.filter(item => item.createdAt && new Date(item.createdAt) > oneMonthAgo).length;

      setStatistics(prevStats => ({
        ...prevStats,
        totalItems: totalItemsCount,
        totalLocations: totalLocations,
        recentItemsAdded: recentItemsAdded,
        itemsNeedingUpdate: 0,
      }));
    } catch (err: any) {
      setError(err.message || "Failed to load dashboard data");
    } finally {
      setLoading(false);
    }
  }, []);

  useFocusEffect(
    useCallback(() => {
      loadDashboardData();
    }, [loadDashboardData])
  );

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
    router.push(`/productDetail?id=${itemId}`);
  };

  const statsData = [
    {
      label: "Tổng số đồ vật",
      value: statistics.totalItems,
      icon: "inventory",
      subLabel: "Tất cả đồ vật bạn đã quản lý"
    },
    {
      label: "Phòng / khu vực",
      value: statistics.totalLocations,
      icon: "meeting-room",
      subLabel: "Tổng số phòng/khu vực"
    },
    {
      label: "Cần cập nhật",
      value: statistics.itemsNeedingUpdate,
      icon: "update",
      subLabel: "Đồ vật cần kiểm tra lại"
    },
    {
      label: "Thêm gần đây",
      value: statistics.recentItemsAdded,
      icon: "add-circle-outline",
      subLabel: "Đồ vật mới trong tháng"
    },
  ];

  const handleTabPress = (index: number) => {
    setActiveTab(index);
    switch (index) {
      case 0:
        // đang ở dashboard rồi
        break;
      case 1:
        router.push("/searchScreen");
        break;
      case 2:
        router.push("/locationManager");
        break;
      case 3:
        router.push("/profile");
        break;
    }
  };

  const handleAddPress = () => {
    router.push("/addItem");
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
              <StatsCard
                key={index}
                label={stat.label}
                value={stat.value}
                icon={stat.icon}
                subLabel={stat.subLabel}
              />
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
                moderationStatus={item.moderationStatus}
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
