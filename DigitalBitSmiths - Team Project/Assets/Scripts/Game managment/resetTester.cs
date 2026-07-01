using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] bool resetRDPartsOnStart;

    private void Start()
    {
        if (resetRDPartsOnStart)
        {
            PlayerPrefs.DeleteKey("RDHeart1");
            PlayerPrefs.DeleteKey("RDHeart2");
            PlayerPrefs.DeleteKey("RDHeart3");
            PlayerPrefs.Save();

            Debug.Log("RD Heart progress reset.");
        }
    }
}
