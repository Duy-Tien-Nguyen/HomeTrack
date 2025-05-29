import React, { useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
} from "react-native";
import { useRouter } from "expo-router";

import AppHeader from "./components/AppHeader";
import StatsCard from "./components/StatsCard";
import ItemCard from "./components/ItemCard";
import BottomNavigation from "./components/BottomNavigation";

export default function Dashboard() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState(0);

  const statsData = [
    { label: "Tổng số đồ vật", value: 128 },
    { label: "Phòng / khu vực", value: 8 },
    { label: "Cần cập nhật", value: 5 },
    { label: "Thêm gần đây", value: 12 },
  ];

  const recentItems = [
    { 
      id: "1", 
      icon: "laptop" as const, 
      name: "Máy tính xách tay", 
      location: "Phòng làm việc > Bàn" 
    },
    { 
      id: "2", 
      icon: "book" as const, 
      name: "Sách thiết kế", 
      location: "Phòng khách > Kệ sách" 
    },
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

  const handleItemPress = (itemId: string) => {
    router.push(`/product-detail?id=${itemId}`);
  };

  return (
    <View style={styles.container}>
      <AppHeader 
        title="Trang chủ"
        onAvatarPress={handleAvatarPress}
      />

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        <View style={styles.statsContainer}>
          {statsData.map((stat, index) => (
            <StatsCard key={index} label={stat.label} value={stat.value} />
          ))}
        </View>

        <Text style={styles.sectionTitle}>Đồ vật gần đây</Text>

        <View style={styles.itemsContainer}>
          {recentItems.map(item => (
            <ItemCard
              key={item.id}
              icon={item.icon}
              name={item.name}
              location={item.location}
              onPress={() => handleItemPress(item.id)}
            />
          ))}
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
});
