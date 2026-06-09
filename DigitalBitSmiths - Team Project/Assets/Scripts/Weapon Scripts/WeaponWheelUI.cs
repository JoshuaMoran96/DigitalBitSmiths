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
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenWheel();
        }

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

        if (gamemanager.instance != null)
        {
            gamemanager.instance.isPaused = false;
        }

        Time.timeScale = 1f;
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