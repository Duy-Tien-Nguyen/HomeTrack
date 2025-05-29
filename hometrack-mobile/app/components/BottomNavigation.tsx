import React from "react";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { MaterialIcons } from "@expo/vector-icons";

interface BottomNavigationProps {
  activeTab: number;
  onTabPress: (index: number) => void;
  onAddPress: () => void;
  variant?: "home" | "default";
}

const TabItem = ({
  icon,
  label,
  active,
  onPress,
  variant = "default"
}: {
  icon: keyof typeof MaterialIcons.glyphMap;
  label: string;
  active: boolean;
  onPress: () => void;
  variant?: "home" | "default";
}) => (
  <TouchableOpacity style={styles.tab} onPress={onPress} activeOpacity={0.7}>
    <MaterialIcons
      name={icon}
      size={variant === "home" ? 28 : 24}
      color={active ? "#4b44f6" : "#888"}
    />
    <Text
      style={[
        variant === "home" ? styles.tabLabelHome : styles.tabLabel,
        { color: active ? "#4b44f6" : "#888" },
        active && variant === "home" && { fontWeight: "700" },
      ]}
    >
      {label}
    </Text>
  </TouchableOpacity>
);

const AddButton = ({ onPress }: { onPress: () => void }) => (
  <TouchableOpacity
    style={styles.addButton}
    onPress={onPress}
    activeOpacity={0.7}
  >
    <MaterialIcons name="add" size={32} color="#fff" />
  </TouchableOpacity>
);

const BottomNavigation: React.FC<BottomNavigationProps> = ({
  activeTab,
  onTabPress,
  onAddPress,
  variant = "default"
}) => {
  return (
    <View
      style={[
        styles.bottomNav,
        variant === "home" ? styles.bottomNavHome : null,
      ]}
    >
      <TabItem
        icon="home"
        label="Trang chủ"
        active={activeTab === 0}
        onPress={() => onTabPress(0)}
        variant={variant}
      />
      <TabItem
        icon="search"
        label="Tìm kiếm"
        active={activeTab === 1}
        onPress={() => onTabPress(1)}
        variant={variant}
      />
      <View style={styles.addButtonContainer}>
        <AddButton onPress={onAddPress} />
      </View>
      <TabItem
        icon="location-on"
        label="Vị trí"
        active={activeTab === 2}
        onPress={() => onTabPress(2)}
        variant={variant}
      />
      <TabItem
        icon="settings"
        label="Cài đặt"
        active={activeTab === 3}
        onPress={() => onTabPress(3)}
        variant={variant}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  bottomNav: {
    position: "absolute",
    bottom: 0,
    left: 0,
    right: 0,
    height: 70,
    flexDirection: "row",
    backgroundColor: "#fff",
    borderTopWidth: 1,
    borderTopColor: "#e0e0e0",
    alignItems: "center",
    paddingTop: 10,
    paddingBottom: 10,
    zIndex: 10,
  },
  bottomNavHome: {
    height: 80,
    borderTopColor: "#ccc",
    paddingTop: 12,
    paddingBottom: 14,
  },
  tab: {
    flex: 1,
    alignItems: "center",
    paddingVertical: 10,
  },
  tabLabel: {
    fontSize: 12,
    marginTop: 4,
    fontWeight: "600",
  },
  tabLabelHome: {
    fontSize: 14,
    marginTop: 6,
    fontWeight: "600",
  },
  addButtonContainer: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
  },
  addButton: {
    width: 60,
    height: 60,
    borderRadius: 30,
    backgroundColor: "#4b44f6",
    justifyContent: "center",
    alignItems: "center",
    shadowColor: "#4b44f6",
    shadowOffset: { width: 0, height: 6 },
    shadowOpacity: 0.5,
    shadowRadius: 10,
    elevation: 12,
    position: "absolute",
    top: -30,
  },
});

export default BottomNavigation;
