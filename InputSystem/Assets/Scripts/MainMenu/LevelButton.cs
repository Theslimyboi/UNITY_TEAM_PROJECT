using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    public Button button;
    public GameObject lockIcon;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int unlocked = LevelProgress.GetUnlockedLevel();

        bool isUnlocked = levelNumber <= unlocked;

        button.interactable = isUnlocked;
        lockIcon.SetActive(!isUnlocked);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Level" + levelNumber);
        });
    }
}

