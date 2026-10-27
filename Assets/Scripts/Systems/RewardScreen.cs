using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;   // BARU
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

    private enum UpgradeType { Attack, Heal }   // BARU

    [SerializeField] private GameObject panel;
    [SerializeField] private RewardOption hpOption;
    [FormerlySerializedAs("actionOption")]                     // BARU
    [SerializeField] private RewardOption upgradeOption;       // UBAH: dulu actionOption
    [SerializeField] private RewardOption cardOption;

    [Header("HP Reward per Tier")]
    [SerializeField] private int easyHpReward = 50;
    [SerializeField] private int normalHpReward = 70;
    [SerializeField] private int eliteHpReward = 100;

    [Header("Card Reward")]
    [SerializeField] private List<CardData> cardRewardPool = new List<CardData>();
    [Header("Upgrade Icons")]                            // BARU
    [SerializeField] private Sprite attackUpgradeIcon;   // BARU
    [SerializeField] private Sprite healUpgradeIcon;     // BARU

    private int hpReward;
    private CardData offeredCard;
    private UpgradeType offeredUpgrade;   // BARU

    private void Awake()
    {
        panel.SetActive(false);

        if (hpOption.button != null) hpOption.button.onClick.AddListener(ChooseHp);
        if (upgradeOption.button != null) upgradeOption.button.onClick.AddListener(ChooseUpgrade);   // UBAH
        if (cardOption.button != null) cardOption.button.onClick.AddListener(ChooseCard);
    }

    public void Show(MapTier tier)
    {
        hpReward = GetHpReward(tier);
        SetTexts(hpOption, "Vitality", $"+{hpReward} Max HP");

        bool hasUpgrade = PickUpgrade(out offeredUpgrade);   // BARU
        if (hasUpgrade)
        {
            bool isAttack = offeredUpgrade == UpgradeType.Attack;   // BARU

            if (isAttack)
                SetTexts(upgradeOption, "Sharpen", $"All attack cards +{RunManager.Instance.AttackBonusPerUpgrade} damage");
            else
                SetTexts(upgradeOption, "Restoration", $"All heal cards +{RunManager.Instance.HealBonusPerUpgrade} HP");

            if (upgradeOption.icon != null)   // BARU
                upgradeOption.icon.sprite = isAttack ? attackUpgradeIcon : healUpgradeIcon;
        }
        SetOptionVisible(upgradeOption, hasUpgrade);         // UBAH

        offeredCard = PickCardReward();
        if (offeredCard != null)
        {
            SetTexts(cardOption, offeredCard.cardName, offeredCard.GetDescription());   // UBAH: dulu .description
            if (cardOption.icon != null) cardOption.icon.sprite = offeredCard.illustration;
        }
        SetOptionVisible(cardOption, offeredCard != null);

        panel.SetActive(true);
    }

    // BARU: acak kalau dua-duanya masih bisa, kalau satu penuh pasti yang lain
    private bool PickUpgrade(out UpgradeType type)
    {
        bool canAttack = RunManager.Instance.CanUpgradeAttack;
        bool canHeal = RunManager.Instance.CanUpgradeHeal;

        if (canAttack && canHeal)
        {
            type = Random.value < 0.5f ? UpgradeType.Attack : UpgradeType.Heal;
            return true;
        }

        type = canHeal ? UpgradeType.Heal : UpgradeType.Attack;
        return canAttack || canHeal;
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

    // DIHAPUS: ChooseAction()

    private void ChooseUpgrade()   // BARU
    {
        if (offeredUpgrade == UpgradeType.Attack)
            RunManager.Instance.UpgradeAttack();
        else
            RunManager.Instance.UpgradeHeal();

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