using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    private void InitializeDependencies()
    {
        IMovieAPI movieAPI = TMDbAPI.Instance;

        IUIHandler uiManager = UIManager.Instance;

        uiManager.SetMovieAPI(movieAPI);
        movieAPI.SetUIHandler(uiManager);
    }
}