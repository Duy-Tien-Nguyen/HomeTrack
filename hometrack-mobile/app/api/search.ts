import { baseUrl, port, fetchWithAuth } from "./api";

// Hàm gọi API tìm kiếm theo keyword
export async function searchItems(keyword: string) {
  const response = await fetchWithAuth(`${baseUrl}:${port}/api/search/items?keyword=${encodeURIComponent(keyword)}`);
  if (!response.ok) throw new Error('Network response was not ok');
  return response.json();
}

// Hàm gọi API tìm kiếm nâng cao
export async function searchAdvanced(params: { tags?: string[]; color?: string; sortBy?: string }) {
  const response = await fetchWithAuth(`${baseUrl}:${port}/api/search/advanced`, {
    method: 'POST', // Changed to POST as per Swagger UI for request body
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(params),
  });
  if (!response.ok) throw new Error('Network response was not ok');
  return response.json();
}
 

export const statisticsUsage = `${baseUrl}:${port}/api/statistics/usage`; 

// Add default export
export default {
  searchItems,
  searchAdvanced,
  statisticsUsage,
}; 