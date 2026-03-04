using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class trainHealth : MonoBehaviour
{
    //Mason Kuhn

    public int maxHealth = 500;
    public int currentHealth;

    public bool isBreached { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnBreached;

    void Awake()
    {
        currentHealth = maxHealth;
        isBreached = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isBreached)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Breach();
        }
    }

    void Breach()
    {
        isBreached = true;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        OnBreached?.Invoke();
    }
}