using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Leaderboard : MonoBehaviour
{
    public TMP_Text scoreText;
    private float totalScore = 0;
    private int attempts = 0;

    public void UpdateScore(float matchPercentage)
    {
        totalScore += matchPercentage;
        attempts++;
        float averageScore = totalScore / attempts;
        scoreText.text = "Score: " + averageScore.ToString("F2");
    }
}

