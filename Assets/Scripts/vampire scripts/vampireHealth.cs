using System;
using UnityEngine;
using UnityEngine.VFX;

public class vampireHealth : MonoBehaviour
{
    //Mason Kuhn

    public int maxHealth = 200;
    public int currentHealth;
    public event Action onDamage;
     public VisualEffect blood;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        Debug.Log("hit");
        blood.Play();
        onDamage?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
