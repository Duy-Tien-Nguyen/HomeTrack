// useColorScheme.test.ts
import { renderHook } from '@testing-library/react-hooks';
import { useColorScheme } from './useColorScheme';
import * as ReactNative from 'react-native';

describe('useColorScheme', () => {
  it('should return "light" before hydration', () => {
    jest.spyOn(ReactNative, 'useColorScheme').mockReturnValue('dark');
    const { result } = renderHook(() => useColorScheme());
    expect(result.current).toBe('light');
  });

  it('should return actual color scheme after hydration', async () => {
    jest.spyOn(ReactNative, 'useColorScheme').mockReturnValue('dark');
    const { result, waitForNextUpdate } = renderHook(() => useColorScheme());

    // simulate hydration effect
    await waitForNextUpdate();

    expect(result.current).toBe('dark');
  });
});
