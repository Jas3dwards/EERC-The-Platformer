using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("EnemyAttack script active!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered with: " + collision.name + " / Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("Dealing 1 damage to player");
                health.TakeDamage(1);
            }
            else
            {
                Debug.LogWarning("No PlayerHealth component found on " + collision.name);
            }
        }
    }
}
