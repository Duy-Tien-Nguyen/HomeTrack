import React from "react";
import { View, Text, StyleSheet } from "react-native";

const Logo = () => (
  <View style={styles.container}>
    <View style={styles.icon}>
      <Text style={styles.iconText}>HT</Text>
    </View>
    <Text style={styles.text}>HomeTrack</Text>
  </View>
);

const styles = StyleSheet.create({
  container: { alignItems: "center", marginBottom: 24 },
  icon: {
    backgroundColor: "#4b44f6",
    borderRadius: 8,
    padding: 20,
    marginBottom: 8,
  },
  iconText: {
    color: "white",
    fontWeight: "bold",
    fontSize: 28,
  },
  text: {
    fontWeight: "bold",
    fontSize: 22,
  },
});

export default Logo;
