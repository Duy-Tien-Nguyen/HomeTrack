# HomeTrack Mobile - Cấu trúc dự án và luồng hoạt động

## Tổng quan dự án

HomeTrack Mobile là ứng dụng React Native được xây dựng bằng Expo để quản lý đồ vật trong nhà. Ứng dụng cho phép người dùng:
- Đăng nhập/đăng ký tài khoản
- Thêm, xem, tìm kiếm và quản lý đồ vật
- Quản lý vị trí lưu trữ đồ vật
- Tìm kiếm nâng cao (tính năng Premium)
- Xem thống kê và báo cáo

## Công nghệ sử dụng

- **Framework**: React Native với Expo
- **Navigation**: Expo Router
- **State Management**: React Hooks (useState, useEffect, useCallback)
- **Storage**: AsyncStorage
- **UI Components**: Custom components + Material Icons
- **Image Handling**: Expo Image Picker
- **API Communication**: Fetch API với authentication

## Cấu trúc thư mục

```
hometrack-mobile/
├── app/                    # Thư mục chính chứa các màn hình
│   ├── components/         # Các component tái sử dụng
│   ├── api/               # API endpoints
│   └── *.tsx              # Các màn hình chính
├── assets/                # Tài nguyên (images, fonts)
├── package.json           # Dependencies và scripts
├── app.json              # Cấu hình Expo
└── tsconfig.json         # Cấu hình TypeScript
```

## Luồng hoạt động chính

### 1. Khởi động ứng dụng
- **Entry point**: `app/index.tsx` → Chuyển hướng đến màn hình đăng nhập
- **Layout**: `app/_layout.tsx` → Định nghĩa cấu trúc navigation stack

### 2. Authentication Flow
- **Login** → **Dashboard** (nếu thành công)
- **Register** → **Verify OTP** → **Dashboard**
- **Forgot Password** → **Reset Password**

### 3. Main App Flow
- **Dashboard** → Trang chủ với thống kê và danh sách đồ vật
- **Search** → Tìm kiếm đồ vật (cơ bản + nâng cao)
- **Add Item** → Thêm đồ vật mới
- **Location Manager** → Quản lý vị trí và đồ vật theo vị trí
- **Profile** → Thông tin cá nhân và cài đặt

## Chi tiết từng file

### 📁 Configuration Files

#### `package.json`
```json
{
  "name": "hometrack-mobile",
  "main": "expo-router/entry",
  "dependencies": {
    "expo": "~53.0.9",
    "expo-router": "~5.0.6",
    "react-native": "0.79.2",
    "@react-native-async-storage/async-storage": "2.1.2"
  }
}
```
**Chức năng**: Định nghĩa dependencies, scripts và cấu hình cơ bản của dự án.

#### `app.json`
```json
{
  "expo": {
    "name": "hometrack-mobile",
    "slug": "hometrack-mobile",
    "orientation": "portrait",
    "scheme": "hometrackmobile"
  }
}
```
**Chức năng**: Cấu hình Expo app với tên, scheme, orientation và plugins.

### 📁 Core Files

#### `app/_layout.tsx`
```typescript
export default function Layout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="login" />
      <Stack.Screen name="register" />
      <Stack.Screen name="dashboard" />
      // ... other screens
    </Stack>
  );
}
```
**Chức năng**: 
- Định nghĩa cấu trúc navigation stack
- Ẩn header mặc định cho tất cả màn hình
- Đăng ký các màn hình có thể navigate

#### `app/index.tsx`
```typescript
export default function Index() {
  return <LoginScreen />;
}
```
**Chức năng**: Entry point của ứng dụng, chuyển hướng ngay lập tức đến màn hình đăng nhập.

#### `app/api.tsx`
```typescript
export const baseUrl = "http://34.10.134.196";
export const port = "8001";

export const login = `${baseUrl}:${port}/api/auth/login`;
export const itemsCreate = `${baseUrl}:${port}/api/items/create`;
// ... other endpoints

export async function fetchWithAuth(url: string, options: any = {}) {
  const token = await AsyncStorage.getItem("accessToken");
  const headers = {
    ...(options.headers || {}),
    Authorization: token ? `Bearer ${token}` : undefined,
  };
  return fetch(url, { ...options, headers, cache: 'no-store' });
}
```
**Chức năng**:
- Định nghĩa tất cả API endpoints
- Cung cấp hàm `fetchWithAuth` để tự động thêm token vào header
- Quản lý authentication cho tất cả API calls

### 📁 Authentication Screens

#### `app/login.tsx`
**Chức năng chính**:
- Form đăng nhập với validation email/password
- Gọi API login và lưu token vào AsyncStorage
- Lấy thông tin profile người dùng
- Chuyển hướng đến dashboard nếu thành công

