using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    public GameEndManager gameEndManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameEndManager.ShowEndScreen();
        }
    }
}
