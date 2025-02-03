using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class TMDbAPI : MonoBehaviour
{
    public static TMDbAPI Instance { get; private set; }

    private const string BaseUrl = "https://api.themoviedb.org/3";
    private string _apiKey;
    private const string _baseImageUrl = "https://image.tmdb.org/t/p/w500";

    private const string CachePrefix = "MovieSearchCache_";
    private const int CacheExpiryMinutes = 60;

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
    }
    #endregion

    #region API Methods
    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

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

    public void GetMovieImage(MovieSearchResult movie, Action<Texture2D> callback)
    {
        if (string.IsNullOrEmpty(movie.poster_path))
        {
            callback?.Invoke(null);
            return;
        }

        StartCoroutine(DownloadMovieImage(movie, callback));
    }

    private IEnumerator DownloadMovieImage(MovieSearchResult movie, Action<Texture2D> callback)
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
    private string GetCacheKey(string query)
    {
        return $"{CachePrefix}{query}";
    }

    private void CacheResponse(string key, string jsonResponse)
    {
        string timestamp = DateTime.UtcNow.ToString("O");
        PlayerPrefs.SetString(key, jsonResponse);
        PlayerPrefs.SetString($"{key}_timestamp", timestamp);
        PlayerPrefs.Save();
    }

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

    [Serializable]
    public class MovieSearchResponse
    {
        public List<MovieSearchResult> results;
    }

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
    public class Genre
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class CastMember
    {
        public string name;
        public string character;
    }
    #endregion
}