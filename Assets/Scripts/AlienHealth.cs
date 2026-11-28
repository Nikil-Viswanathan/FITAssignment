
using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class AlienHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1;

    private int currentHealth;
    private Animator animator;
    private EnemySpawner enemySpawner;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log("Monster health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (enemySpawner != null)
            enemySpawner.EnemyDestroyed();

        if (animator != null)
        {
            animator.SetTrigger("Die");
            StartCoroutine(DestroyAfterDeath());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1f);


        Destroy(gameObject);
    }
}
