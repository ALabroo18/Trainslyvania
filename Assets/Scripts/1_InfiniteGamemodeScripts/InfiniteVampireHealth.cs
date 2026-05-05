using UnityEngine;
using UnityEngine.VFX;

public class InfiniteVampireHealth : MonoBehaviour
{
    //Mason Kuhn

    public int maxHealth = 200;
    public int currentHealth;
    public event System.Action OnDied;

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
            OnDied?.Invoke();
        }
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

    public void SetStats(int hp)
    {
        maxHealth = hp;
        currentHealth = hp;
    }
}
