using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Teleport : MonoBehaviour
{
    public string sceneToLoad;
    public int currentLevelNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelProgress.UnlockNextLevel(currentLevelNumber);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
