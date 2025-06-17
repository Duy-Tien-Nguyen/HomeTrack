import React from "react";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface ItemCardProps {
  id: string;
  icon: keyof typeof MaterialIcons.glyphMap;
  name: string;
  location: string;
  onPress?: (itemId: string) => void;
  onDetailPress?: (itemId: string) => void;
  onDeletePress?: (itemId: string) => void;
}

const ItemCard: React.FC<ItemCardProps> = ({ id, icon, name, location, onPress, onDetailPress, onDeletePress }) => {
  const CardComponent = onPress ? TouchableOpacity : View;

  return (
    <CardComponent
      style={styles.item}
      onPress={() => onPress && onPress(id)}
      activeOpacity={onPress ? 0.7 : 1}
    >
      <View style={styles.iconContainer}>
        <MaterialIcons name={icon} size={20} color="#4b44f6" />
      </View>
      <View style={styles.textContainer}>
        <Text style={styles.itemName}>{name}</Text>
        <Text style={styles.itemLocation}>{location}</Text>
      </View>
      <View style={styles.buttonsContainer}>
        {onDetailPress && (
          <TouchableOpacity
            style={[styles.button, styles.detailButton]}
            onPress={() => onDetailPress(id)}
          >
            <Text style={styles.buttonText}>Chi tiết</Text>
          </TouchableOpacity>
        )}
        {onDeletePress && (
          <TouchableOpacity
            style={[styles.button, styles.deleteButton]}
            onPress={() => onDeletePress(id)}
          >
            <Text style={styles.buttonText}>Xóa</Text>
          </TouchableOpacity>
        )}
      </View>
    </CardComponent>
  );
};

const styles = StyleSheet.create({
  item: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "#fff",
    padding: 16,
    borderRadius: 12,
    marginBottom: 12,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  iconContainer: {
    width: 36,
    height: 36,
    borderRadius: 8,
    backgroundColor: "#f0f0f5",
    justifyContent: "center",
    alignItems: "center",
  },
  textContainer: {
    marginLeft: 12,
    flex: 1,
  },
  itemName: {
    fontWeight: "600",
    fontSize: 16,
    color: "#000",
    marginBottom: 4
  },
  itemLocation: {
    fontSize: 14,
    color: "#666"
  },
  buttonsContainer: {
    flexDirection: "row",
    marginLeft: 10,
  },
  button: {
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 8,
    marginLeft: 8,
  },
  detailButton: {
    backgroundColor: "#4CAF50", // Green for "Chi tiết"
  },
  deleteButton: {
    backgroundColor: "#F44336", // Red for "Xóa"
  },
  buttonText: {
    color: "#fff",
    fontWeight: "bold",
    fontSize: 12,
  },
});

export default ItemCard;