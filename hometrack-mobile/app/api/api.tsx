import AsyncStorage from "@react-native-async-storage/async-storage";

export const baseUrl = "http://34.10.134.196";
export const port = "8001";

export const login = `${baseUrl}:${port}/api/auth/login`;
export const accessToken = `${baseUrl}:${port}/api/auth/access_token`;
export const logout = `${baseUrl}:${port}/api/auth/logout`;
export const resetPassword = `${baseUrl}:${port}/api/auth/reset_password`;
export const forgotPassword = `${baseUrl}:${port}/api/auth/forgot_password`;
export const register = `${baseUrl}:${port}/api/registration/register`;
export const sendOtp = `${baseUrl}:${port}/api/registration/resendOTP`;
export const verifyOtp = `${baseUrl}:${port}/api/registration/verify`;
export const itemsCreate = `${baseUrl}:${port}/api/items/create`;
export const itemsGetById = (id: string) => `${baseUrl}:${port}/api/items/getById/${id}`;
export const itemsUpdate = (id: string) => `${baseUrl}:${port}/api/items/update/${id}`;
export const itemsDelete = (id: number) => `${baseUrl}:${port}/api/items/delete/${id}`;
export const itemsByLocation = (locationId: number) => `${baseUrl}:${port}/api/items/by-location/${locationId}`;
export const locationsCreate = `${baseUrl}:${port}/api/locations/create`;
export const locationsGetAll = `${baseUrl}:${port}/api/locations/getAll`;
export const locationsGetById = (id: number) => `${baseUrl}:${port}/api/locations/getById/${id}`;
export const locationsUpdate = (id: number) => `${baseUrl}:${port}/api/locations/update/${id}`;
export const locationsDelete = (id: number) => `${baseUrl}:${port}/api/locations/delete/${id}`;
export const searchItems = `${baseUrl}:${port}/api/search/items`;
export const searchAdvanced = `${baseUrl}:${port}/api/search/advanced`;
export const statisticsUsage = `${baseUrl}:${port}/api/statistics/usage`;
export const topMovedStatistics = `${baseUrl}:${port}/api/statistics/top-moved`;
export const itemsGetAll = `${baseUrl}:${port}/api/items/getAll`;
export const myProfile = `${baseUrl}:${port}/api/auth/myprofile`;


export const packagesGetAll = `${baseUrl}:${port}/api/packages/all`;
export const subscriptionsGetMy = `${baseUrl}:${port}/api/subscriptions/by-myself`;
export const subscriptionsRegister = `${baseUrl}:${port}/api/subscriptions/regis-subcription`;

export const itemImage = (id: number) => `${baseUrl}:${port}/api/items/${id}/image`;

/**
 * Hàm fetchWithAuth: tự động thêm accessToken vào header Authorization
 * @param url URL endpoint
 * @param options fetch options (headers, method, body...)
 */
export async function fetchWithAuth(url: string, options: any = {}) {
  const token = await AsyncStorage.getItem("accessToken");
  const headers = {
    ...(options.headers || {}),
    Authorization: token ? `Bearer ${token}` : undefined,
  };
  return fetch(url, { 
    ...options, 
    headers,
    cache: 'no-store'
  });
}

export async function getUsageStatistics(timeframe: string) {
  const url = new URL(statisticsUsage);
  url.searchParams.append("timeframe", timeframe);
  return fetchWithAuth(url.toString(), {
    method: "GET",
  });
}

export async function getTopMovedStatistics(timeframe: string) {
  const url = new URL(topMovedStatistics);
  url.searchParams.append("timeframe", timeframe);
  return fetchWithAuth(url.toString(), {
    method: "GET",
  });
}

export async function getMyProfile() {
  return fetchWithAuth(myProfile, {
    method: "GET",
  });
}

/**
 * Gợi ý tag từ AI dựa trên ảnh và context text
 * @param imageUri uri của ảnh
 * @param itemContextText mô tả hoặc context của item
 */
export async function suggestTagsForImage(imageUri: string, itemContextText: string) {
  const formData = new FormData();
  formData.append('ImageFile', {
    uri: imageUri,
    name: 'item-image.jpg',
    type: 'image/jpeg',
  } as any);
  formData.append('ItemContextText', itemContextText);

  const response = await fetchWithAuth(`${baseUrl}:${port}/api/items/suggest-tags-for-image`, {
    method: 'POST',
    headers: {}, 
    body: formData,
  });
  const data = await response.json();
  if (!data.isSuccess) throw new Error(data.errorMessage || 'Không lấy được gợi ý tag');
  return data.suggestedTags;
}

// Add default export
export default {
  baseUrl,
  port,
  login,
  accessToken,
  logout,
  resetPassword,
  forgotPassword,
  register,
  sendOtp,
  verifyOtp,
  itemsCreate,
  itemsGetById,
  itemsUpdate,
  itemsDelete,
  itemsByLocation,
  locationsCreate,
  locationsGetAll,
  locationsGetById,
  locationsUpdate,
  locationsDelete,
  searchItems,
  searchAdvanced,
  statisticsUsage,
  topMovedStatistics,
  itemsGetAll,
  myProfile,
  packagesGetAll,
  subscriptionsGetMy,
  subscriptionsRegister,
  itemImage,
  fetchWithAuth,
  getUsageStatistics,
  getTopMovedStatistics,
  getMyProfile,
  suggestTagsForImage,
};
