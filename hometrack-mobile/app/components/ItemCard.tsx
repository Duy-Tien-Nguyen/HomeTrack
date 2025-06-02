import React from "react";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface ItemCardProps {
  icon: keyof typeof MaterialIcons.glyphMap;
  name: string;
  location: string;
  onPress?: () => void;
}

const ItemCard: React.FC<ItemCardProps> = ({ icon, name, location, onPress }) => {
  const CardComponent = onPress ? TouchableOpacity : View;

  return (
    <CardComponent 
      style={styles.item} 
      onPress={onPress}
      activeOpacity={onPress ? 0.7 : 1}
    >
      <View style={styles.iconContainer}>
        <MaterialIcons name={icon} size={20} color="#4b44f6" />
      </View>
      <View style={styles.textContainer}>
        <Text style={styles.itemName}>{name}</Text>
        <Text style={styles.itemLocation}>{location}</Text>
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
});

export default ItemCard;