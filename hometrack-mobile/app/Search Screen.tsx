import React, { useState, useEffect, useCallback } from "react";
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  SafeAreaView,
  ActivityIndicator,
  Alert,
} from "react-native";
import { useRouter, useFocusEffect } from "expo-router";

import AppHeader from "./components/AppHeader";
import SearchInput from "./components/SearchInput";
import FilterTags from "./components/FilterTags";
import SearchResultItem from "./components/SearchResultItem";
import BottomNavigation from "./components/BottomNavigation";
import { searchItems, searchAdvanced, fetchWithAuth, itemsDelete, getMyProfile } from "./api";

// Tags dùng cho filter
const mockFilterTags = [
  { id: "all", label: "Tất cả", active: true },
  { id: "tag", label: "Theo Tag", active: false },
  { id: "color", label: "Theo Màu", active: false },
  { id: "time", label: "Theo Thời gian", active: false },
];

export default function SearchScreen() {
  const router = useRouter();

  const [searchQuery, setSearchQuery] = useState("");
  const [filterTags, setFilterTags] = useState(mockFilterTags);
  const [searchResults, setSearchResults] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState(1);
  const [isPremiumUser, setIsPremiumUser] = useState(false); // New state for premium user status

  useEffect(() => {
    const fetchUserProfile = async () => {
      try {
        const response = await getMyProfile();
        if (response.ok) {
          const userProfile = await response.json();
          setIsPremiumUser(userProfile.role === "Premium");
        } else {
          console.error("Failed to fetch user profile:", response.status);
        }
      } catch (error) {
        console.error("Error fetching user profile:", error);
      }
    };
    fetchUserProfile();
  }, []);

  const fetchSearchResults = useCallback(async () => {
    const activeFilter = filterTags.find((t) => t.active)?.id;
    if (activeFilter === "all") {
      handleBasicSearch(searchQuery);
    } else if (isPremiumUser) {
      executeSearch(searchQuery, activeFilter || "all");
    } else {
      Alert.alert("Tính năng cao cấp", "Tính năng tìm kiếm nâng cao chỉ dành cho người dùng Premium.");
      handleBasicSearch(searchQuery); // Fallback to basic search if not premium
    }
  }, [searchQuery, filterTags, isPremiumUser]);

  useFocusEffect(
    useCallback(() => {
      fetchSearchResults();
    }, [fetchSearchResults])
  );

  // Tìm kiếm cơ bản
  const handleBasicSearch = async (keyword: string) => {
    setLoading(true);
    try {
      const url = new URL(searchItems);
      if (keyword.trim() !== "") {
        url.searchParams.append("keyword", keyword.trim());
      }

      const res = await fetchWithAuth(url.toString(), {
        method: "GET",
      });

      const responseText = await res.text();

      if (res.status === 204 || responseText.length === 0) {
        setSearchResults([]);
        return;
      }

      const data = JSON.parse(responseText);
      setSearchResults(data);
    } catch (err: any) {
      console.error("Error in basic search:", err);
      Alert.alert("Lỗi", `Tìm kiếm cơ bản thất bại: ${err.message || "Phản hồi không hợp lệ"}`);
      setSearchResults([]);
    } finally {
      setLoading(false);
    }
  };

  // Tìm kiếm nâng cao
  const executeSearch = async (keyword: string, filterId: string) => {
    setLoading(true);
    let body: any = {
      tags: [],
      color: "",
      sortBy: "",
    };

    if (keyword.trim() !== "") {
      switch (filterId) {
        case "tag":
          body.tags.push(keyword.trim());
          break;
        case "color":
          body.color = keyword.trim();
          break;
        case "time":
          body.tags.push(keyword.trim()); // Keyword can still be a tag even if sorting by time
          body.sortBy = "time";
          break;
        case "all":
        default:
          body.tags.push(keyword.trim());
          break;
      }
    } else {
      // Keyword is empty
      switch (filterId) {
        case "time":
          body.sortBy = "time";
          break;
        case "tag":
        case "color":
        case "all":
        default:
          // For "all" or if a tag/color filter is active but no keyword,
          // the backend API should handle an empty/default search.
          break;
      }
    }

    try {
      const res = await fetchWithAuth(searchAdvanced, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
      });

      const responseText = await res.text();

      if (res.status === 204 || responseText.length === 0) {
        // No content to parse, return empty array
        setSearchResults([]);
        return;
      }

      const data = JSON.parse(responseText);
      setSearchResults(data);
    } catch (err: any) {
      console.error("Error in advanced search:", err);
      Alert.alert("Lỗi", `Tìm kiếm nâng cao thất bại: ${err.message || "Phản hồi không hợp lệ"}`);
      setSearchResults([]);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (text: string) => setSearchQuery(text);
  const handleSearchClear = () => setSearchQuery("");

  const handleTagPress = (tagId: string) => {
    if (tagId !== "all" && !isPremiumUser) {
      Alert.alert("Tính năng cao cấp", "Chỉ người dùng Premium mới có thể dùng các bộ lọc khác ngoài \"Tất cả\".");
      return;
    }
    const updatedTags = filterTags.map((tag) => ({
      ...tag,
      active: tag.id === tagId,
    }));
    setFilterTags(updatedTags);

    if (tagId === "all") {
      setSearchQuery(""); // reset keyword khi chọn all
    }
  };

  const handleItemPress = (itemId: string) => {
    router.push(`/product-detail?id=${itemId}`);
  };

  const handleDeleteItem = async (itemId: number) => {
    try {
      const response = await fetchWithAuth(itemsDelete(itemId), {
        method: "DELETE",
      });
      if (!response.ok) {
        const data = await response.json().catch(() => null);
        throw new Error(data?.message || `Failed to delete item: ${response.status}`);
      }
      Alert.alert("Thành công", "Đồ vật đã được xóa.");
      fetchSearchResults(); // Refresh the list after deletion
    } catch (err: any) {
      console.error("Error deleting item:", err);
      Alert.alert("Lỗi", err.message || "Không thể xóa đồ vật.");
    }
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
        break;
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
        showBackButton
        onBackPress={handleBackPress}
        onAvatarPress={handleAvatarPress}
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
          Kết quả tìm kiếm {searchResults.length}
        </Text>

        {loading ? (
          <ActivityIndicator size="large" color="#888" style={{ marginTop: 20 }} />
        ) : (
          <FlatList
            data={searchResults}
            keyExtractor={(item) => item.id.toString()} // Ensure key is string
            renderItem={({ item }) => (
              <SearchResultItem
                id={item.id}
                icon={item.icon || "inbox"}
                name={item.name}
                location={item.location}
                tags={item.tags || []}
                onPress={handleItemPress}
                onDelete={handleDeleteItem} // Pass the delete handler
              />
            )}
            contentContainerStyle={styles.listContent}
            ListEmptyComponent={
              <View style={styles.emptyContainer}>
                <Text style={styles.emptyText}>Không tìm thấy kết quả nào</Text>
              </View>
            }
          />
        )}
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
