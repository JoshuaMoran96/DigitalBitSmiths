using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistanceManager : MonoBehaviour
{
    public static PersistanceManager instance;

    [Header("----- Persistent Stats -----")]
    public float maxHP = 100f;
    public float damage = 10f;
    public float speed = 10f;


    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;

        // Save as "last level" only for gameplay scenes
        if (name != "MainMenu" && name != "credits" && name != "HUB Level ALPHA" )
        {
            PlayerPrefs.SetString("LastLevel", name);
            PlayerPrefs.Save();
            Debug.Log("Saved last level: " + name);
        }
    }

    // Upgrade functions
    public void AddHP(float amount)
    {
        maxHP += amount;
    }

    public void AddDamage(float amount)
    {
        damage += amount;
    }

    public void AddSpeed(float amount)
    {
        speed += amount;
    }

}
