using System.Collections.Generic;
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


    //Highest score player has reached
    public static int highScore = 0;

    //top ten highscores achieved, will be updating and changing
    public static List<int> topTenScores = new List<int>();




    [SerializeField] TMP_Text scoreText;

    [Header("Score Settings")]
    [SerializeField] bool resetScoreOnStart = false;  //update because of highscore tracker

    private const string HighScoreKey = "HighScore";
    private const string TopScoreKeyPrefix = "TopScore_";

    private void Awake()
    {
        instance = this;

        LoadHighScores();

        if (resetScoreOnStart)
        {
            ResetScore();
        }



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
        //scoreText.text = totalScore.ToString("0"); // starts at 0
    }

    public void AddScore(int amount)
    {
        //setting a temporary line to test score on kill
        Debug.Log("AddScore CALLED. Amount: " + amount + " | Before: " + totalScore);
        //

        totalScore += amount;
        if (totalScore < 0)
        {
            totalScore = 0;
        }
        //test
        Debug.Log("New totalScore: " + totalScore);
        //test


        //reinforce the zero 
        UpdateScoreUI();

        // ui will show the new high score
        if (UIManager.Instance != null)
        { 
        UIManager.Instance.UpdateScoreDisplay();
        UIManager.Instance.UpdateEmployeeRank();
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null. HUD score did not update.");
        }
    }

    public void SubtractScore(int amount)
    {
        totalScore -= amount;
        if (totalScore < 0)
        {
            totalScore = 0;
        }
        //setting limit to deduction to stop from going below 0
        totalScore = Mathf.Max(totalScore, 0);
        if (UIManager.Instance != null)
        { 
        UIManager.Instance.UpdateScoreDisplay();
        UIManager.Instance.UpdateEmployeeRank();
        }
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
            scoreText.text = totalScore.ToString("#,0");
        }
    }


    // Functions for management of Highscores
    // Call this when the full run ends, or when the player finishes the final level.
    public void SubmitFinalScore()
    {
        Debug.Log("SubmitFinalScore CALLED. totalScore = " + totalScore + ", current highScore = " + highScore);

        PlayerPrefs.SetInt("LastFinalScore", totalScore);
        PlayerPrefs.Save();

        AddScoreToTopTen(totalScore);

        if (totalScore > highScore)
        {
            highScore = totalScore;
            Debug.Log("NEW HIGH SCORE set to: " + highScore);
        }

        SaveHighScores();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHighScoreDisplay();
            Debug.Log("Told UIManager to update high score display");
        }
        else
        {
            Debug.Log("UIManager.Instance is NULL — display not updated");
        }

        Debug.Log("Final Score Submitted: " + totalScore);
        Debug.Log("High Score: " + highScore);
    }

    public void AddScoreToTopTen(int newScore)
    {
        topTenScores.Add(newScore);

        // Sort highest to lowest
        topTenScores.Sort((a, b) => b.CompareTo(a));

        // Keep only top 10
        while (topTenScores.Count > 10)
        {
            topTenScores.RemoveAt(topTenScores.Count - 1);
        }

        if (topTenScores.Count > 0)
        {
            highScore = topTenScores[0];
        }
    }

    //save the highscores
    //Using PlayerPref to assign a physical location
    public void SaveHighScores()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);

        for (int i = 0; i < 10; i++)
        {
            if (i < topTenScores.Count)
            {
                PlayerPrefs.SetInt(TopScoreKeyPrefix + i, topTenScores[i]);
            }
            else
            {
                PlayerPrefs.SetInt(TopScoreKeyPrefix + i, 0);
            }
        }

        PlayerPrefs.Save();
    }

    //load the high scores
    public void LoadHighScores()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        topTenScores.Clear();

        for (int i = 0; i < 10; i++)
        {
            int savedScore = PlayerPrefs.GetInt(TopScoreKeyPrefix + i, 0);

            if (savedScore > 0)
            {
                topTenScores.Add(savedScore);
            }
        }

        topTenScores.Sort((a, b) => b.CompareTo(a));

        if (topTenScores.Count > 0)
        {
            highScore = topTenScores[0];
        }
    }

    //This is only for the devs so we can clear out the Highscores for testing
    public void ClearSavedScores()
    {
        PlayerPrefs.DeleteKey("HighScore");

        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey("TopScore_" + i);
        }

        PlayerPrefs.Save();

        highScore = 0;
        topTenScores.Clear();

        UpdateScoreUI();

        Debug.Log("High scores cleared.");
    }

    private void Update()
    {// to clear the score Press F9 clears high score/top ten
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ClearSavedScores();
            ResetScore();

            // ui will show the new high score
            if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateHighScoreDisplay(); // update the high score
                    UIManager.Instance.UpdateScoreDisplay(); // update the score 
                    UIManager.Instance.UpdateEmployeeRank(); // update the employee rank
                }
        }
    }

    //creating a new save of score and values for Evaluation report
    public void SaveLevelResult(string sceneName)
    {
        int levelScore = Mathf.Max(0, totalScore - levelStartScore);

        string scoreKey = "BestScore_" + sceneName;
        string rankKey = "BestRank_" + sceneName;

        int previousBest = PlayerPrefs.GetInt(scoreKey, 0);

        if (levelScore > previousBest)
        {
            PlayerPrefs.SetInt(scoreKey, levelScore);
            PlayerPrefs.SetString(rankKey, GetRankLetter(levelScore));
            PlayerPrefs.Save();

            Debug.Log("Saved new best for " + sceneName + ": " + levelScore);
        }
        else
        {
            Debug.Log("Score did not beat previous best for " + sceneName + ". Score: " + levelScore + " Best: " + previousBest);
        }
    }

    private string GetRankLetter(int score)
    {
        float performance = Mathf.Clamp01(score / 2000f);

        if (performance >= 0.90f) return "S";
        if (performance >= 0.80f) return "A+";
        if (performance >= 0.70f) return "A";
        if (performance >= 0.60f) return "B+";
        if (performance >= 0.50f) return "B";
        if (performance >= 0.40f) return "C+";
        if (performance >= 0.30f) return "C";
        return "D";
    }
}
