using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardHolder;
    [SerializeField] GameObject upgradePanel;

    void Awake()
    {
        instance = this;
        
    }

    // LevelUpUI.instance.ShowUpgradeChoices(); call this in the area you would like to have activate the upgrade system.
    public void ShowUpgradeChoices()
    {
        upgradePanel.SetActive(true);

        CreateCard(new Upgrades("Damage Up", UpgradeType.Damage, .25f));
        CreateCard(new Upgrades("Speed Up", UpgradeType.Speed, .15f));
        CreateCard(new Upgrades("Health Up", UpgradeType.Health, .25f));
    }

    // Creating the upgrade cards for the UI so they don't live in scene forever.
    void CreateCard(Upgrades upgrade)
    {
        GameObject card = Instantiate(cardPrefab, cardHolder);
        card.GetComponent<UpgradeCardUI>().Setup(upgrade);
    }

    // Hides cards after upgrade is applied. ( Destroys cards, will repopulate them when ugprade tab is activated again.
    public void HideCards()
    {
        foreach (Transform c in cardHolder) 
        {
            Destroy(c.gameObject);
        }

        upgradePanel.SetActive(false);
    }
}
