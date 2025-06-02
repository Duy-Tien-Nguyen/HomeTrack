import React from "react";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface AppHeaderProps {
  title: string;
  avatarText?: string;
  onAvatarPress?: () => void;
  showBackButton?: boolean;
  onBackPress?: () => void;
  rightComponent?: React.ReactNode;
}

const AppHeader: React.FC<AppHeaderProps> = ({
  title,
  avatarText = "NT",
  onAvatarPress,
  showBackButton = false,
  onBackPress,
  rightComponent,
}) => {
  return (
    <View style={styles.header}>
      <View style={styles.leftSection}>
        {showBackButton && (
          <TouchableOpacity
            onPress={onBackPress}
            activeOpacity={0.7}
            style={styles.backButton}
          >
            <MaterialIcons name="arrow-back" size={24} color="#000" />
          </TouchableOpacity>
        )}
        <Text style={styles.headerTitle}>{title}</Text>
      </View>

      <View style={styles.rightSection}>
        {rightComponent ? (
          rightComponent
        ) : (
          // Chỉ render avatar khi có onAvatarPress
          onAvatarPress ? (
            <TouchableOpacity
              style={styles.avatar}
              onPress={onAvatarPress}
              activeOpacity={0.7}
            >
              <Text style={styles.avatarText}>{avatarText}</Text>
            </TouchableOpacity>
          ) : null
        )}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingHorizontal: 20,
    paddingTop: 50,
    paddingBottom: 20,
    backgroundColor: "#fff",
    borderBottomWidth: 1,
    borderBottomColor: "#e0e0e0",
  },
  leftSection: {
    flexDirection: "row",
    alignItems: "center",
    flex: 1,
  },
  backButton: {
    marginRight: 12,
    padding: 4,
  },
  headerTitle: {
    fontSize: 22,
    fontWeight: "600",
    color: "#000",
  },
  rightSection: {
    alignItems: "center",
  },
  avatar: {
    backgroundColor: "#4b44f6",
    width: 40,
    height: 40,
    borderRadius: 20,
    justifyContent: "center",
    alignItems: "center",
  },
  avatarText: {
    color: "white",
    fontWeight: "600",
    fontSize: 16,
  },
});

export default AppHeader;
