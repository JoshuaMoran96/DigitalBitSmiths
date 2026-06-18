using UnityEngine;

public class partCollected : MonoBehaviour
{
    //being set for RD Heart1, RD Heart2 , and RD Heart3
    public string partSaveKey;

    private void Start()
    {
        if (PlayerPrefs.GetInt(partSaveKey, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //save and annotate when a certain heart is collected and log it
            PlayerPrefs.SetInt(partSaveKey, 1);
            PlayerPrefs.Save();

            Debug.Log(partSaveKey + " collected!");
            Destroy(gameObject);
        }
    }
}
