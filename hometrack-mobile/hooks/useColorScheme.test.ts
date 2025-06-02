import { useThemeColor } from './useThemeColor';

// Mock useColorScheme
jest.mock('@/hooks/useColorScheme', () => ({
  useColorScheme: jest.fn(),
}));

// Mock Colors
jest.mock('@/constants/Colors', () => ({
  Colors: {
    light: {
      text: '#000000',
      background: '#ffffff',
    },
    dark: {
      text: '#ffffff',
      background: '#000000',
    },
  },
}));

import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

describe('useThemeColor', () => {
  it('returns light prop when theme is light and prop is provided', () => {
    (useColorScheme as jest.Mock).mockReturnValue('light');
    const color = useThemeColor({ light: '#111111' }, 'text');
    expect(color).toBe('#111111');
  });

  it('returns dark prop when theme is dark and prop is provided', () => {
    (useColorScheme as jest.Mock).mockReturnValue('dark');
    const color = useThemeColor({ dark: '#222222' }, 'text');
    expect(color).toBe('#222222');
  });

  it('returns fallback color from Colors.light when no prop is provided', () => {
    (useColorScheme as jest.Mock).mockReturnValue('light');
    const color = useThemeColor({}, 'background');
    expect(color).toBe(Colors.light.background);
  });

  it('returns fallback color from Colors.dark when no prop is provided', () => {
    (useColorScheme as jest.Mock).mockReturnValue('dark');
    const color = useThemeColor({}, 'background');
    expect(color).toBe(Colors.dark.background);
  });

  it('defaults to light theme if useColorScheme returns undefined', () => {
    (useColorScheme as jest.Mock).mockReturnValue(undefined);
    const color = useThemeColor({}, 'text');
    expect(color).toBe(Colors.light.text);
  });
});
