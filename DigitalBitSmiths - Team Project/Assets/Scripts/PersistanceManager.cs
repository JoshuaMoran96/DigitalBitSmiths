using UnityEngine;

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
        }
        else
        {
            Destroy(gameObject);
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
