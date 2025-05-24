
<h1 align="center">🏠 HomeTrack</h1>

<p align="center">
  <strong>Personal Item Location Management System</strong><br/>
  Web + Mobile + API
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Web%20%7C%20Mobile%20%7C%20API-green" alt="Platform: Web | Mobile | API" />
  <img src="https://img.shields.io/badge/React-19.x-blue" alt="React 19.x" />
  <img src="https://img.shields.io/badge/React%20Native-Expo-orange" alt="React Native (Expo)" />
  <img src="https://img.shields.io/badge/.NET-9.0-purple" alt=".NET 9.0" />
  <img src="https://img.shields.io/github/license/YOUR_GITHUB_USERNAME/HomeTrack" alt="License: MIT" />
  <!-- Add more badges as needed: build status, version, etc. -->
  <!-- e.g., <img src="https://img.shields.io/github/workflow/status/YOUR_GITHUB_USERNAME/HomeTrack/CI%20Pipeline" alt="Build Status" /> -->
</p>

---

## 🌟 About The Project

Ever spent precious minutes searching for your keys, wallet, or that specific book? **HomeTrack** aims to solve this common frustration by providing a seamless and intuitive system to manage and track the location of your personal belongings.

With dedicated web and mobile applications, all powered by a robust backend API, HomeTrack ensures your item data is always synchronized and accessible, wherever you are.

### ✨ Key Features

*   📦 **Item & Location Management**: Easily add, categorize, and update items and their designated storage locations.
*   📱 **Cross-Platform Access**: Manage your items from anywhere using our web application and mobile apps (iOS & Android via Expo).
*   🌐 **Cloud Synchronization**: Data is seamlessly synced across all your devices via a robust backend API.
*   🧭 **Quick Search & Filtering**: Find what you're looking for in seconds with powerful search and filtering capabilities.
*   📷 **Image Attachments**: Attach photos to items for easier identification.
*   🏷️ **Tagging System**: Use tags for flexible organization and searching.

---

## 🛠️ Tech Stack

This project is built using a modern and scalable technology stack:

*   **Frontend (Web):** [React](https://reactjs.org/) (v19.x)
*   **Frontend (Mobile):** [React Native](https://reactnative.dev/) with [Expo](https://expo.dev/)
*   **Backend API:** [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) (.NET 9.0)
*   **Database:** (Specify your database, e.g., PostgreSQL, SQL Server, SQLite)

---

## 📁 Project Structure

The repository is organized as follows:

```bash
HomeTrack/
├── HomeTrack.Api/        # Backend API - ASP.NET Core Solution
│   └── HomeTrack.Api.csproj
│   └── ... (other backend files)
├── hometrack-mobile/     # Mobile Application - React Native (Expo)
│   └── package.json
│   └── App.js
│   └── ... (other mobile app files)
├── hometrack-web/        # Web Application - ReactJS
│   └── package.json
│   └── public/
│   └── src/
│       └── App.js
│       └── ... (other web app files)
├── HomeTrack.sln         # Main Solution File for .NET Backend
└── README.md             # This file
└── LICENSE               # MIT License file
```
---

## 🚀 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

Ensure you have the following installed:
*   [.NET SDK 9.0 or later](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Node.js (LTS version recommended)](https://nodejs.org/)
*   [npm](https://www.npmjs.com/) or [Yarn](https://yarnpkg.com/)
*   [Expo CLI](https://docs.expo.dev/get-started/installation/) (for mobile development)
    ```bash
    npm install -g expo-cli
    ```
*   (Your chosen database, e.g., SQL Server Management Studio, pgAdmin)
*   A code editor like [VS Code](https://code.visualstudio.com/) or Visual Studio.

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/YOUR_GITHUB_USERNAME/HomeTrack.git
    cd HomeTrack
    ```

2.  **Backend API (HomeTrack.Api):**
    *   Navigate to the API directory:
        ```bash
        cd HomeTrack.Api
        ```
    *   Restore dependencies:
        ```bash
        dotnet restore
        ```
    *   Configure your database connection string in `appsettings.Development.json` (create it if it doesn't exist, copying from `appsettings.json`).
    *   Apply database migrations (if using Entity Framework Core):
        ```bash
        dotnet ef database update
        ```
    *   Run the API:
        ```bash
        dotnet run
        ```
    The API should now be running (typically on `https://localhost:7xxx` or `http://localhost:5xxx`).

3.  **Web Application (hometrack-web):**
    *   Navigate to the web app directory:
        ```bash
        cd ../hometrack-web  # From HomeTrack.Api or from root: cd hometrack-web
        ```
    *   Install dependencies:
        ```bash
        npm install
        # or
        # yarn install
        ```
    *   Configure the API endpoint in your web app's environment variables or configuration file (e.g., `.env` or a config service) to point to your running backend API.
    *   Start the development server:
        ```bash
        npm start
        # or
        # yarn start
        ```
    The web app should open in your browser, typically at `http://localhost:3000`.

4.  **Mobile Application (hometrack-mobile):**
    *   Navigate to the mobile app directory:
        ```bash
        cd ../hometrack-mobile # From hometrack-web or from root: cd hometrack-mobile
        ```
    *   Install dependencies:
        ```bash
        npm install
        # or
        # yarn install
        ```
    *   Configure the API endpoint in your mobile app's environment variables or configuration file to point to your running backend API.
    *   Start the Expo development server:
        ```bash
        npx expo start
        # or
        # yarn expo start
        ```
    Scan the QR code with the Expo Go app on your mobile device or run on an emulator/simulator.

---

## 📖 Usage

Once all components are set up and running:
1.  Access the web application via your browser (e.g., `http://localhost:3000`).
2.  Use the Expo Go app on your mobile device to run the mobile application.
3.  Register an account (if applicable) and start adding your items and their locations!

*(Add more specific usage instructions or link to user documentation if available.)*

---

## 🗺️ Roadmap

*   [ ] Feature: User Authentication & Authorization
*   [ ] Feature: Image uploads for items
*   [ ] Feature: Advanced search filters (by room, category, tags)
*   [ ] Feature: Sharing item locations with other users
*   [ ] Improvement: UI/UX enhancements
*   [ ] Tech: Add unit and integration tests

See the [open issues](https://github.com/Duy-Tien-Nguyen/Software_Technology/issues) for a full list of proposed features (and known issues).

---

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

Please read `CONTRIBUTING.md` (you might want to create this file) for details on our code of conduct, and the process for submitting pull requests to us.

---

## 📜 License

Distributed under the MIT License. See `LICENSE` file for more information.

---

## 📞 Contact

Nguyen Tien Duy / Organization - [@Duy Tien Nguyen](https://www.linkedin.com/in/duy-tien-nguyen/) - duynguyen.das@gmail.com

Project Link: [https://github.com/Duy-Tien-Nguyen/Software_Technology.git](https://github.com/Duy-Tien-Nguyen/Software_Technology.git)

---

## 🙏 Acknowledgements

*   [React](https://reactjs.org/)
*   [React Native](https://reactnative.dev/) & [Expo](https://expo.dev/)
*   [.NET](https://dotnet.microsoft.com/)
*   [Shields.io](https://shields.io/) for badges


```