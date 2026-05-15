using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    //reference to the boss controller
    [SerializeField] BossController boss;

    //prevents trigger from activating multiple times
    private bool hasTriggered;

    void Start()
    {
        //automatically find boss if not assigned
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
        //stop trigger from activating again
        if (hasTriggered)
        {
            return;
        }

        //start boss intro when player enters trigger
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
