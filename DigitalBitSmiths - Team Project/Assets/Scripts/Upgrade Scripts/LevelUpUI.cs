using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardHolder;
    [SerializeField] GameObject upgradePanel;

    void Awake()
    {
        //upgradePanel.SetActive(true);
        instance = this;
        
    }
    
    void OnEnable()
    {
        ShowUpgradeChoices();
    }
    public void ShowUpgradeChoices()
    {
        upgradePanel.SetActive(true);
        ClearCards();

        CreateCard(new Upgrades("Damage Up", UpgradeType.Damage, .25f), false);
        CreateCard(new Upgrades("Speed Up",  UpgradeType.Speed,  .15f), false);
        CreateCard(new Upgrades("Health Up", UpgradeType.Health, .25f), false);
    }
    public void ShowAvailableUpgrades()
    {
        upgradePanel.SetActive(true);
        ClearCards();

        CreateCard(new Upgrades("Damage Up", UpgradeType.Damage, .25f), true);
        CreateCard(new Upgrades("Speed Up",  UpgradeType.Speed,  .15f), true);
        CreateCard(new Upgrades("Health Up", UpgradeType.Health, .25f), true);
    }

    // Creating the upgrade cards for the UI so they don't live in scene forever.
    void CreateCard(Upgrades upgrade, bool displayOnly)
    {
        GameObject card = Instantiate(cardPrefab, cardHolder);
        card.GetComponent<UpgradeCardUI>().Setup(upgrade, displayOnly);
    }

    void ClearCards()
    {
        foreach (Transform c in cardHolder)
            Destroy(c.gameObject);
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
