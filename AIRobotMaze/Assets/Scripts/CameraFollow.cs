using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // The object the camera follows (e.g., player)
    public Vector3 offset = new Vector3(0, 10, 0); // Height above the player
    public float smoothSpeed = 5f;   // How smooth the camera follows

    public float CameraAngle = 105f;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position from the top
        Vector3 desiredPosition = target.position + offset;

        // Smooth transition
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Always look down (top-down)
        transform.rotation = Quaternion.Euler(CameraAngle, 0f, 0f); 
    }
}
