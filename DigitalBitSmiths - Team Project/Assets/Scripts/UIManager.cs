using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections.Generic;

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
    }

    // Update is called once per frame
    void Update()
    {
    }


    // Get Player Current Level
    void GetCurrentLevel()
    {
       
       for (int i = 0; i < gameLevels.Count; i++)
        {
            if (gameLevels[i] == currentLevel.name)
            {
                playerCurrentLevel.text = gameLevels.Count.ToString();
            }
        }
    }

}
