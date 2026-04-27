using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [Header("Menu")]
    public GameObject mainMenuArea;
    public GameObject levelSelectionArea;

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoBack()
    {
        levelSelectionArea.SetActive(false);
        mainMenuArea.SetActive(true);
    }
}