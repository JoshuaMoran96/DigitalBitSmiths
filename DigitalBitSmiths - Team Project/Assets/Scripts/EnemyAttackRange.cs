using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    [SerializeField] RaycastEnemyAI enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<RaycastEnemyAI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && enemy != null)
        {
            enemy.SetPlayerInAttackRange(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.SetPlayerInAttackRange(false);
        }
    }
}