**Key Features**:
```typescript
const validateEmail = (email: string) => {
  const re = /\S+@\S+\.\S+/;
  return re.test(email);
};

const loginApi = async (email: string, password: string) => {
  const response = await fetchWithAuth(login, {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
  // Lưu token và chuyển hướng
};
```

#### `app/register.tsx`
**Chức năng**: Form đăng ký tài khoản mới với validation và gửi OTP.

#### `app/ForgotPassword.tsx`
**Chức năng**: Form quên mật khẩu, gửi email reset.

#### `app/reset-password.tsx`
**Chức năng**: Form đặt lại mật khẩu mới.

#### `app/VerifyOtpScreen.tsx`
**Chức năng**: Xác thực OTP khi đăng ký hoặc reset password.

### 📁 Main App Screens

#### `app/dashboard.tsx`
**Chức năng chính**:
- Hiển thị thống kê tổng quan (tổng đồ vật, vị trí, đồ vật gần đây)
- Danh sách đồ vật gần đây
- Bottom navigation với 4 tab chính
- Chức năng xóa đồ vật

**Key Features**:
```typescript
const loadDashboardData = useCallback(async () => {
  // 1. Lấy tất cả vị trí
  const locationsResponse = await fetchWithAuth(locationsGetAll);
  
  // 2. Lấy đồ vật từ mỗi vị trí
  const itemPromises = locations.map(loc => 
    fetchWithAuth(itemsByLocation(loc.id))
  );
  
  // 3. Tính toán thống kê
  const totalItemsCount = allItems.length;
  const recentItemsAdded = allItems.filter(item => 
    new Date(item.createdAt) > oneMonthAgo
  ).length;
}, []);
```

#### `app/add-item.tsx`
**Chức năng chính**:
- Form thêm đồ vật mới với validation
- Upload ảnh sử dụng Expo Image Picker
- Chọn vị trí từ danh sách có sẵn
- Gửi dữ liệu dưới dạng FormData

**Key Features**:
```typescript
const pickImage = async () => {
  let result = await ImagePicker.launchImageLibraryAsync({
    mediaTypes: ImagePicker.MediaTypeOptions.Images,
    allowsEditing: true,
    quality: 0.7,
  });
};

const handleSubmit = async () => {
  const formData = new FormData();
  formData.append("name", name);
  formData.append("description", description);
  formData.append("tags", JSON.stringify(tags));
  formData.append("locationId", selectedLocation?.id || "");
  formData.append("color", color);
  
  if (imageUri) {
    formData.append("imageFile", {
      uri: imageUri,
      name: "item-image.jpg",
      type: "image/jpeg",
    } as any);
  }
};
```

#### `app/Search Screen.tsx`
**Chức năng chính**:
- Tìm kiếm cơ bản theo keyword
- Tìm kiếm nâng cao (Premium) theo tag, màu sắc, thời gian
- Filter tags để chọn loại tìm kiếm
- Hiển thị kết quả dạng danh sách

**Key Features**:
```typescript
// Kiểm tra Premium user
const fetchSubscription = async () => {
  const response = await fetchWithAuth(subscriptionsGetMy);
  if (response.ok) {
    const data = await response.json();
    setIsPremiumUser(data[0]?.packageName === "Premium");
  }
};

// Tìm kiếm nâng cao
const executeSearch = async (keyword: string, filterId: string) => {
  let body: any = { tags: [], color: "", sortBy: "" };
  
  switch (filterId) {
    case "tag":
      body.tags.push(keyword.trim());
      break;
    case "color":
      body.color = keyword.trim();
      break;
    case "time":
      body.sortBy = "time";
      break;
  }
};
```

#### `app/LocationManager.tsx`
**Chức năng chính**:
- Hiển thị danh sách vị trí
- Thêm/xóa vị trí mới
- Hiển thị đồ vật theo từng vị trí
- Quản lý đồ vật trong vị trí

**Key Features**:
```typescript
const loadLocations = async () => {
  const response = await fetchWithAuth(locationsGetAll);
  const data: LocationType[] = await response.json();
  setLocations(data);
  
  if (data.length > 0) {
    setSelectedLocationId(data[0].id);
    fetchItemsForLocation(data[0].id);
  }
};

const handleAddLocation = async () => {
  const newLoc = {
    name: newLocationName.trim(),
    parentLocationId: null,
    description: newLocationDescription.trim(),
  };
  
  const response = await fetchWithAuth(locationsCreate, {
    method: "POST",
    body: JSON.stringify(newLoc),
  });
};
```

