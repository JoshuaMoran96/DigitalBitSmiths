using TMPro;
using UnityEngine;

public class scoreSystem : MonoBehaviour

// Basic score chart for the game
//pickups = 20
// Grunt = 25
//Wasp = 50
//Stinger = 75
//Peacemaker = 100
//Bulldozer = 100
//Mini-boss = 700
//Boss = 1500


{//creating score to travel betweeen levels
 //goal of this script is to handle the mechanic so Gamemanager doesnt have to
    public static scoreSystem instance;
    //overall game score
    public static int totalScore = 0;
    //current level score
    public static int levelStartScore = 0;
    [SerializeField] TMP_Text scoreText;

    [Header("Score Settings")]
    [SerializeField] bool resetScoreOnStart = true;

    private void Awake()
    {
        instance = this;

        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.Find("ScoreText");

            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<TMP_Text>();
            }
        }

        UpdateScoreUI();
    }

    public void SetLevelStartScore()
    {
        levelStartScore = totalScore;
    }

    public void AddScore(int amount)
    {
        totalScore += amount;
        UpdateScoreUI();
    }

    public void SubtractScore(int amount)
    {
        totalScore -= amount;
        //setting limit to deduction to stop from going below 0
        totalScore = Mathf.Max(totalScore, 0);
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        totalScore = 0;
        levelStartScore = 0;
        UpdateScoreUI();
    }

    public void ResetToLevelStartScore()
    {
        totalScore = levelStartScore;
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE " + totalScore.ToString("0000");
        }
    }
}
