using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject GameOverPanel;

    private bool GameOver = false;

    public AudioSource music;

    public PlayerMovement player;


    void Awake()
    {
        Instance = this;
    }

    public void PlayerDied()
    {
        GameOver = true;
        Time.timeScale = 0f; // stops the game

        music?.Pause();                  // null-safe — won't crash if not assigned
        player?.sfxSource?.Stop();       // null-safe
        GameOverPanel?.SetActive(true);  // null-safe

    }

    void Update()
    {
        if (GameOver && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
