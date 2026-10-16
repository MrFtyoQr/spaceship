using UnityEngine;
using System.Collections.Generic;

public static class SaveManager
{
    const string KEY_POINTS = "TotalPoints";
    const string KEY_UNLOCKED = "UnlockedShips";
    const string KEY_SELECTED = "SelectedShip";
    const string KEY_HIGHSCORE = "HighScore";

    public static int LoadPoints()
    {
        return PlayerPrefs.GetInt(KEY_POINTS, 0);
    }

    public static void SavePoints(int total)
    {
        PlayerPrefs.SetInt(KEY_POINTS, total);
        PlayerPrefs.Save();
    }

    public static HashSet<int> LoadUnlockedShips()
    {
        var set = new HashSet<int> { 0 };
        string saved = PlayerPrefs.GetString(KEY_UNLOCKED, "0");
        foreach (string s in saved.Split(','))
        {
            if (int.TryParse(s.Trim(), out int index))
                set.Add(index);
        }
        return set;
    }

    public static void SaveUnlockedShips(HashSet<int> indices)
    {
        indices.Add(0);
        PlayerPrefs.SetString(KEY_UNLOCKED, string.Join(",", indices));
        PlayerPrefs.Save();
    }

    public static int LoadSelectedShip()
    {
        return PlayerPrefs.GetInt(KEY_SELECTED, 0);
    }

    public static void SaveSelectedShip(int index)
    {
        PlayerPrefs.SetInt(KEY_SELECTED, index);
        PlayerPrefs.Save();
    }

    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);
    }

    public static void SaveHighScore(int score)
    {
        if (score > LoadHighScore())
        {
            PlayerPrefs.SetInt(KEY_HIGHSCORE, score);
            PlayerPrefs.Save();
        }
    }
}
