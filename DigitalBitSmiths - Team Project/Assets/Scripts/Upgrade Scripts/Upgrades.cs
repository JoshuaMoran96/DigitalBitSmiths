
using UnityEngine;

public enum UpgradeType
{
    Damage,
    Health,
    Speed
}

public class Upgrades
{
    public string name;
    public UpgradeType type;
    public float amount;

    public bool isUpgraded = false;

    public Upgrades (string name, UpgradeType type, float amount)
    {
        this.name = name;
        this.type = type;
        this.amount = amount;
    }
    

    // Applies the chosen upgrade to player stats.

    public virtual void ApplyUpgrade()
    {
        playerController player = gamemanager.instance.playerScript;
        switch (type) 
        { 
            case UpgradeType.Damage:
                player.AddDamage(amount);
                isUpgraded = true;
                break;

            case UpgradeType.Health:
                player.AddHP(amount);
                isUpgraded = true;
                break;
            case UpgradeType.Speed:
                player.AddSpeed(amount);
                isUpgraded = true;
                break;
        }
    }
}
