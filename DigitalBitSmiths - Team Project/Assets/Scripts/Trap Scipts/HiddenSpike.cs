using UnityEngine;

public class HiddenSpike : MonoBehaviour
{
    [SerializeField] float startHeight;
    [SerializeField] float speed;
    [SerializeField] float endHeight;

    void Start()
    {
        startHeight = transform.position.y;
        endHeight = transform.position.y + 0.2f;
        transform.position = new Vector2(transform.position.x, startHeight);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
             transform.position = new Vector2(transform.position.x, endHeight);
          
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = new Vector2(transform.position.x, startHeight);
        }
    }
}
