using System.Collections;
using TMPro;
using UnityEngine;

public class MessageDisplay : Singleton<MessageDisplay>
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine currentRoutine;

    protected override void Awake()
    {
        base.Awake();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public static void Show(string message)
    {
        if (Instance == null)
        {
            Debug.Log(message);
            return;
        }

        Instance.ShowMessage(message);
    }

    private void ShowMessage(string message)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        messageText.text = message;
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(showDuration);

        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - timeElapsed / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        currentRoutine = null;
    }
}