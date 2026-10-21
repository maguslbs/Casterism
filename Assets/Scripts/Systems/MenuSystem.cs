using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MapScene");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenDeckBuilder()
    {
        SceneManager.LoadScene("DeckBuilderScene");
    }

    public void BackFromDeckBuilder()
    {
        string targetScene = RunManager.Instance != null ? "MapScene" : "MainMenuScene";
        SceneManager.LoadScene(targetScene);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else   
                Application.Quit();
        #endif


    }
}
