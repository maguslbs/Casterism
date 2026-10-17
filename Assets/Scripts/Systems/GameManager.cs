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
    [SerializeField] private RewardScreen rewardScreen;

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
        GameEvents.GameOver();

        bool isBoss = RunManager.Instance != null && RunManager.Instance.CurrentNodeTier == MapTier.Boss;
        winLoseDisplay.text = isBoss ? "RUN COMPLETE" : "VICTORY ACHIEVED";

        StartCoroutine(FinishBattle(true));
    }

    private void PlayerLose()
    {
        isGameActive = false;
        GameEvents.GameOver();
        winLoseDisplay.text = "YOU DEATH";
        StartCoroutine(FinishBattle(false));
    }

    private IEnumerator FinishBattle(bool playerWon)
    {
        yield return new WaitForSeconds(transitionTime);

        if (RunManager.Instance == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        if (playerWon)
        {
            bool isBoss = RunManager.Instance.CurrentNodeTier == MapTier.Boss;

            if (isBoss)
            {
                RunManager.Instance.EndRun();
            }
            else if (rewardScreen != null)
            {
                rewardScreen.Show(RunManager.Instance.CurrentNodeTier);
            }
            else
            {
                RunManager.Instance.CompleteCurrentNode();
            }
        }
        else
        {
            RunManager.Instance.EndRun();
        }
    }
}
