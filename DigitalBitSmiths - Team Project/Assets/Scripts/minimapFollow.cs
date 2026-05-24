using UnityEngine;

public class minimapFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float zOffset = -20f; 

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        transform.position = new Vector3(target.position.x, target.position.y, zOffset);
    }
}
