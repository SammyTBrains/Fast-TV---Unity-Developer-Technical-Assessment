using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMDbAPI;
using UnityEngine.UI;

/// <summary>
/// Manages the user interface for the movie exploration application.
/// Implements the <see cref="IUIHandler"/> interface for UI-related operations.
/// </summary>
public class UIManager : MonoBehaviour, IUIHandler
{
    /// <summary>
    /// Singleton instance of the UIManager class.
    /// </summary>
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _apiKeyPromptPanel;
    [SerializeField] private TMP_InputField _apiKeyInputField;

    [SerializeField] private TMP_InputField _searchInputField;

    [SerializeField] private GameObject _movieContainerPrefab;
    [SerializeField] private Transform _scrollViewContent;

    [SerializeField] private GameObject _searchScreen;
    [SerializeField] private GameObject _movieDetailsScreen;

    [SerializeField] private Transform _movieDetailsScreenContent;
    [SerializeField] private GameObject _genrePrefab, _genreContainer;
    [SerializeField] private GameObject _castPrefab, _castContainer;

    [SerializeField] private AudioClip _uiClickClip;
    [SerializeField] private GameObject _spinner;
    [SerializeField] private GameObject _searchTextIns, _noResultsFoundText;

    private IMovieAPI movieAPI;

    /// <summary>
    /// Sets the movie API instance for making API calls.
    /// </summary>
    /// <param name="movieAPI">The movie API instance to set.</param>
    public void SetMovieAPI(IMovieAPI movieAPI)
    {
        this.movieAPI = movieAPI;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        switch (movieAPI.LoadingData)
        {
            case true:
                _searchTextIns.SetActive(false);//Only needs to be set inactive once, at start
                _spinner.SetActive(true); 
                break;
            case false: 
                _spinner.SetActive(false); 
                break;
        }
    }

    /// <summary>
    /// Displays the API key prompt panel.
    /// </summary>
    public void ShowApiKeyPrompt()
    {
        _searchScreen.SetActive(false);
        _apiKeyPromptPanel.SetActive(true);
    }

    /// <summary>
    /// Submits the API key entered by the user.
    /// </summary>
    public void SubmitApiKey()
    {
        string apiKey = _apiKeyInputField.text.Trim();

        if (!string.IsNullOrEmpty(apiKey))
        {
            movieAPI.SetApiKey(apiKey);
            _apiKeyPromptPanel.SetActive(false);
            _searchScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("API Key cannot be empty!");
        }
    }

    /// <summary>
    /// Initiates a movie search using the entered query.
    /// </summary>
    public void SearchMovies()
    {
        StartCoroutine(movieAPI.SearchMovies(_searchInputField.text, OnSearchSuccess, OnError));
    }

    /// <summary>
    /// Retrieves detailed information about a movie.
    /// </summary>
    /// <param name="movieId">The ID of the movie.</param>
    private void GetMovieDetails(int movieId)
    {
        PlayButtonClick(_uiClickClip);
        StartCoroutine(movieAPI.GetMovieDetails(movieId, OnMovieDetailsSuccess, OnError));
    }

    /// <summary>
    /// Returns to the search screen from the movie details screen.
    /// </summary>
    public void BackToSearchScreen()
    {
        _movieDetailsScreen.SetActive(false);
        _searchScreen.SetActive(true);
    }

    /// <summary>
    /// Plays Audio clip at camera's postion.
    /// </summary>
    public void PlayButtonClick(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
    }

    /// <summary>
    /// Handles successful movie search results.
    /// </summary>
    /// <param name="results">The list of movie search results.</param>
    private void OnSearchSuccess(List<MovieSearchResult> results)
    {
        ClearChildren(_scrollViewContent);

        foreach (MovieSearchResult movie in results)
        {
            GameObject newMovie = Instantiate(_movieContainerPrefab, _scrollViewContent);

            MovieImageReqInfo info = new MovieImageReqInfo();
            info.title = movie.title;
            info.poster_path = movie.poster_path;

            SetMovieImage(info, newMovie.transform.Find("Image").GetComponent<Image>());
            newMovie.transform.Find("Title").GetComponent<TMP_Text>().text = movie.title;
            newMovie.transform.Find("Release Date").GetComponent<TMP_Text>().text = movie.release_date;
            newMovie.transform.Find("Overview").GetComponent<TMP_Text>().text = movie.overview;

            Button movieButton = newMovie.GetComponent<Button>();
            if (movieButton != null)
            {
                int movieId = movie.id; // Store movieId in a local variable to avoid closure issues
                movieButton.onClick.AddListener(() => GetMovieDetails(movieId));
            }
        }
    }

    /// <summary>
    /// Handles successful retrieval of movie details.
    /// </summary>
    /// <param name="movieDetails">The detailed information about the movie.</param>
    private void OnMovieDetailsSuccess(MovieDetails movieDetails)
    {
        _searchScreen.SetActive(false);
        _movieDetailsScreen.SetActive(true);

        MovieImageReqInfo info = new MovieImageReqInfo();
        info.title = movieDetails.title;
        info.poster_path = movieDetails.poster_path;

        _movieDetailsScreenContent.Find("Poster").GetComponent<Image>().sprite = null;//Clear previous Image
        SetMovieImage(info, _movieDetailsScreenContent.Find("Poster").GetComponent<Image>());
        _movieDetailsScreenContent.Find("Title").GetComponent<TMP_Text>().text = movieDetails.title;
        _movieDetailsScreenContent.Find("Release Date").GetComponent<TMP_Text>().text = movieDetails.release_date;
        _movieDetailsScreenContent.Find("Synopsis").GetComponent<TMP_Text>().text = movieDetails.overview;
        _movieDetailsScreenContent.Find("Vote Average").GetComponent<TMP_Text>().text = movieDetails.vote_average.ToString("F2");


        ClearChildren(_genreContainer.transform);
        foreach (Genre genre in movieDetails.genres)
        {
            GameObject newGenre = Instantiate(_genrePrefab, _genreContainer.transform);

            newGenre.GetComponent<TextMeshProUGUI>().text = genre.name;
        }

        ClearChildren(_castContainer.transform);
        foreach (CastMember cast in movieDetails.cast)
        {
            GameObject castMember = Instantiate(_castPrefab, _castContainer.transform);
            castMember.GetComponent<TextMeshProUGUI>().text = cast.name;
        }
    }

    /// <summary>
    /// Handles errors during API calls.
    /// </summary>
    /// <param name="error">The error message.</param>
    private void OnError(string error)
    {
        if (error == "No results found.")
        {
            // Handle no results found in UI
            Debug.Log("No results found.");
            // Example: Show a "No results found" message in the UI
            return;
        }

        Debug.LogError($"Error: {error}");
        // Example: Show an error message in the UI
    }

    /// <summary>
    /// Clears all child objects of a parent container.
    /// </summary>
    /// <param name="parentContainer">The parent container.</param>
    private void ClearChildren(Transform parentContainer)
    {
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Sets the movie poster image in the UI.
    /// </summary>
    /// <param name="movie">The movie information containing the poster path.</param>
    /// <param name="image">The UI Image component to set.</param>
    private void SetMovieImage(MovieImageReqInfo movie, Image image)
    {
        movieAPI.GetMovieImage(movie, (Texture2D texture) =>
        {
            if (texture != null)
            {
                image.sprite = SpriteFromTexture(texture);
            }
        });
    }

    /// <summary>
    /// Converts a Texture2D to a Sprite.
    /// </summary>
    /// <param name="texture">The Texture2D to convert.</param>
    /// <returns>The generated Sprite.</returns>
    Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}