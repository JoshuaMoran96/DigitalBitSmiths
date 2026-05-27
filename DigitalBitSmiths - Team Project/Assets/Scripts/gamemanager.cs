using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;

    //setting up an enum for game goals and objectives
    public enum LevelGoalType { ClearAllEnemies, ReachObjective }

    [Header("Menu")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

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

    //Creating win condition
    int gameGoalCount;

    //Updated Win Condition, enemy tracker, objective tracker
    //updating for a adjustable win condition
    [Header("UI & Level Goals")]
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text ObjectiveText;
    //addition for unique goal per level
    [SerializeField] private string levelGoalMessage = "Reach the Trophy";
    //Set notificcation for UI when player crosses a checkpoint
    public GameObject checkPointPopup;

    //choose the goal type in Inspector for each level
    [SerializeField] private LevelGoalType currentGoalType = LevelGoalType.ReachObjective;

    int enemyCount;

    float timeScaleOrig;

    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    //first function for manager
    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        timeScaleOrig = Time.timeScale;


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
            Debug.LogWarning("Player Start Pos not found. Using player's current position.");
        }

        //enemy tracker 
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        updateEnemyCountUI();


        //Set up for player Objectives and Goals
        if (ObjectiveText != null)
        {
            //updated to be a variable
            ObjectiveText.text = levelGoalMessage;
        }


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

    //creating aa reset for destroyable platforms to avoid softlocking a players progression
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

}