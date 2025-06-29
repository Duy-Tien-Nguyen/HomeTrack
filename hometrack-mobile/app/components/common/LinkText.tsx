import React from "react";
import { Text, TouchableOpacity, StyleSheet, TextStyle } from "react-native";

interface LinkTextProps {
  text: string;
  onPress: () => void;
  style?: TextStyle;
}

export default function LinkText({ text, onPress, style }: LinkTextProps) {
  return (
    <TouchableOpacity onPress={onPress}>
      <Text style={[styles.link, style]}>{text}</Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  link: {
    color: "#4b44f6",
    fontSize: 14,
  },
});
