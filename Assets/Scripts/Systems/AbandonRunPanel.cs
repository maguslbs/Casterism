using UnityEngine;
using UnityEngine.SceneManagement;

public class AbandonRunPanel : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private void Start()
    {
        confirmPanel.SetActive(false);
    }

    public void Open()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive()) return;

        confirmPanel.SetActive(true);
    }

    public void Cancel()
    {
        confirmPanel.SetActive(false);
    }

    public void Confirm()
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.EndRun();
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}