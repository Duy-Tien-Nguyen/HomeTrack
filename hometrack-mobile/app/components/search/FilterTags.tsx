import React from "react";
import { View, Text, StyleSheet, TouchableOpacity, ScrollView } from "react-native";

interface FilterTag {
  id: string;
  label: string;
  active: boolean;
}

interface FilterTagsProps {
  tags: FilterTag[];
  onTagPress: (tagId: string) => void;
}

const FilterTags: React.FC<FilterTagsProps> = ({ tags, onTagPress }) => {
  return (
    <View style={styles.container}>
      <ScrollView 
        horizontal 
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        {tags.map((tag) => (
          <TouchableOpacity
            key={tag.id}
            style={[
              styles.tag,
              tag.active && styles.activeTag
            ]}
            onPress={() => onTagPress(tag.id)}
            activeOpacity={0.7}
          >
            <Text style={[
              styles.tagText,
              tag.active && styles.activeTagText
            ]}>
              {tag.label}
            </Text>
          </TouchableOpacity>
        ))}
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: "#fff",
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: "#e0e0e0",
  },
  scrollContent: {
    paddingHorizontal: 20,
    gap: 8,
  },
  tag: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 20,
    backgroundColor: "#f5f5f5",
    borderWidth: 1,
    borderColor: "#e0e0e0",
  },
  activeTag: {
    backgroundColor: "#4b44f6",
    borderColor: "#4b44f6",
  },
  tagText: {
    fontSize: 14,
    color: "#666",
    fontWeight: "500",
  },
  activeTagText: {
    color: "#fff",
  },
});

export default FilterTags;