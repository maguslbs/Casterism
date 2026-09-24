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
