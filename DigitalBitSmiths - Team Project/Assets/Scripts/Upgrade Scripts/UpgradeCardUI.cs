using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button chooseButton;

    public Image upgradeIconImage;
    public Sprite[] typeIcons;
    private Upgrades upgrade;

    public float upgradedHealth;
    public float upgradedDamage;
    public float upgradedSpeed;
    public void Setup(Upgrades upgradeData, bool displayOnly = false)
    {
        upgrade = upgradeData;

        titleText.text = upgrade.name;
        descriptionText.text = $"+{upgrade.amount} {upgrade.type}";

       int index = (int)upgrade.type;
       if (upgradeIconImage != null && typeIcons != null && index < typeIcons.Length)
            upgradeIconImage.sprite = typeIcons[index];

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


        chooseButton.onClick.RemoveAllListeners();
        //chooseButton.onClick.AddListener(ApplyUpgrade);
        
        if (displayOnly)
        {
            // no buy button
            chooseButton.gameObject.SetActive(false);
        }
        else
        {
            // during the gameplay apply the upgrades
            chooseButton.gameObject.SetActive(true);
            chooseButton.onClick.AddListener(ApplyUpgrade);
        }
    }

    public void ApplyUpgrade()
    {
        upgrade.ApplyUpgrade();
        
        if (LevelUpUI.instance != null)
        {
                    LevelUpUI.instance.HideCards();

        }

        if (gamemanager.instance != null && gamemanager.instance.playerScript != null)
            gamemanager.instance.playerScript.upgradeTesterOpen = false;
    }
}
