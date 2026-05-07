using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionMenu : MonoBehaviour
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

    public void RefreshButtons()
    {
        LevelButton[] buttons = GetComponentsInChildren<LevelButton>(true);

        foreach (var btn in buttons)
        {
            btn.Refresh(); // priverstinai atnaujina būseną
        }
    }

    public void OpenMenu()
    {
        mainMenuArea.SetActive(false);
        levelSelectionArea.SetActive(true);
        RefreshButtons();
    }



}