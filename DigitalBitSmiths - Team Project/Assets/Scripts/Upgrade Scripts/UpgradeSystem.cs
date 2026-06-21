using NUnit.Framework;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem instance;

    public bool hasChosenUpgradeThisLevel = false;

    public bool upgradeAvailable = false;   // earned by beating a level
    public bool upgradeUsed = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when player selects an upgrade card
    public void ApplyUpgrade(Upgrades upgrade)
    {
        if (hasChosenUpgradeThisLevel)
        {
            Debug.Log("Upgrade already chosen this level!");
            return;
        }

        upgrade.ApplyUpgrade();
        hasChosenUpgradeThisLevel = true;
        upgradeAvailable = false;
    }

    // Reset when a new level loads
    public void ResetForNewLevel()
    {
        hasChosenUpgradeThisLevel = false;
    }
}
