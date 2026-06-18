using UnityEngine;
using TMPro;

public class hubPartCounter : MonoBehaviour
{

    public TextMeshProUGUI counterText;

    private void Start()
    {// a console check for picked up hearts

        Debug.Log("RDHeart1 = " + PlayerPrefs.GetInt("RDHeart1", 0));
        Debug.Log("RDHeart2 = " + PlayerPrefs.GetInt("RDHeart2", 0));
        Debug.Log("RDHeart3 = " + PlayerPrefs.GetInt("RDHeart3", 0));
        UpdatePartUI();
    }

    public void UpdatePartUI()
    {
        int collectedCount = 0;

        //check for each part collected
        if (PlayerPrefs.GetInt("RDHeart1", 0) == 1) collectedCount++;
        if (PlayerPrefs.GetInt("RDHeart2", 0) == 1) collectedCount++;
        if (PlayerPrefs.GetInt("RDHeart3", 0) == 1) collectedCount++;

        counterText.text = "RD Parts: " + collectedCount + "/3";
    }
}
