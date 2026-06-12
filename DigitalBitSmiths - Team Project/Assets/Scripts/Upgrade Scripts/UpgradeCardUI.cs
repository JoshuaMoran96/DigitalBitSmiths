using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button chooseButton;

    private Upgrades upgrade;

    public float upgradedHealth;
    public float upgradedDamage;
    public float upgradedSpeed;
    public void Setup(Upgrades upgradeData)
    {
        upgrade = upgradeData;

        titleText.text = upgrade.name;
        descriptionText.text = $"+{upgrade.amount} {upgrade.type}";

        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                titleText.color = Color.red;
                descriptionText.color = Color.red;
                break;

            case UpgradeType.Speed:
                titleText.color = Color.blue;
                descriptionText.color = Color.blue;
                break;

            case UpgradeType.Health:
                titleText.color = Color.green;
                descriptionText.color = Color.green;
                break;
        }


        chooseButton.onClick.AddListener(ApplyUpgrade);
    }

    void ApplyUpgrade()
    {
        upgrade.ApplyUpgrade();
        LevelUpUI.instance.HideCards();
        gamemanager.instance.playerScript.upgradeTesterOpen = false;
    }
}
