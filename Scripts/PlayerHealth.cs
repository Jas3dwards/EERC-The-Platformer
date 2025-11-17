using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    public HealthBar healthBar;
    public Transform currentCheckpoint;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentCheckpoint.position = transform.position;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Respawn();

        healthBar.SetHealth(currentHealth); 
        Debug.Log("Player took damage! Current health: " + currentHealth);
    }
    public void Respawn()
    {
        transform.position = currentCheckpoint.position;
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.transform.tag == "Checkpoint"){
            currentCheckpoint = collision.transform;
            Debug.Log(currentCheckpoint);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("Active");
        }
        if (collision.transform.tag == "Enemy")
        {
            TakeDamage(1);
        }
        if (collision.transform.tag == "Elevator")
        {
            Debug.Log("Scene Changed");
            SceneManager.LoadScene(1);
        }
    }

    
}
