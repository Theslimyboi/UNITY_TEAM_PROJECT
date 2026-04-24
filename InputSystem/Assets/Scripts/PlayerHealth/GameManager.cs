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
        Time.timeScale = 0f; // sustabdo žaidimą
        music.Pause();
        player.sfxSource.Stop();
        GameOverPanel.SetActive(true);
    }

    void Update()
    {
        if (GameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }
    }
}
