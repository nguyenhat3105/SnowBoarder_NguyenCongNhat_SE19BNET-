using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Assets.Scripts.Score
{
    public class HighestScore : MonoBehaviour
    {

        // Lưu điểm số cao nhất nếu điểm mới cao hơn
        public static void SaveHighScore(int score)
        {
            List<int> highScores = new List<int>();

            // Lấy danh sách điểm cũ
            for (int i = 0; i < 5; i++)
            {
                highScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
                PlayerPrefs.Save();
            }

            // Thêm điểm mới vào danh sách
            highScores.Add(score);
            highScores.Sort((a, b) => b.CompareTo(a)); 

            // Chỉ lưu 10 điểm cao nhất
            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
            }

            PlayerPrefs.Save();
        }

        

        
    }
}
