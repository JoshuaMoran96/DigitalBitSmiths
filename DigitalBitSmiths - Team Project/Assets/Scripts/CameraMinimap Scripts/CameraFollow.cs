using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float zDepth = -10f;

    [Header("Vertical Lock")]
    [SerializeField] private float fixedY = -7.42f;   // camera stays at this height

    [Header("Horizontal Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -42f;        // left limit (set by hand)
    [SerializeField] private float maxX = 42f;         // right limit (set by hand)

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offsetX;

        if (useBounds)
            targetX = Mathf.Clamp(targetX, minX, maxX);

        Vector3 desired = new Vector3(targetX, fixedY, zDepth);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // X eases smoothly; Y and Z are hard-locked so nothing ever drifts vertically
        transform.position = new Vector3(smoothed.x, fixedY, zDepth);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Camera cam = GetComponent<Camera>();
        float h = (cam != null && cam.orthographic) ? cam.orthographicSize : 5f;
        float w = (cam != null) ? h * cam.aspect : h * 1.78f;

        Gizmos.color = Color.yellow;

        // left-most camera frame
        DrawFrame(minX, fixedY, w, h);
        // right-most camera frame
        DrawFrame(maxX, fixedY, w, h);
    }

    private void DrawFrame(float cx, float cy, float w, float h)
    {
        Vector3 bl = new Vector3(cx - w, cy - h, 0f);
        Vector3 br = new Vector3(cx + w, cy - h, 0f);
        Vector3 tr = new Vector3(cx + w, cy + h, 0f);
        Vector3 tl = new Vector3(cx - w, cy + h, 0f);
        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
    }
#endif
}