using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeTotalVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            AudioListener.volume = 0.75f;
        }
    }

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject areYouSurePanel;

    [Header("Buttons")]
    public GameObject returnBtn;
    public GameObject settingsBtn;
    public GameObject exitBtn;

    private bool isPaused = false;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (optionsMenuPanel.activeSelf || areYouSurePanel.activeSelf)
                {
                    BackToPauseMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(false);

        SetMainButtonsActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        SetMainButtonsActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public void OpenAreYouSure()
    {
        SetMainButtonsActive(false);
        areYouSurePanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        optionsMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(false);
        SetMainButtonsActive(true);
    }

    public void ConfirmExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    private void SetMainButtonsActive(bool state)
    {
        if (returnBtn) returnBtn.SetActive(state);
        if (settingsBtn) settingsBtn.SetActive(state);
        if (exitBtn) exitBtn.SetActive(state);
    }
}