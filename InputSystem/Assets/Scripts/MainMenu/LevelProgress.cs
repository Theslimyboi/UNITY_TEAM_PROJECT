using UnityEngine;

public static class LevelProgress
{
    private const string KEY = "HighestLevelUnlocked";

    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(KEY, 1); // default = 1
    }

    public static void UnlockNextLevel(int completedLevel)
    {
        int current = GetUnlockedLevel();

        if (completedLevel >= current)
        {
            PlayerPrefs.SetInt(KEY, completedLevel + 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked level: " + (completedLevel + 1));
        }
    }
}
