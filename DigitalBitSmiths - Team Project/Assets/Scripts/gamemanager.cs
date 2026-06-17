using System.Collections.Generic;
using TMPro;
//using UnityEditor.Rendering.Universal;  was blocking build profile
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;

    //setting up an enum for game goals and objectives
    public enum LevelGoalType { ClearAllEnemies, ReachObjective }
    public static HashSet<string> completedLevels = new HashSet<string>();

    [Header("Menu")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public int upgradeTokens = 0;
    
    public LoadoutRefresher loadoutDisplay;

    [SerializeField] GameObject cam;
    [SerializeField] public Sprite pSprite; // flip sprite on x depending on player pos

    //Disappearing platform
    private List<DestroyPlatform> platforms = new List<DestroyPlatform>();


    // respawn point for player
    private Vector3 currentRespawnPosition;
    public GameObject playerStartPos;


    public bool isPaused;
    public GameObject player;

    public playerController playerScript;
    public string sceneToLoad;

    //Creating win condition
    int gameGoalCount;

    //Updated Win Condition, enemy tracker, objective tracker
    //updating for a adjustable win condition
    [Header("UI & Level Goals")]
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text ObjectiveText;
    //addition for unique goal per level
    [SerializeField] private string levelGoalMessage = "Reach the Trophy";
    [SerializeField] TextMeshProUGUI GoalUIText;
    //Set notificcation for UI when player crosses a checkpoint
    public GameObject checkPointPopup;

    //choose the goal type in Inspector for each level
    [SerializeField] private LevelGoalType currentGoalType = LevelGoalType.ReachObjective;

    int enemyCount;

    float timeScaleOrig;

    //creating bool for score submission
    private bool finalScoreSubmitted;

    //Temporary display for Highscore and final score
    [Header("Win Score UI")]
    [SerializeField] TMP_Text finalScoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text CurrentScoreText;

    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    //first function for manager
    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        timeScaleOrig = Time.timeScale;
        
        loadoutDisplay = FindAnyObjectByType<LoadoutRefresher>();


        // Update for cursor shoot interference
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Locate the player
        player = GameObject.FindWithTag("Player");
        cam = GameObject.Find("CinemachineCamera");
        playerScript = player.GetComponent<playerController>();

        //Set player spawn point
        playerStartPos = GameObject.FindWithTag("Player Start Pos");
        if (playerStartPos != null)
        {
            currentRespawnPosition = playerStartPos.transform.position;
            RespawnPlayer();
        }
        else if (player != null)
        {
            currentRespawnPosition = player.transform.position;
            //Debug.LogWarning("Player Start Pos not found. Using player's current position.");  just for starting position of level
        }

        //enemy tracker 
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        updateEnemyCountUI();


        //Set up for player Objectives and Goals
        if (ObjectiveText != null)
        {
            ObjectiveText.text = levelGoalMessage;
        }

        if (GoalUIText != null)
        {
            GoalUIText.text = levelGoalMessage;
        }
        else
        {
            Debug.LogWarning("Goal UI Text is not assigned in GameManager.");
        }



    }


    //setting up start for highscore and final score
    void Start()
    { // call the score system to manage score
        if (scoreSystem.instance != null)
        {
            scoreSystem.instance.SetLevelStartScore();
        }
    }

    public void AddTokens(int tokenAmount)
    {
        upgradeTokens += tokenAmount;
    }

    //Adding a call to change goals if things need to adjust mid level or if levels have a unique challenge
    public void UpdateObjectiveText(string newGoal)
    {
        if (ObjectiveText != null)
        {
            ObjectiveText.text = newGoal;
        }
    }


        // Update is called once per frame
        void Update()
    {
        

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

        // Current Score
        CurrentScore();
        
    }


    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        //update so player can use cursor in menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose() {
        // lose
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    //object condition
    public void youWin()
    {
        SubmitFinalScoreOnce();
       //updating so the function is handled at submission
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    //Updating enemy count function
    public void updateEnemyCount(int amount)
    {
        enemyCount += amount;

        if (enemyCount < 0)
        {
            enemyCount = 0;
        }

        updateEnemyCountUI();

        //in the case the goal is eliminate all enemies
        // Check if the win condition is met after an enemy dies
        CheckWinCondition();
    }

    void updateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = enemyCount.ToString("000");
        }
        else
        {
            Debug.LogWarning("Enemy Count Amount text is not assigned in GameManager.");
        }

    }

    //helper so every script can use the same player reference
    public Transform GetPlayerTransform()
    {
        if (player != null)
        {
            return player.transform;
        }
        return null;
    }

    //respawn method to remove manual drag

    public void UpdateRespawnPoint(Transform newCheckpoint)
    {
        currentRespawnPosition = newCheckpoint.position;
        ResetAllPlatforms();
    }
    public void RespawnPlayer()
    {
        if (player == null) return;

        //void and clear all projectiles fired while player is falling
        ClearPlayerProjectiles();

        // Reset player velocity to stop continuous falling physics
        if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
        }



        // Teleport player to the latest tracked position
        player.transform.position = currentRespawnPosition;

        // Forces Unity to instantly update the physics system location
        Physics2D.SyncTransforms();

        //foreach (var p in platforms)  will use in its own function istead will call that seperate function
        //    p.ResetPlatform();
        ResetAllPlatforms();

        //Player will relocate position back to checkpoint or spawn
        //Will not heal just carry dmg bak to position this is part one of our respawn
    }

    public void RespawnPlayerFullHealth()
    {
        //Reset Health
        if (playerScript != null)
        {
            playerScript.ResetHealth();
        }
        //call respawn player for location logic, this is part 2 of respawn logic
        RespawnPlayer();
    }


    //open function for enemy fall off map into RT zone
    public void OnEnemyFell(GameObject enemy)
    {
      
    }

    //updated win condition call when player reaches Endgoal
    public void ReachObjectiveGoal()
    {
        if(currentGoalType == LevelGoalType.ReachObjective)
        {
            youWin();
        }
    }

    //run check against the win condition and see if term has been met, can expand as objectives and enum expand
    private void CheckWinCondition()
    {
        if(currentGoalType == LevelGoalType.ClearAllEnemies && enemyCount <= 0)
        {
            youWin();
        }
    }


  public void AddPlatforms(DestroyPlatform p)
    {
        platforms.Add(p);
    }

    //creating a reset for destroyable platforms to avoid softlocking a players progression
    public void ResetAllPlatforms()
    {
        foreach (var p in platforms)
        {
            if (p != null)
            {
                p.ResetPlatform();
            }
        }
    }

    //creating a clean up to avoid player fire effects upon respawn
    //all fired projectiles with tag bullet are the criteria
    public void ClearPlayerProjectiles()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }

    //function to get the submitted highscores from score system and play
    private void SubmitFinalScoreOnce()
    {
        if (finalScoreSubmitted)
        {
            return;
        }

        finalScoreSubmitted = true;

        // get the scene index
        LevelComplete(SceneManager.GetActiveScene().name);

        if (scoreSystem.instance != null)
        {
            scoreSystem.instance.SubmitFinalScore();

            // Moved assignment for safely read and format the fields using the static access pattern
            if (finalScoreText != null)
            {
                finalScoreText.text = "FINAL SCORE: " + scoreSystem.totalScore.ToString("0000");
            }

            if (highScoreText != null)
            {
                highScoreText.text = "HIGH SCORE: " + scoreSystem.highScore.ToString("0000");
            }
        }
    }
    
    // Current Score
    private void CurrentScore()
    {

        if (scoreSystem.instance != null && CurrentScoreText != null)
        {
            CurrentScoreText.text = scoreSystem.totalScore.ToString("0000");
        }
    }


    // Levels
    // Level complete
    public static void LevelComplete(string sceneName) {

        completedLevels.Add(sceneName);

    }
    public static bool IsLevelComplete(string sceneName)
    {
        return completedLevels.Contains(sceneName);
    }

}