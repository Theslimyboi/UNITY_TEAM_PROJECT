using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TextMeshProUGUI livesText;

    void Update()
    {
        livesText.text = "Lives: " + playerHealth.currentLives;
    }
}
