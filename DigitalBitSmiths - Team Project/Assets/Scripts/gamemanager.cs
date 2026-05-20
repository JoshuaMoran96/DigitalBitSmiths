
using UnityEngine;
using TMPro;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;
    [Header("Menu")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] GameObject cam;
    [SerializeField] public Sprite pSprite; // flip sprite on x depending on player pos

    // respawn point for player
    private Vector3 currentRespawnPosition;

    public bool isPaused;
    public GameObject player;

    public playerController playerScript;

    //Creating win condition
    int gameGoalCount;

    //Updated Win Conditio, enemy tracker, objective tracker
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text ObjectiveText;
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

        player = GameObject.FindWithTag("Player");
        cam = GameObject.Find("CinemachineCamera");
        playerScript = player.GetComponent<playerController>();

        //enemy tracker 
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        updateEnemyCountUI();

        if (ObjectiveText != null)
        {
            ObjectiveText.text = "Reach the Trophy";
        }

        if (player != null)
        {
            // Store the player's starting position as the first respawn point
            currentRespawnPosition = player.transform.position;
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
    }

    void updateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = enemyCount.ToString("00");
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
    }


    //open function for enemy fall off map into RT zone
    public void OnEnemyFell(GameObject enemy)
    {
      
    }

    //older win update
    //public void updateGameGoal(int amount)
    //{
    //    gameGoalCount += amount;
    //    if (gameGoalCount <= 0)
    //    {
    //        //you have won
    //        //I hope
    //        statePause();
    //        menuActive = menuWin;
    //        menuActive.SetActive(true);
    //        //Note update the enemy ai script start with the following
    //        //gamemanager.instance.updateGameGoal(1);
    //        //Note Update takeDamage
    //        //within HP if statement gamemanager.instance.updateGameGoal(-1);  followed by destroy gameobject
    //    }
    //}

}