#### `app/product-detail.tsx`
**Chức năng**: Hiển thị chi tiết đồ vật với thông tin đầy đủ và chức năng chỉnh sửa.

#### `app/profile.tsx`
**Chức năng**: Quản lý thông tin cá nhân, cài đặt tài khoản.

#### `app/upgrade-package.tsx`
**Chức năng**: Nâng cấp gói Premium với các tính năng nâng cao.

### 📁 Components

#### `app/components/BottomNavigation.tsx`
**Chức năng**:
- Navigation bar ở dưới cùng với 4 tab chính
- Nút "Add" ở giữa để thêm đồ vật mới
- Hỗ trợ 2 variant: "home" và "default"

**Key Features**:
```typescript
const TabItem = ({ icon, label, active, onPress, variant }) => (
  <TouchableOpacity style={styles.tab} onPress={onPress}>
    <MaterialIcons
      name={icon}
      size={variant === "home" ? 28 : 24}
      color={active ? "#4b44f6" : "#888"}
    />
    <Text style={[styles.tabLabel, { color: active ? "#4b44f6" : "#888" }]}>
      {label}
    </Text>
  </TouchableOpacity>
);
```

#### `app/components/AppHeader.tsx`
**Chức năng**: Header tùy chỉnh với title, back button và avatar.

#### `app/components/Button.tsx`
**Chức năng**: Button component tái sử dụng với loading state và styling.

#### `app/components/InputField.tsx`
**Chức năng**: Input field với label, placeholder, error handling và validation.

#### `app/components/ItemCard.tsx`
**Chức năng**: Card hiển thị thông tin đồ vật với ảnh, tên, vị trí và actions.

#### `app/components/SearchInput.tsx`
**Chức năng**: Input tìm kiếm với icon và clear button.

#### `app/components/FilterTags.tsx`
**Chức năng**: Tags filter cho tìm kiếm nâng cao.

#### `app/components/SearchResultItem.tsx`
**Chức năng**: Item hiển thị kết quả tìm kiếm với actions.

#### `app/components/StatsCard.tsx`
**Chức năng**: Card hiển thị thống kê với icon và số liệu.

#### `app/components/Logo.tsx`
**Chức năng**: Component logo của ứng dụng.

#### `app/components/LinkText.tsx`
**Chức năng**: Text có thể click với styling link.

### 📁 API

#### `app/api/search.ts`
**Chức năng**: Các hàm helper cho tìm kiếm và xử lý kết quả.

## Luồng dữ liệu

### 1. Authentication Flow
```
User Input → Validation → API Call → Token Storage → Navigation
```

### 2. Data Fetching Flow
```
Component Mount → API Call → Loading State → Data Update → UI Render
```

### 3. CRUD Operations Flow
```
User Action → Validation → API Call → Success/Error → UI Update
```

## State Management

Ứng dụng sử dụng React Hooks để quản lý state:

- **Local State**: `useState` cho form data, loading states
- **Side Effects**: `useEffect` cho API calls, data fetching
- **Performance**: `useCallback` cho functions được pass qua props
- **Navigation**: `useFocusEffect` để refresh data khi màn hình được focus

## Error Handling

- **API Errors**: Try-catch blocks với user-friendly error messages
- **Validation**: Client-side validation với real-time feedback
- **Network Errors**: Graceful degradation với retry mechanisms
- **User Feedback**: Alert dialogs và loading indicators

## Security Features

- **Token-based Authentication**: JWT tokens stored in AsyncStorage
- **Automatic Token Refresh**: `fetchWithAuth` handles token management
- **Input Validation**: Client-side validation for all user inputs
- **Secure API Calls**: All API calls include authentication headers

## Performance Optimizations

- **Lazy Loading**: Components load only when needed
- **Image Optimization**: Compressed images with quality settings
- **Caching**: API responses cached appropriately
- **Memory Management**: Proper cleanup in useEffect hooks

## Testing Considerations

- **Component Testing**: Each component can be tested independently
- **API Mocking**: `fetchWithAuth` can be mocked for testing
- **Navigation Testing**: Expo Router provides testing utilities
- **State Testing**: Hooks can be tested with React Testing Library

## Deployment

- **Expo Build**: Configured for iOS, Android, and Web
- **Environment Variables**: API endpoints can be configured per environment
- **Asset Management**: Images and fonts properly bundled
- **App Store Ready**: Configured with proper app metadata

## Future Enhancements

- **Offline Support**: Implement offline-first architecture
- **Push Notifications**: Add notification system
- **Advanced Search**: Implement full-text search
- **Data Export**: Add export functionality for user data
- **Multi-language**: Internationalization support 