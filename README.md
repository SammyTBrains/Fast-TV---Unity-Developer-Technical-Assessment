---
# **Movie Exploration App - Unity (Android)**

## **Overview**
This is a Unity-based Android application that allows users to explore movies by searching for titles and viewing detailed information, including posters, synopses, and metadata. The app integrates with **The Movie Database (TMDb) API** to fetch movie data and displays it in a user-friendly interface.
---

## **Features**

- **Search Functionality**: Users can search for movies by title.
- **Movie Details**: Displays detailed information about a selected movie, including posters, synopsis, release date, rating, genres, and cast.
- **Responsive Design**: The UI adapts to both portrait and landscape orientations.
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
   git clone https://github.com/your-username/MovieExplorationApp.git
   ```

2. **Open in Unity**:

   - Open the project in Unity (version 2020.3 or later).
   - Ensure the **Android Build Support** module is installed.

3. **Set Up TMDb API Key**:

   - Obtain an API key from [TMDb](https://www.themoviedb.org/settings/api).
   - Enter the API key in the app when prompted on first launch.

4. **Build and Run**:
   - Go to `File > Build Settings`.
   - Select **Android** as the platform and click `Switch Platform`.
   - Connect an Android device or use an emulator.
   - Click `Build and Run`.

---

## **Documentation**

- **[TMDbAPI Documentation](Documentation/TMDbAPI.md)**: Detailed documentation for the `TMDbAPI` class.
- **[UIManager Documentation](Documentation/UIManager.md)**: Detailed documentation for the `UIManager` class.
- **[GameManager Documentation](Documentation/GameManager.md)**: Detailed documentation for the `GameManager` class.

---

## **Demo**

[Watch a demo of the app here](https://www.youtube.com/demo-link) (replace with your actual demo link).

---

## **Known Issues and Limitations**

- **No Offline Mode**: The app requires an internet connection to fetch data.
- **Limited Error Handling**: Some edge cases (e.g., invalid API key) are not fully handled.
- **UI Scaling**: The UI may not scale perfectly on all devices.

---

## **Future Improvements**

- **Offline Mode**: Cache search results and movie details for offline access.
- **Enhanced UI**: Add animations and transitions for a smoother user experience.
- **Pagination**: Implement pagination for search results with many entries.
- **Localization**: Support multiple languages for a global audience.

---

## **Contributing**

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Submit a pull request with a detailed description of your changes.

---

## **License**

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## **Contact**

For questions or feedback, please contact:

- **Your Name**: your.email@example.com
- **GitHub**: [your-username](https://github.com/your-username)

---
