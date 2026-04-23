using UnityEngine;
using UnityEngine.VFX;

public class vampireHealth : MonoBehaviour
{
    //Mason Kuhn

    public int maxHealth = 200;
    public int currentHealth;
    public VisualEffect blood;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        blood.Play();

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
