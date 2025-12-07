using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public GameObject endScreenCanvas;

    public void ShowEndScreen()
    {
        endScreenCanvas.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
