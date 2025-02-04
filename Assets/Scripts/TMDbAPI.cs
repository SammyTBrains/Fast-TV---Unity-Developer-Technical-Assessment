using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles communication with The Movie Database (TMDb) API.
/// Implements the <see cref="IMovieAPI"/> interface for movie-related operations.
/// </summary>
public class TMDbAPI : MonoBehaviour, IMovieAPI
{
    /// <summary>
    /// Singleton instance of the TMDbAPI class.
    /// </summary>
    public static TMDbAPI Instance { get; private set; }

    private const string BaseUrl = "https://api.themoviedb.org/3";
    private string _apiKey;
    private const string _baseImageUrl = "https://image.tmdb.org/t/p/w500";
    private const string API_KEY_PREF = "TMDbAPIKey"; // PlayerPrefs key for storing API key

    private const string CachePrefix = "MovieSearchCache_";
    private const int CacheExpiryMinutes = 60;

    private IUIHandler uIHandler;

    /// <summary>
    /// Sets the UI handler for displaying API-related messages and prompts.
    /// </summary>
    /// <param name="uIHandler">The UI handler to set.</param>
    public void SetUIHandler(IUIHandler uIHandler)
    {
        this.uIHandler = uIHandler;
    }

    #region MonoBehaviour Methods
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

        LoadApiKey(); // Load the API key on startup
    }
    #endregion

    #region API Methods
    /// <summary>
    /// Sets the TMDb API key and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="apiKey">The API key to set.</param>
    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        PlayerPrefs.SetString(API_KEY_PREF, apiKey);
        PlayerPrefs.Save();
        Debug.Log("API Key saved.");
    }

    /// <summary>
    /// Loads the API key from PlayerPrefs. If no key is found, prompts the user to enter one.
    /// </summary>
    private void LoadApiKey()
    {
        if (PlayerPrefs.HasKey(API_KEY_PREF))
        {
            _apiKey = PlayerPrefs.GetString(API_KEY_PREF);
        }
        else
        {
            Debug.LogWarning("No API Key found. Prompting user...");
            uIHandler.ShowApiKeyPrompt();
        }
    }

    /// <summary>
    /// Searches for movies using the TMDb API.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="onSuccess">Callback invoked when the search is successful.</param>
    /// <param name="onError">Callback invoked when an error occurs.</param>
    /// <returns>An IEnumerator for coroutine support.</returns>
    public IEnumerator SearchMovies(string query, Action<List<MovieSearchResult>> onSuccess, Action<string> onError)
    {
        string cacheKey = GetCacheKey(query);

        if (TryGetCachedResponse(cacheKey, out string cachedResponse))
        {
            MovieSearchResponse response = JsonUtility.FromJson<MovieSearchResponse>(cachedResponse);
            onSuccess?.Invoke(response.results);
            yield break;
        }

        string url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={UnityWebRequest.EscapeURL(query)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                if (request.downloadHandler.text == null)
                {
                    onError?.Invoke("No results found.");
                    yield break;
                }

                string jsonResponse = request.downloadHandler.text;
                CacheResponse(cacheKey, jsonResponse);

                MovieSearchResponse response = JsonUtility.FromJson<MovieSearchResponse>(jsonResponse);
                onSuccess?.Invoke(response.results);
            }
        }
    }

    /// <summary>
    /// Downloads a movie poster image from the TMDb API.
    /// </summary>
    /// <param name="movie">The movie information containing the poster path.</param>
    /// <param name="callback">Callback invoked with the downloaded texture.</param>
    public void GetMovieImage(MovieImageReqInfo movie, Action<Texture2D> callback)
    {
        if (string.IsNullOrEmpty(movie.poster_path))
        {
            callback?.Invoke(null);
            return;
        }

        StartCoroutine(DownloadMovieImage(movie, callback));
    }

    /// <summary>
    /// Coroutine to download a movie poster image.
    /// </summary>
    /// <param name="movie">The movie information containing the poster path.</param>
    /// <param name="callback">Callback invoked with the downloaded texture.</param>
    /// <returns>An IEnumerator for coroutine support.</returns>
    private IEnumerator DownloadMovieImage(MovieImageReqInfo movie, Action<Texture2D> callback)
    {
        string imageUrl = $"{_baseImageUrl}{movie.poster_path}";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            callback?.Invoke(texture);
        }
        else
        {
            Debug.LogError($"Error loading image ({imageUrl}): {request.error}");
            callback?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieves detailed information about a movie from the TMDb API.
    /// </summary>
    /// <param name="movieId">The ID of the movie.</param>
    /// <param name="onSuccess">Callback invoked when the details are successfully retrieved.</param>
    /// <param name="onError">Callback invoked when an error occurs.</param>
    /// <returns>An IEnumerator for coroutine support.</returns>
    public IEnumerator GetMovieDetails(int movieId, Action<MovieDetails> onSuccess, Action<string> onError)
    {
        string url = $"{BaseUrl}/movie/{movieId}?api_key={_apiKey}";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            MovieDetails movieDetails = JsonUtility.FromJson<MovieDetails>(jsonResponse);
            onSuccess?.Invoke(movieDetails);
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Generates a cache key for a search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>The cache key.</returns>
    private string GetCacheKey(string query)
    {
        return $"{CachePrefix}{query}";
    }

    /// <summary>
    /// Caches an API response in PlayerPrefs.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="jsonResponse">The JSON response to cache.</param>
    private void CacheResponse(string key, string jsonResponse)
    {
        string timestamp = DateTime.UtcNow.ToString("O");
        PlayerPrefs.SetString(key, jsonResponse);
        PlayerPrefs.SetString($"{key}_timestamp", timestamp);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Attempts to retrieve a cached API response.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="jsonResponse">The cached JSON response, if found.</param>
    /// <returns>True if a valid cached response is found; otherwise, false.</returns>
    private bool TryGetCachedResponse(string key, out string jsonResponse)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string timestamp = PlayerPrefs.GetString($"{key}_timestamp");
            DateTime cachedTime = DateTime.Parse(timestamp);
            if ((DateTime.UtcNow - cachedTime).TotalMinutes < CacheExpiryMinutes)
            {
                jsonResponse = PlayerPrefs.GetString(key);
                return true;
            }
            else
            {
                // Remove expired cache entry
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.DeleteKey($"{key}_timestamp");
                PlayerPrefs.Save();
            }
        }

        jsonResponse = null;
        return false;
    }
    #endregion

    #region Data Models
    /// <summary>
    /// Represents a movie search result.
    /// </summary>
    [Serializable]
    public class MovieSearchResult
    {
        public int id;
        public string title;
        public string poster_path;
        public string overview;
        public string release_date;
        public float vote_average;
    }

    /// <summary>
    /// Represents a response containing a list of movie search results.
    /// </summary>
    [Serializable]
    public class MovieSearchResponse
    {
        public List<MovieSearchResult> results;
    }

    /// <summary>
    /// Represents detailed information about a movie.
    /// </summary>
    [Serializable]
    public class MovieDetails
    {
        public int id;
        public string title;
        public string poster_path;
        public string overview;
        public string release_date;
        public float vote_average;
        public List<Genre> genres;
        public List<CastMember> cast;
    }

    [Serializable]
    public class MovieImageReqInfo
    {
        public string title;
        public string poster_path;
    }

    /// <summary>
    /// Represents a movie genre.
    /// </summary>
    [Serializable]
    public class Genre
    {
        public int id;
        public string name;
    }

    /// <summary>
    /// Represents a cast member in a movie.
    /// </summary>
    [Serializable]
    public class CastMember
    {
        public string name;
        public string character;
    }
    #endregion
}