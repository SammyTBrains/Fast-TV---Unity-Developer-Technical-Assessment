using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class TMDbAPITests
{
    [Test]
    public void ParseMovieSearchResponse_ValidJson_ReturnsCorrectResults()
    {
        // Arrange
        string jsonResponse = @"
        {
            ""results"": [
                {
                    ""id"": 123,
                    ""title"": ""Test Movie"",
                    ""poster_path"": ""/test.jpg"",
                    ""overview"": ""Test overview"",
                    ""release_date"": ""2023-01-01"",
                    ""vote_average"": 7.5
                }
            ]
        }";

        // Act
        MovieSearchResponse response = JsonUtility.FromJson<MovieSearchResponse>(jsonResponse);

        // Assert
        Assert.AreEqual(1, response.results.Count);
        Assert.AreEqual("Test Movie", response.results[0].title);
        Assert.AreEqual("/test.jpg", response.results[0].poster_path);
    }

    [Test]
    public void ParseMovieDetailsResponse_ValidJson_ReturnsCorrectDetails()
    {
        // Arrange
        string jsonResponse = @"
        {
            ""id"": 123,
            ""title"": ""Test Movie"",
            ""poster_path"": ""/test.jpg"",
            ""overview"": ""Test overview"",
            ""release_date"": ""2023-01-01"",
            ""vote_average"": 7.5,
            ""genres"": [
                { ""id"": 1, ""name"": ""Action"" }
            ],
            ""cast"": [
                { ""name"": ""Actor 1"", ""character"": ""Character 1"" }
            ]
        }";

        // Act
        MovieDetails details = JsonUtility.FromJson<MovieDetails>(jsonResponse);

        // Assert
        Assert.AreEqual("Test Movie", details.title);
        Assert.AreEqual(1, details.genres.Count);
        Assert.AreEqual("Action", details.genres[0].name);
        Assert.AreEqual(1, details.cast.Count);
        Assert.AreEqual("Actor 1", details.cast[0].name);
    }

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