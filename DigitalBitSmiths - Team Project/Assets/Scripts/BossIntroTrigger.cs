using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    [SerializeField] BossController boss;

    private bool hasTriggered;

    void Start()
    {
        if (boss == null)
        {
            boss = GetComponent<BossController>();

            if (boss == null && transform.parent != null)
            {
                boss = transform.parent.GetComponentInChildren<BossController>();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            if (boss != null)
            {
                boss.StartBossIntro();
            }
        }
    }
}
