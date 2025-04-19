using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.localScale = target.localScale;
        }
    }
}
