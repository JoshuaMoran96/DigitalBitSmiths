using UnityEngine;

public class endGoal : MonoBehaviour
{
    public float startHeight;
    public float endHeight;
    public float speed;
    private void Start()
    {
        startHeight = transform.position.y;
    }

    //seting behavior to trigger win condition
    private void Update()
    {

        float currY = startHeight + Mathf.PingPong(Time.time * speed, endHeight);
        transform.position = new Vector2(transform.position.x, currY);
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            gamemanager.instance.youWin();
        }
    }

}
