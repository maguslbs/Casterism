using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int totalHealth = 100;

    private Flash flash;

    private void Awake()
    {
        flash = GetComponentInChildren<Flash>();
    }

    private int currentHealth;
    private int maxHealthReduction = 0; //taking notes how many hp taken by curse

    private void Start()
    {
        currentHealth = totalHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthText.text = currentHealth + "/" + totalHealth;
        healthSlider.maxValue = totalHealth;
        healthSlider.value = currentHealth;
    }

    public void HealDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth += amount;

        if (currentHealth > totalHealth)
        {
            currentHealth = totalHealth;
        }

        UpdateHealthUI();
    }

    public void ReduceMaxHealth(int amount) //Upon cursed by skeleton king
    {
        if (amount <= 0) return;

        int oldTotal = totalHealth;                         
        totalHealth = Mathf.Max(1, totalHealth - amount);
        maxHealthReduction += oldTotal - totalHealth;       

        currentHealth = Mathf.Clamp(currentHealth - amount, 1, totalHealth);
        UpdateHealthUI();
    }

    public void RestoreMaxHealth() //upon cursed cleansed to restore hp
    {
        if (maxHealthReduction <= 0) return;

        totalHealth += maxHealthReduction;

        if (IsAlive())
        {
            currentHealth = Mathf.Min(currentHealth + maxHealthReduction, totalHealth);
        }

        maxHealthReduction = 0;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        StartCoroutine(flash.FlashRoutine());

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthUI();
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

}
