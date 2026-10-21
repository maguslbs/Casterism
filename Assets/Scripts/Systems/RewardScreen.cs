using System.Collections.Generic;
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
        public Image icon;
    }

    [SerializeField] private GameObject panel;
    [SerializeField] private RewardOption hpOption;
    [SerializeField] private RewardOption actionOption;
    [SerializeField] private RewardOption cardOption;

    [Header("HP Reward per Tier")]
    [SerializeField] private int easyHpReward = 50;
    [SerializeField] private int normalHpReward = 70;
    [SerializeField] private int eliteHpReward = 100;

    [Header("Card Reward")]
    [SerializeField] private List<CardData> cardRewardPool = new List<CardData>();

    private int hpReward;
    private CardData offeredCard;

    private void Awake()
    {
        panel.SetActive(false);

        if (hpOption.button != null) hpOption.button.onClick.AddListener(ChooseHp);
        if (actionOption.button != null) actionOption.button.onClick.AddListener(ChooseAction);
        if (cardOption.button != null) cardOption.button.onClick.AddListener(ChooseCard);
    }

    public void Show(MapTier tier)
    {
        hpReward = GetHpReward(tier);
        SetTexts(hpOption, "Vitality", $"+{hpReward} Max HP");
        SetTexts(actionOption, "Focus", "+1 Action");
        SetOptionVisible(actionOption, TurnSystem.Instance.CanGainAction());

        offeredCard = PickCardReward();
        if (offeredCard != null)
        {
            SetTexts(cardOption, offeredCard.cardName, offeredCard.description);
            if (cardOption.icon != null) cardOption.icon.sprite = offeredCard.illustration;
        }
        SetOptionVisible(cardOption, offeredCard != null);

        panel.SetActive(true);
    }

    private CardData PickCardReward()
    {
        List<CardData> candidates = new List<CardData>();

        foreach (CardData card in cardRewardPool)
        {
            if (card == null) continue;
            if (DeckManager.Instance != null && DeckManager.Instance.IsUnlocked(card)) continue;

            candidates.Add(card);
        }

        if (candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
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

    private void ChooseCard()
    {
        if (offeredCard != null && DeckManager.Instance != null)
        {
            DeckManager.Instance.UnlockCard(offeredCard);
        }

        Finish();
    }

    private void Finish()
    {
        panel.SetActive(false);
        RunManager.Instance.CompleteCurrentNode();
    }
}