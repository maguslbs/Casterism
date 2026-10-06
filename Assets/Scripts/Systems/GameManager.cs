using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private bool isGameActive = true;

    [SerializeField] private float transitionTime;
    [SerializeField] private TextMeshProUGUI winLoseDisplay;
    public LevelController lvl1;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += PlayerLose;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= PlayerLose;
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void PlayerWin()
    {
        isGameActive = false;
        winLoseDisplay.text = "VICTORY ACHIEVED";
        StartCoroutine(RestartGame());
    }

    private void PlayerLose()
    {
        isGameActive = false;
        winLoseDisplay.text = "YOU DEATH";
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("GameScene");
    }
}
