using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] players;
    private int currentPlayerIndex = 0;

    private void Start()
    {
        // Activate only the first player
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i == 0);
        }
    }

    private void Update()
    {
        // Switch between players with number keys 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchPlayer(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchPlayer(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchPlayer(2);
    }

    void SwitchPlayer(int index)
    {
        if (index == currentPlayerIndex) return;

        // Save current player position
        Vector3 currentPos = players[currentPlayerIndex].transform.position;

        // Disable current player
        players[currentPlayerIndex].SetActive(false);

        // Enable new player
        players[index].SetActive(true);

        // Move new player to old position
        players[index].transform.position = currentPos;

        // Update index
        currentPlayerIndex = index;
    }
}
