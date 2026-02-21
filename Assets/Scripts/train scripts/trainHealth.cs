using UnityEngine;
using TMPro;

public class trainHealth : MonoBehaviour
{
    //Mason Kuhn

    public int maxHealth = 500;
    public int currentHealth;

    public TMP_Text healthText;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Health: {currentHealth}";
    }

    void Die()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}