using UnityEngine;

public class minimapFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float zOffset = -20f;

    //setting a start function to automatically target the player
    private void Start()
    {
            GameObject playerObj = gamemanager.instance.player;
           
            target = playerObj.transform;
    
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        transform.position = new Vector3(target.position.x, target.position.y, zOffset);
    }
}
