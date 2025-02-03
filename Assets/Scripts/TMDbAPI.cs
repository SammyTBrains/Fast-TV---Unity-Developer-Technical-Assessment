using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TMDbAPI : MonoBehaviour
{
    public static TMDbAPI Instance { get; private set; }

    private const string BaseUrl = "https://api.themoviedb.org/3";
    private string _apiKey;

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

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public IEnumerator SearchMovies(string query, System.Action<List<MovieSearchResult>> onSuccess, System.Action<string> onError)
    {
        string url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={UnityWebRequest.EscapeURL(query)}";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            if (request.downloadHandler.text == null )
            {
                onError?.Invoke("No results found.");
            }

            string jsonResponse = request.downloadHandler.text;
            MovieSearchResponse response = JsonUtility.FromJson<MovieSearchResponse>(jsonResponse);
            onSuccess?.Invoke(response.results);
        }
    }

    public IEnumerator GetMovieDetails(int movieId, System.Action<MovieDetails> onSuccess, System.Action<string> onError)
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

    [System.Serializable]
    public class MovieSearchResult
    {
        public int id;
        public string title;
        public string poster_path;
        public string overview;
        public string release_date;
        public float vote_average;
    }

    [System.Serializable]
    public class MovieSearchResponse
    {
        public List<MovieSearchResult> results;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Genre
    {
        public int id;
        public string name;
    }

    [System.Serializable]
    public class CastMember
    {
        public string name;
        public string character;
    }
}