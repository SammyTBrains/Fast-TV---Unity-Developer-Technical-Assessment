---
# **Movie Exploration App - Unity (Android)**

## **Overview**
This is a Unity-based Android application that allows users to explore movies by searching for titles and viewing detailed information, including posters, synopses, and metadata. The app integrates with **The Movie Database (TMDb) API** to fetch movie data and displays it in a user-friendly interface.
---

## **Features**

- **Search Functionality**: Users can search for movies by title.
- **Movie Details**: Displays detailed information about a selected movie, including posters, synopsis, release date, rating, genres, and cast.
- **Caching**: API responses are cached to improve performance and reduce redundant API calls.
- **Error Handling**: Proper error handling for API calls and user input.

---

## **Architecture Overview**

The application follows a **modular architecture** with clear separation of concerns. Here’s a high-level breakdown:

### **1. TMDbAPI**

- **Responsibility**: Handles all communication with the TMDb API.
- **Key Features**:
  - Implements the `IMovieAPI` interface.
  - Manages API key storage and retrieval using `PlayerPrefs`.
  - Caches API responses to improve performance.
  - Downloads movie poster images.

### **2. UIManager**

- **Responsibility**: Manages the user interface and user interactions.
- **Key Features**:
  - Implements the `IUIHandler` interface.
  - Displays search results and movie details.
  - Handles UI transitions and updates.
  - Provides error feedback to the user.

### **3. GameManager**

- **Responsibility**: Initializes and wires up dependencies between `TMDbAPI` and `UIManager`.
- **Key Features**:
  - Ensures singleton instances of `TMDbAPI` and `UIManager` are properly initialized.
  - Injects dependencies to decouple components.

### **4. Interfaces**

- **IMovieAPI**: Defines the contract for interacting with the TMDb API.
- **IUIHandler**: Defines the contract for UI-related operations.

---

## **Folder Structure**

```
MovieExplorationApp/
├── Assets/
│   ├── Scripts/               # All C# scripts
│   │   ├── TMDbAPI.cs         # API communication and data models
│   │   ├── UIManager.cs       # UI management
│   │   ├── GameManager.cs     # Dependency initialization
│   │   ├── Interfaces/        # IMovieAPI and IUIHandler
│   │   └── Tests/             # Unit tests
│   ├── Scenes/                # Unity scenes
│   ├── Resources/             # Assets like images, fonts, etc.
│   └── Plugins/               # Third-party libraries
├── ProjectSettings/           # Unity project settings
├── README.md                  # Main documentation
└── Builds/                    # APK builds (optional)
```

---

## **Setup Instructions**

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/SammyTBrains/Fast-TV---Unity-Developer-Technical-Assessment.git
   ```

2. **Open in Unity**:

   - Open the project in Unity (version 6000.0.23 or later).
   - Ensure the **Android Build Support** module is installed.

3. **Set Up TMDb API Key**:

   - **IMPORTANT**: Obtain an API key from [TMDb](https://www.themoviedb.org/settings/api).
   - Enter the API key in the app when prompted on first launch.
   - **WARNING**: If the API key is incorrect or missing, movies will not be fetched. Ensure the API key is valid and properly entered.

4. **Build and Run**:
   - Go to `File > Build Settings`.
   - Select **Android** as the platform and click `Switch Platform`.
   - Connect an Android device (preferrably) or use an emulator.
   - Click `Build and Run`.

---

## **APK Download**

Download the latest APK from the [Releases](https://github.com/SammyTBrains/Fast-TV---Unity-Developer-Technical-Assessment/releases) page.

---

## **Demo**

[Watch a demo of the app here](https://drive.google.com/file/d/1u3G0KTE4RSxx32-aEK0yQyLLDSEt7Ehl/view?usp=sharing).

---

## **Design Decisions and Trade-offs**

1. **Singleton Pattern**:

   - Used for `TMDbAPI`, `UIManager`, and `GameManager` to ensure a single instance of each class.
   - Trade-off: Global state can make testing and debugging more challenging.

2. **Caching**:

   - API responses are cached using `PlayerPrefs` to reduce redundant API calls.
   - Trade-off: Cached data may become stale if not properly invalidated.

3. **Dependency Injection**:

   - `GameManager` injects dependencies between `TMDbAPI` and `UIManager` to decouple components.
   - Trade-off: Adds some complexity to the initialization process.

4. **Error Handling**:
   - Basic error handling is implemented for API calls and user input.
   - Trade-off: Limited handling of edge cases (e.g., network timeouts).

---

## **Known Issues and Limitations**

- **UI Scaling**: The UI may not scale perfectly on all devices.

---

## **Possible Improvements**

- **Enhanced UI**: Even more animations and clean look for a smoother user experience.
- **Responsivity**: Much better responsivity for portrait mode
- **Pagination**: Implement pagination for search results with many entries.
- **Localization**: Support multiple languages for a global audience.

---

## **Contributing**

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Submit a pull request with a detailed description of your changes.

---

## **Contact**

For questions or feedback, please contact:

- **Your Name**: josephsamuel034@gmail.com
- **GitHub**: [your-username](https://github.com/SammyTBrains)

---

### **Important Note**

**MAKE SURE YOUR TMDb API KEY IS CORRECT. IF THE API KEY IS INVALID OR MISSING, MOVIES WILL NOT BE FETCHED.**

---
