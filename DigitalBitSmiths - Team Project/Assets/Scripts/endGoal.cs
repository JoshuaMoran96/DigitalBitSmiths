using UnityEngine;

public class endGoal : MonoBehaviour
{
    //seting behavior to trigger win condition
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            gamemanager.instance.youWin();
        }
    }
}
