import React from "react";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface SearchResultItemProps {
  id: string;
  icon: keyof typeof MaterialIcons.glyphMap;
  name: string;
  location: string;
  tags?: string[];
  onPress: (id: string) => void;
}

const SearchResultItem: React.FC<SearchResultItemProps> = ({
  id,
  icon,
  name,
  location,
  tags = [],
  onPress
}) => {
  return (
    <TouchableOpacity 
      style={styles.container}
      onPress={() => onPress(id)}
      activeOpacity={0.7}
    >
      <View style={styles.iconContainer}>
        <MaterialIcons name={icon} size={24} color="#4b44f6" />
      </View>
      
      <View style={styles.contentContainer}>
        <Text style={styles.name}>{name}</Text>
        <Text style={styles.location}>{location}</Text>
        
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
      
      <MaterialIcons name="chevron-right" size={20} color="#ccc" />
    </TouchableOpacity>
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
});

export default SearchResultItem;