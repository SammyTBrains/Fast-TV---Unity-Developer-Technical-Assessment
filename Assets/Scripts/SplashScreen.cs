using System.Threading.Tasks; // Required for async/await
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float delayBeforeLoad = 3f; // Set delay time in seconds

    private async void Start()
    {
        await WaitAndLoadNextScene();
    }

    private async Task WaitAndLoadNextScene()
    {
        // Wait for the specified delay
        await Task.Delay((int)(delayBeforeLoad * 1000)); // Convert seconds to milliseconds

        // Load the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        // wait until the scene is fully loaded before continuing
        while (!asyncLoad.isDone)
        {
            await Task.Yield(); // Yield control back to the main thread until loading is done
        }
    }
}
