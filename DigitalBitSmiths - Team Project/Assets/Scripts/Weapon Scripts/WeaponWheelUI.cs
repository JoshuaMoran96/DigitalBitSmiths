using UnityEngine;

public class WeaponWheelUI : MonoBehaviour
{
    [SerializeField] GameObject wheelPanel;

    void Start()
    {
        if (wheelPanel != null)
        {
            wheelPanel.SetActive(false);
        }
        
    }

    void Update()
    {   // Don't allow opening the weapon wheel if the pause menu is active
        if (gamemanager.instance != null && gamemanager.instance.menuActive != null && gamemanager.instance.menuActive != wheelPanel)
        {
            return;
        }



        // press tab down 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenWheel();
        }
        //release tab
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            CloseWheel();
        }
    }

    void OpenWheel()
    {
        if (wheelPanel != null)
        {
            wheelPanel.SetActive(true);
        }

        if (gamemanager.instance != null)
        {
            gamemanager.instance.isPaused = true;
            gamemanager.instance.menuActive = wheelPanel;
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseWheel()
    {
        if (wheelPanel != null)
        {
            wheelPanel.SetActive(false);
        }

        if (gamemanager.instance != null && gamemanager.instance.menuActive == wheelPanel)
        {
            gamemanager.instance.isPaused = false;
            gamemanager.instance.menuActive = null;
        }

        Time.timeScale = 1f;

        // Update to lock cursor back to center so player can aim again
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SelectWeapon(WeaponData weapon)
    {
        if (WeaponInventory.instance != null)
        {
            WeaponInventory.instance.EquipWeapon(weapon);
        }

        CloseWheel();
    }
}