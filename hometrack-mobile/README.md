# HomeTrack Mobile App

## Cấu trúc thư mục mới

```
src/
├── screens/
│   ├── auth/
│   │   ├── login.tsx
│   │   ├── register.tsx
│   │   ├── ForgotPassword.tsx
│   │   ├── reset-password.tsx
│   │   └── VerifyOtpScreen.tsx
│   ├── main/
│   │   ├── dashboard.tsx
│   │   └── profile.tsx
│   ├── product/
│   │   ├── product-detail.tsx
│   │   └── add-item.tsx
│   └── search/
│       └── Search Screen.tsx
├── components/
│   ├── common/
│   │   ├── Button.tsx
│   │   ├── InputField.tsx
│   │   └── LinkText.tsx
│   ├── layout/
│   │   ├── AppHeader.tsx
│   │   └── BottomNavigation.tsx
│   ├── product/
│   │   ├── ItemCard.tsx
│   │   ├── SearchResultItem.tsx
│   │   └── StatsCard.tsx
│   └── search/
│       ├── SearchInput.tsx
│       └── FilterTags.tsx
├── services/
│   ├── api.ts
│   └── location.ts
├── hooks/
├── utils/
├── constants/
├── types/
└── styles/
```

## Hướng dẫn di chuyển file

1. Tạo cấu trúc thư mục mới:
```bash
mkdir -p src/screens/{auth,main,product,search}
mkdir -p src/components/{common,layout,product,search}
mkdir -p src/{services,hooks,utils,constants,types,styles}
```

2. Di chuyển các file màn hình:
```bash
# Auth screens
mv app/login.tsx src/screens/auth/
mv app/register.tsx src/screens/auth/
mv app/ForgotPassword.tsx src/screens/auth/
mv app/reset-password.tsx src/screens/auth/
mv app/VerifyOtpScreen.tsx src/screens/auth/

# Main screens
mv app/dashboard.tsx src/screens/main/
mv app/profile.tsx src/screens/main/

# Product screens
mv app/product-detail.tsx src/screens/product/
mv app/add-item.tsx src/screens/product/

# Search screens
mv "app/Search Screen.tsx" src/screens/search/
```

3. Di chuyển các components:
```bash
# Common components
mv app/components/Button.tsx src/components/common/
mv app/components/InputField.tsx src/components/common/
mv app/components/LinkText.tsx src/components/common/

# Layout components
mv app/components/AppHeader.tsx src/components/layout/
mv app/components/BottomNavigation.tsx src/components/layout/

# Product components
mv app/components/ItemCard.tsx src/components/product/
mv app/components/SearchResultItem.tsx src/components/product/
mv app/components/StatsCard.tsx src/components/product/

# Search components
mv app/components/SearchInput.tsx src/components/search/
mv app/components/FilterTags.tsx src/components/search/
```

4. Di chuyển các service files:
```bash
mv app/api.tsx src/services/api.ts
mv app/LocationManager.tsx src/services/location.ts
```

5. Di chuyển các utility files:
```bash
mv app/utils/* src/utils/
```

## Cập nhật import paths

Sau khi di chuyển các file, cần cập nhật các import paths trong code để sử dụng các alias path mới đã được định nghĩa trong tsconfig.json:

```typescript
// Thay vì
import { Button } from '../../components/Button';

// Sử dụng
import { Button } from '@components/common/Button';

// Thay vì
import { login } from '../../api';

// Sử dụng
import { login } from '@services/api';
```

Các alias path đã được định nghĩa trong tsconfig.json:
- `@/*`: `src/*`
- `@components/*`: `src/components/*`
- `@screens/*`: `src/screens/*`
- `@services/*`: `src/services/*`
- `@utils/*`: `src/utils/*`
- `@hooks/*`: `src/hooks/*`
- `@constants/*`: `src/constants/*`
- `@types/*`: `src/types/*`
- `@styles/*`: `src/styles/*`

# Welcome to your Expo app 👋

This is an [Expo](https://expo.dev) project created with [`create-expo-app`](https://www.npmjs.com/package/create-expo-app).

## Get started

1. Install dependencies

   ```bash
   npm install
   ```

2. Start the app

   ```bash
   npx expo start
   ```

In the output, you'll find options to open the app in a

- [development build](https://docs.expo.dev/develop/development-builds/introduction/)
- [Android emulator](https://docs.expo.dev/workflow/android-studio-emulator/)
- [iOS simulator](https://docs.expo.dev/workflow/ios-simulator/)
- [Expo Go](https://expo.dev/go), a limited sandbox for trying out app development with Expo

You can start developing by editing the files inside the **app** directory. This project uses [file-based routing](https://docs.expo.dev/router/introduction).

## Get a fresh project

When you're ready, run:

```bash
npm run reset-project
```

This command will move the starter code to the **app-example** directory and create a blank **app** directory where you can start developing.

## Learn more

To learn more about developing your project with Expo, look at the following resources:

- [Expo documentation](https://docs.expo.dev/): Learn fundamentals, or go into advanced topics with our [guides](https://docs.expo.dev/guides).
- [Learn Expo tutorial](https://docs.expo.dev/tutorial/introduction/): Follow a step-by-step tutorial where you'll create a project that runs on Android, iOS, and the web.

## Join the community

Join our community of developers creating universal apps.

- [Expo on GitHub](https://github.com/expo/expo): View our open source platform and contribute.
- [Discord community](https://chat.expo.dev): Chat with Expo users and ask questions.
