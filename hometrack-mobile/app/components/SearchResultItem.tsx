import React from "react";
import { View, Text, StyleSheet, TouchableOpacity, Alert } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface SearchResultItemProps {
  id: number;
  icon: keyof typeof MaterialIcons.glyphMap;
  name: string;
  location: string;
  tags?: string[];
  onPress: (id: string) => void;
  onDelete: (id: number) => void;
}

const SearchResultItem: React.FC<SearchResultItemProps> = ({
  id,
  icon,
  name,
  location,
  tags = [],
  onPress,
  onDelete
}) => {
  const handleDelete = () => {
    Alert.alert("Xác nhận xóa", "Bạn có chắc chắn muốn xóa đồ vật này?", [
      { text: "Hủy", style: "cancel" },
      { text: "Xóa", style: "destructive", onPress: () => onDelete(id) },
    ]);
  };

  return (
    <View style={styles.container}>
      <TouchableOpacity 
        style={styles.itemContent}
        onPress={() => onPress(id.toString())}
        activeOpacity={0.7}
      >
        <View style={styles.iconContainer}>
          <MaterialIcons name={icon} size={24} color="#4b44f6" />
        </View>
        
        <View style={styles.contentContainer}>
          <Text style={styles.name}>{name}</Text>
          {location ? (
            <Text style={styles.location}>Vị trí: {location}</Text>
          ) : null}
          
          {tags.length > 0 && (
            <View style={styles.tagsContainer}>
              {tags.map((tag, index) => (
                <View key={index} style={styles.tagBadge}>
                  <Text style={styles.tagText}>{tag}</Text>
                </View>
              ))}
            </View>
          )}
        </View>
      </TouchableOpacity>
      
      <TouchableOpacity 
        style={styles.deleteButton}
        onPress={handleDelete}
      >
        <MaterialIcons name="delete" size={20} color="#fff" />
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "#fff",
    padding: 16,
    marginHorizontal: 20,
    marginBottom: 8,
    borderRadius: 12,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  itemContent: {
    flexDirection: "row",
    alignItems: "center",
    flex: 1,
  },
  iconContainer: {
    width: 40,
    height: 40,
    borderRadius: 10,
    backgroundColor: "#f0f0f5",
    justifyContent: "center",
    alignItems: "center",
    marginRight: 12,
  },
  contentContainer: {
    flex: 1,
  },
  name: {
    fontSize: 16,
    fontWeight: "600",
    color: "#000",
    marginBottom: 4,
  },
  location: {
    fontSize: 14,
    color: "#666",
    marginBottom: 8,
  },
  tagsContainer: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 6,
  },
  tagBadge: {
    backgroundColor: "#e8f2ff",
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 10,
  },
  tagText: {
    fontSize: 12,
    color: "#4b44f6",
    fontWeight: "500",
  },
  deleteButton: {
    backgroundColor: "#dc3545",
    padding: 8,
    borderRadius: 8,
    marginLeft: 10,
  },
});

export default SearchResultItem;