using UnityEngine;

/// <summary>
/// Manages the initialization and setup of dependencies for the application.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager class.
    /// </summary>
    public static GameManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDependencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes and wires up dependencies between the TMDbAPI and UIManager.
    /// </summary>
    private void InitializeDependencies()
    {
        IMovieAPI movieAPI = TMDbAPI.Instance;

        IUIHandler uiManager = UIManager.Instance;

        uiManager.SetMovieAPI(movieAPI);
        movieAPI.SetUIHandler(uiManager);
    }
}