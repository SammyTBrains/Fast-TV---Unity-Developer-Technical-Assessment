using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMDbAPI;
using UnityEngine.UI;//Use defined data models

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_InputField _searchInputField;

    [SerializeField] private GameObject _movieContainerPrefab;
    [SerializeField] private Transform _scrollViewContent;

    [SerializeField] private GameObject _searchScreen;
    [SerializeField] private GameObject _movieDetailsScreen;

    [SerializeField] private Transform _movieDetailsScreenContent;
    [SerializeField] private GameObject _genrePrefab, _genreContainer;
    [SerializeField] private GameObject _castPrefab, _castContainer;

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

    void Start()
    {
        // GET THE SET API KEY BY USER - CHECK DOC TO CONFIRM
        TMDbAPI.Instance.SetApiKey("5ef6b2bf288ba18b58b8deb349913ff0");
    }

    public void SearchMovies()
    {
        StartCoroutine(TMDbAPI.Instance.SearchMovies(_searchInputField.text, OnSearchSuccess, OnError));
    }

    private void GetMovieDetails(int movieId)
    {
        StartCoroutine(TMDbAPI.Instance.GetMovieDetails(movieId, OnMovieDetailsSuccess, OnError));
    }

    public void BackToSearchScreen()
    {
        _movieDetailsScreen.SetActive(false);
        _searchScreen.SetActive(true);
    }

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

            // Get the Button component
            Button movieButton = newMovie.GetComponent<Button>();
            if (movieButton != null)
            {
                int movieId = movie.id; // Store movieId in a local variable to avoid closure issues
                movieButton.onClick.AddListener(() => GetMovieDetails(movieId));
            }
        }
    }

    private void OnMovieDetailsSuccess(MovieDetails movieDetails)
    {
        _searchScreen.SetActive(false);
        _movieDetailsScreen.SetActive(true);

        MovieImageReqInfo info = new MovieImageReqInfo();
        info.title = movieDetails.title;
        info.poster_path = movieDetails.poster_path;

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

    private void ClearChildren(Transform parentContainer)
    {
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetMovieImage(MovieImageReqInfo movie, Image image)
    {
        TMDbAPI.Instance.GetMovieImage(movie, (Texture2D texture) =>
        {
            if (texture != null)
            {
                image.sprite = SpriteFromTexture(texture);
            }
        });
    }

    Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}