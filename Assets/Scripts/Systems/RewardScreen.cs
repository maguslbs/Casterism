using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : MonoBehaviour
{
    [System.Serializable]
    private class RewardOption
    {
        public Button button;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
    }

    [SerializeField] private GameObject panel;
    [SerializeField] private RewardOption hpOption;
    [SerializeField] private RewardOption actionOption;
    [SerializeField] private RewardOption cardOption;

    [Header("HP Reward per Tier")]
    [SerializeField] private int easyHpReward = 50;
    [SerializeField] private int normalHpReward = 70;
    [SerializeField] private int eliteHpReward = 100;

    private int hpReward;

    private void Awake()
    {
        panel.SetActive(false);

        if (hpOption.button != null) hpOption.button.onClick.AddListener(ChooseHp);
        if (actionOption.button != null) actionOption.button.onClick.AddListener(ChooseAction);
    }

    public void Show(MapTier tier)
    {
        hpReward = GetHpReward(tier);

        SetTexts(hpOption, "Vitality", $"+{hpReward} Max HP");
        SetTexts(actionOption, "Focus", "+1 Action");

        SetOptionVisible(actionOption, TurnSystem.Instance.CanGainAction());
        SetOptionVisible(cardOption, false);   // hadiah kartu belum dibuat

        panel.SetActive(true);
    }

    private int GetHpReward(MapTier tier)
    {
        switch (tier)
        {
            case MapTier.Normal: return normalHpReward;
            case MapTier.Elite: return eliteHpReward;
            default: return easyHpReward;
        }
    }

    private void SetTexts(RewardOption option, string title, string description)
    {
        if (option.titleText != null) option.titleText.text = title;
        if (option.descriptionText != null) option.descriptionText.text = description;
    }

    private void SetOptionVisible(RewardOption option, bool visible)
    {
        if (option.button != null) option.button.gameObject.SetActive(visible);
    }

    private void ChooseHp()
    {
        RunManager.Instance.AddMaxHealth(hpReward);
        Finish();
    }

    private void ChooseAction()
    {
        RunManager.Instance.AddBonusAction();
        Finish();
    }

    private void Finish()
    {
        panel.SetActive(false);
        RunManager.Instance.CompleteCurrentNode();
    }
}