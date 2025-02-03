using static TMDbAPI;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public interface IMovieAPI
{
    void SetApiKey(string apiKey);
    IEnumerator SearchMovies(string query, Action<List<MovieSearchResult>> onSuccess, Action<string> onError);
    IEnumerator GetMovieDetails(int movieId, Action<MovieDetails> onSuccess, Action<string> onError);
    void GetMovieImage(MovieImageReqInfo movie, Action<Texture2D> callback);
    void SetUIHandler(IUIHandler uiManager);
}