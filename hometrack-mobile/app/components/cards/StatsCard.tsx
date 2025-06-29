import React from "react";
import { View, Text, StyleSheet, Dimensions } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

const screenWidth = Dimensions.get("window").width;
const cardWidth = (screenWidth - 60) / 2;

interface StatsCardProps {
  label: string;
  value: number;
  icon?: keyof typeof MaterialIcons.glyphMap;
  subLabel?: string;
}

const StatsCard: React.FC<StatsCardProps> = ({ label, value, icon, subLabel }) => {
  return (
    <View style={styles.card}>
      {icon && <MaterialIcons name={icon} size={32} color="#4b44f6" style={{ marginBottom: 8 }} />}
      <Text style={styles.cardValue}>{value}</Text>
      <Text style={styles.cardLabel}>{label}</Text>
      {subLabel && <Text style={styles.cardSubLabel}>{subLabel}</Text>}
    </View>
  );
};

const styles = StyleSheet.create({
  card: {
    width: cardWidth,
    backgroundColor: "#fff",
    padding: 20,
    borderRadius: 16,
    marginBottom: 16,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.08,
    shadowRadius: 8,
    elevation: 3,
    alignItems: "center",
  },
  cardValue: { 
    fontSize: 32, 
    fontWeight: "700", 
    marginBottom: 8,
    color: "#000"
  },
  cardLabel: { 
    fontSize: 14, 
    color: "#666",
    textAlign: "center",
    lineHeight: 18
  },
  cardSubLabel: {
    fontSize: 12,
    color: "#999",
    textAlign: "center",
    marginTop: 2,
  },
});

export default StatsCard;