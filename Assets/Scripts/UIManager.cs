using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMDbAPI;//Use defined data models

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_InputField searchInputField;

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

    //TEST CACHING!!!!!
    //DELETE NETWORK MANAGER
    //PROMPT USERS TO INPUT THEIR OWN API KEY ON FIRST LAUNCH - CHECK DOC TO CONFIRM

    void Start()
    {
        // GET THE SET API KEY BY USER - CHECK DOC TO CONFIRM
        TMDbAPI.Instance.SetApiKey("5ef6b2bf288ba18b58b8deb349913ff0");
    }

    public void SearchMovies()
    {
        StartCoroutine(TMDbAPI.Instance.SearchMovies(searchInputField.text, OnSearchSuccess, OnError));
    }

    public void GetMovieDetails(int movieId)
    {
        StartCoroutine(TMDbAPI.Instance.GetMovieDetails(movieId, OnMovieDetailsSuccess, OnError));
    }

    private void OnSearchSuccess(List<MovieSearchResult> results)
    {
        ClearSearchResults();

        // Display new search results in the UI
        foreach (var movie in results)
        {
            Debug.Log($"Found: {movie.title}");
            // Add logic to display movie posters and titles in the UI
        }
    }

    private void OnMovieDetailsSuccess(MovieDetails movieDetails)
    {
        // Display movie details in the UI
        Debug.Log($"Movie Details: {movieDetails.title}");
        Debug.Log($"Synopsis: {movieDetails.overview}");
        Debug.Log($"Release Date: {movieDetails.release_date}");
        Debug.Log($"Rating: {movieDetails.vote_average}");

        // Add logic to display movie details in the UI
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

    private void ClearSearchResults()
    {
        // Add logic to clear the search results UI (e.g., remove old movie posters and titles)
        Debug.Log("Clearing search results.");
    }
}