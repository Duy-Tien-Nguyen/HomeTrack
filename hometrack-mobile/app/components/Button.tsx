import React from "react";
import { TouchableOpacity, Text, StyleSheet, GestureResponderEvent, ViewStyle, TextStyle } from "react-native";

interface ButtonProps {
  title: string;
  onPress?: (event: GestureResponderEvent) => void;
  disabled?: boolean;
  style?: ViewStyle | ViewStyle[];
  textStyle?: TextStyle | TextStyle[];
  variant?: 'primary' | 'secondary' | 'outline';
  size?: 'small' | 'medium' | 'large';
}

export default function Button({ 
  title, 
  onPress, 
  disabled = false,
  style,
  textStyle,
  variant = 'primary',
  size = 'medium'
}: ButtonProps) {
  
  // Get base styles based on variant
  const getVariantStyles = () => {
    switch (variant) {
      case 'secondary':
        return {
          button: styles.secondaryButton,
          text: styles.secondaryText
        };
      case 'outline':
        return {
          button: styles.outlineButton,
          text: styles.outlineText
        };
      default:
        return {
          button: styles.primaryButton,
          text: styles.primaryText
        };
    }
  };

  // Get size styles
  const getSizeStyles = () => {
    switch (size) {
      case 'small':
        return {
          button: styles.smallButton,
          text: styles.smallText
        };
      case 'large':
        return {
          button: styles.largeButton,
          text: styles.largeText
        };
      default:
        return {
          button: styles.mediumButton,
          text: styles.mediumText
        };
    }
  };

  const variantStyles = getVariantStyles();
  const sizeStyles = getSizeStyles();

  return (
    <TouchableOpacity
      style={[
        styles.baseButton,
        variantStyles.button,
        sizeStyles.button,
        disabled && styles.disabledButton,
        style
      ]}
      onPress={disabled ? undefined : onPress}
      disabled={disabled}
      activeOpacity={disabled ? 1 : 0.7}
    >
      <Text style={[
        styles.baseText,
        variantStyles.text,
        sizeStyles.text,
        disabled && styles.disabledText,
        textStyle
      ]}>
        {title}
      </Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  // Base styles
  baseButton: {
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
    minHeight: 44,
  },
  baseText: {
    fontWeight: "600",
    textAlign: "center",
  },

  // Variant styles
  primaryButton: {
    backgroundColor: "#4c6ef5",
  },
  primaryText: {
    color: "#fff",
  },
  
  secondaryButton: {
    backgroundColor: "#f8f9fa",
    borderWidth: 1,
    borderColor: "#dee2e6",
  },
  secondaryText: {
    color: "#495057",
  },

  outlineButton: {
    backgroundColor: "transparent",
    borderWidth: 1,
    borderColor: "#4c6ef5",
  },
  outlineText: {
    color: "#4c6ef5",
  },

  // Size styles
  smallButton: {
    paddingVertical: 8,
    paddingHorizontal: 16,
    minHeight: 36,
  },
  smallText: {
    fontSize: 14,
  },

  mediumButton: {
    paddingVertical: 12,
    paddingHorizontal: 20,
    minHeight: 44,
  },
  mediumText: {
    fontSize: 16,
  },

  largeButton: {
    paddingVertical: 16,
    paddingHorizontal: 24,
    minHeight: 52,
  },
  largeText: {
    fontSize: 18,
  },

  // Disabled styles
  disabledButton: {
    backgroundColor: "#e9ecef",
    borderColor: "#e9ecef",
  },
  disabledText: {
    color: "#adb5bd",
  },
});