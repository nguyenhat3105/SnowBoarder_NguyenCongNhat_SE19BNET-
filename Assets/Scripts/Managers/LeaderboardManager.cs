using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts.Score; // Import HighestScore

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text[] scoreTexts;
    public TMP_Text highScoreText; // Hiển thị điểm số cao nhất
    public GameObject HighScorePanel;

    void Start()
    {
        //LoadHighScores();
    }

    public void ShowHighScores()
    {
        HighScorePanel.SetActive(true);
        //LoadHighScores();
    }

    public void SaveHighScore(int newScore)
    {
        List<int> highScores = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            highScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
        }

        highScores.Add(newScore);
        highScores.Sort((a, b) => b.CompareTo(a));

        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }

        // Cập nhật HighestScore mới
        HighestScore.SaveHighScore(newScore);
        PlayerPrefs.Save();
    }

    //void LoadHighScores()
    //{
    //    if (highScoreText != null)
    //    {
    //        highScoreText.text = "High Score: " + HighestScore.GetHighScore();
    //    }

    //    for (int i = 0; i < scoreTexts.Length; i++)
    //    {
    //        int score = PlayerPrefs.GetInt("HighScore" + i, 0);
    //        scoreTexts[i].text = (i + 1) + ". " + score;
    //    }
    //}

    //public void ResetHighScores()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        PlayerPrefs.DeleteKey("HighScore" + i);
    //    }

    //    // Reset điểm số cao nhất
    //    HighestScore.ResetHighScore();
    //    PlayerPrefs.Save();
    //    LoadHighScores();
    //}
}
