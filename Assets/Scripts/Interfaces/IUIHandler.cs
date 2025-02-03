public interface IUIHandler
{
    void ShowApiKeyPrompt();
    void SubmitApiKey();
    void SearchMovies();
    void BackToSearchScreen();
    void SetMovieAPI(IMovieAPI movieAPI);
}