import React, { useState, useEffect } from "react";
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  SafeAreaView,
} from "react-native";
import { useRouter } from "expo-router";

import AppHeader from "./components/AppHeader";
import SearchInput from "./components/SearchInput";
import FilterTags from "./components/FilterTags";
import SearchResultItem from "./components/SearchResultItem";
import BottomNavigation from "./components/BottomNavigation";

const mockItems = [
  {
    id: "1",
    name: "Máy tính xách tay",
    location: "Phòng làm việc > Bàn",
    icon: "laptop" as const,
    tags: ["điện tử", "công việc"],
    category: "electronics",
  },
  {
    id: "2",
    name: "Sách thiết kế",
    location: "Phòng khách > Kệ sách",
    icon: "book" as const,
    tags: ["sách", "thiết kế"],
    category: "books",
  },
];

const mockFilterTags = [
  { id: "all", label: "Tất cả", active: true },
  { id: "tag", label: "Tag", active: false },
  { id: "color", label: "Màu sắc", active: false },
  { id: "time", label: "Thời gian", active: false },
];

export default function SearchScreen() {
  const router = useRouter();

  const [searchQuery, setSearchQuery] = useState("");
  const [filterTags, setFilterTags] = useState(mockFilterTags);
  const [searchResults, setSearchResults] = useState(mockItems);
  const [activeTab, setActiveTab] = useState(1);

  useEffect(() => {
    if (searchQuery.trim() === "") {
      setSearchResults(mockItems);
    } else {
      const filtered = mockItems.filter(
        (item) =>
          item.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          item.location.toLowerCase().includes(searchQuery.toLowerCase()) ||
          item.tags.some((tag) =>
            tag.toLowerCase().includes(searchQuery.toLowerCase())
          )
      );
      setSearchResults(filtered);
    }
  }, [searchQuery]);

  const handleSearchChange = (text: string) => setSearchQuery(text);
  const handleSearchClear = () => setSearchQuery("");

  const handleTagPress = (tagId: string) => {
    const updatedTags = filterTags.map((tag) => ({
      ...tag,
      active: tag.id === tagId,
    }));
    setFilterTags(updatedTags);
    if (tagId === "all") {
      setSearchResults(mockItems);
    } else {
      const filtered = mockItems.filter((item) => {
        switch (tagId) {
          case "tag":
            return item.tags.length > 0;
          case "color":
            return item.category === "electronics";
          case "time":
            return item.id === "1" || item.id === "2";
          default:
            return true;
        }
      });
      setSearchResults(filtered);
    }
  };

  const handleItemPress = (itemId: string) => {
    router.push(`/product-detail?id=${itemId}`);
  };

  const handleAddPress = () => {
    router.push("/add-item");
  };

  const handleTabPress = (index: number) => {
    setActiveTab(index);
    switch (index) {
      case 0:
        router.push("/dashboard");
        break;
      case 1:
        break; // Đang ở search screen
      case 2:
        router.push("/LocationManager");
        break;
      case 3:
        router.push("/profile");
        break;
    }
  };

  const handleBackPress = () => router.back();

  const handleAvatarPress = () => router.push("/profile");

  return (
    <SafeAreaView style={styles.container}>
      <AppHeader
        title="Tìm kiếm"
        showBackButton={true}
        onBackPress={handleBackPress}
        onAvatarPress={handleAvatarPress} // Đây là prop để hiện avatar
        rightComponent={<View />}
      />

      <SearchInput
        value={searchQuery}
        onChangeText={handleSearchChange}
        onClear={handleSearchClear}
        placeholder="Tìm kiếm đồ vật..."
      />

      <FilterTags tags={filterTags} onTagPress={handleTagPress} />

      <View style={styles.resultsSection}>
        <Text style={styles.resultsTitle}>
          Kết quả tìm kiếm ({searchResults.length})
        </Text>
        <FlatList
          data={searchResults}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <SearchResultItem
              id={item.id}
              icon={item.icon}
              name={item.name}
              location={item.location}
              tags={item.tags}
              onPress={handleItemPress}
            />
          )}
          contentContainerStyle={styles.listContent}
          showsVerticalScrollIndicator={false}
          ListEmptyComponent={
            <View style={styles.emptyContainer}>
              <Text style={styles.emptyText}>Không tìm thấy kết quả nào</Text>
            </View>
          }
        />
      </View>

      <BottomNavigation
        activeTab={activeTab}
        onTabPress={handleTabPress}
        onAddPress={handleAddPress}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#f5f5f7" },
  resultsSection: { flex: 1, paddingTop: 16 },
  resultsTitle: {
    fontSize: 16,
    fontWeight: "600",
    color: "#000",
    paddingHorizontal: 20,
    marginBottom: 12,
  },
  listContent: { paddingBottom: 20 },
  emptyContainer: {
    alignItems: "center",
    justifyContent: "center",
    paddingVertical: 40,
  },
  emptyText: { fontSize: 16, color: "#999" },
});

