using NUnit.Framework.Constraints;
using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] GameObject cam;
    [SerializeField] Sprite pSprite; // flip sprite on x depending on player pos

    public bool isPaused;
    public GameObject player;

    public playerController playerScript;

    //Creating win condition
    int gameGoalCount;

    float timeScaleOrig;

    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    //first function for manager
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        cam = GameObject.Find("cinemachineCamera");
        playerScript = player.GetComponent<playerController>();
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose() {
        // lose
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        if (gameGoalCount <= 0)
        {
            //you have won
            //I hope
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            //Note update the enemy ai script start with the following
            //gamemanager.instance.updateGameGoal(1);
            //Note Update takeDamage
            //within HP if statement gamemanager.instance.updateGameGoal(-1);  followed by destroy gameobject
        }
    }
}