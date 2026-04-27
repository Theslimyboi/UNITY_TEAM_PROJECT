using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Menu windows")]
    public GameObject mainMenuArea;
    public GameObject optionsArea;
    public GameObject levelSelectionArea;

    public void OpenOptions()
    {
        mainMenuArea.SetActive(false);
        optionsArea.SetActive(true);
    }

    public void OpenLevelSelection()
    {
        mainMenuArea.SetActive(false);
        levelSelectionArea.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}