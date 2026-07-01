using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    // Scripts
    // game Levels
    [SerializeField] List<string> gameLevels;
    [SerializeField] Scene currentLevel; // gets the current scene or 'level'


    // === Player HUD === \\
    [SerializeField] TextMeshProUGUI playerHealthPercentage; // will convert to string after calc

    [SerializeField] TextMeshProUGUI playerCurrentLevel; // Currentl Level the player is on
    [SerializeField] float lerpSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Weapon Loadout 
    [SerializeField] Image primaryWeapon;
    [SerializeField] Image meleeWeapon;

    [SerializeField] TextMeshProUGUI totalAmmoCount;
    [SerializeField] TextMeshProUGUI CurrentAmmoCount;
    [SerializeField] TextMeshProUGUI primaryWeaponName;
    [SerializeField] TextMeshProUGUI MeleeWeaponName;

    // Scores 
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI employeeRankText;

    // how difficult is each rank to earn
    [SerializeField] float scoreForTopRank = 2000f;



    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Get the current level
        currentLevel = SceneManager.GetActiveScene(); // not sure if we should get the index or name (and just compare current scene name against a list of scenes)

        // Set the current level = Level: 1
        GetCurrentLevel();
        //UpdateWeaponDisplay();
        UpdateScoreDisplay();
        UpdateHighScoreDisplay();
        UpdateEmployeeRank();
    }

    // weapon change call
    public void UpdateWeaponDisplay()
    {
        if (WeaponInventory.instance.currentWeapon == null)
        {
            primaryWeapon.sprite = null;
            primaryWeapon.enabled = false;
            primaryWeaponName.text = "Unequipped";
        }

        WeaponData weapon = WeaponInventory.instance.currentWeapon;
        if (weapon != null && weapon.weaponIcon != null)
        {
            primaryWeapon.sprite = weapon.weaponIcon;
            primaryWeapon.enabled = true;
            if (primaryWeaponName != null)
                primaryWeaponName.text = weapon.weaponName;
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateWeaponDisplay();
    }


    // Get Player Current Level
    void GetCurrentLevel()
    {
        if (playerCurrentLevel == null)
        {
            return;
        }

        for (int i = 0; i < gameLevels.Count; i++)
        {
            if (gameLevels[i] == currentLevel.name)
            {
                playerCurrentLevel.text = (i + 1).ToString();
                break;
            }
        }

    }

    // Update Scores
    public void UpdateHighScoreDisplay()
    {
        //update needed highscore to display for each local level and less of a global score
        if (highScoreText == null)
        {
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        int levelBestScore = PlayerPrefs.GetInt("BestScore_" + currentSceneName, 0);

        highScoreText.text = levelBestScore.ToString("#,0");
    }
    public void UpdateScoreDisplay()
    {
        //making follow up update for display of current level
        if (scoreText == null)
        {
            return;
        }

        int currentLevelScore = Mathf.Max(0, scoreSystem.totalScore - scoreSystem.levelStartScore);
        scoreText.text = currentLevelScore.ToString("#,0");
    }

    // RANKING 

    public void UpdateEmployeeRank()
    {
        // update the rank similar to score and highscore
        if (employeeRankText == null)
        {
            return;
        }

        int currentLevelScore = Mathf.Max(0, scoreSystem.totalScore - scoreSystem.levelStartScore);
        float scoreNorm = Mathf.Clamp01(currentLevelScore / scoreForTopRank);

        employeeRankText.text = GetRankLetter(scoreNorm);


    }

    string GetRankLetter(float performance)
    {
        if (performance >= 0.90f) return "S";
        if (performance >= 0.80f) return "A+";
        if (performance >= 0.70f) return "A";
        if (performance >= 0.60f) return "B+";
        if (performance >= 0.50f) return "B";
        if (performance >= 0.40f) return "C+";
        if (performance >= 0.30f) return "C";
        return "D";
    }

    //setup to update the evaluation report and call it
    public void UpdateEvaluationReport()
    {
        UpdateScoreDisplay();
        UpdateHighScoreDisplay();
        UpdateEmployeeRank();


    }
}
