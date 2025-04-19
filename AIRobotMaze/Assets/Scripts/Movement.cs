using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [Header("Sensing Settings")]
    public float maxSensingDistance = 5.0f;
    public float avoidDistanceFront = 1.0f;
    public float avoidDistanceSide = 3.0f;
    public float rayOffset = 0.5f;

    [Header("Avoidance Settings")]
    public float avoidanceTurnAngle_deg = 45.0f;
    public float incAngleMag_deg = 0.1f;

    [Header("Direction Lock")]
    public float directionLockDuration = 1.5f;

    private bool avoidObstacle = false;
    private bool directionLocked = false;

    private float incAngle_deg = 0;
    private float lockedIncAngle_deg = 0;
    private float turnAngle_deg = 0;
    private float timeSinceLastAvoid = 0f;

    private GameObject hitObject = null;

    void Update()
    {
        AvoidObstacle();

        if (!avoidObstacle)
        {
            timeSinceLastAvoid += Time.deltaTime;

            // Unlock after delay
            if (directionLocked && timeSinceLastAvoid >= directionLockDuration)
            {
                directionLocked = false;
                lockedIncAngle_deg = 0f;
                timeSinceLastAvoid = 0f;
            }
        }
        else
        {
            timeSinceLastAvoid = 0f;
        }
    }

    void AvoidObstacle()
    {
        Ray rayFrontRight = new Ray(transform.position + transform.right * rayOffset, transform.forward);
        Ray rayFrontLeft = new Ray(transform.position - transform.right * rayOffset, transform.forward);
        
        // Check both front rays and also capture hit info
        bool isObstacleInFront = CheckRay(rayFrontRight, avoidDistanceFront) || CheckRay(rayFrontLeft, avoidDistanceFront);

        Vector3 initialDirection = Vector3.forward;

        if (isObstacleInFront && !avoidObstacle)
        {
            avoidObstacle = true;

            // If the obstacle is tagged "Wall", reverse direction
            if (hitObject != null && hitObject.CompareTag("Border"))
            {
                lockedIncAngle_deg = -lockedIncAngle_deg != 0 ? -lockedIncAngle_deg : -incAngleMag_deg;
                directionLocked = true;
            }
            else if (!directionLocked)
            {
                Ray rayRight = new Ray(transform.position, transform.right);
                Ray rayLeft = new Ray(transform.position, -transform.right);

                float rightDist = GetRayDistance(rayRight);
                float leftDist = GetRayDistance(rayLeft);

                if (rightDist < avoidDistanceSide && leftDist < avoidDistanceSide)
                {
                    avoidObstacle = false;
                }
                else if (rightDist > leftDist)
                {
                    lockedIncAngle_deg = incAngleMag_deg;
                }
                else if (leftDist > rightDist)
                {
                    lockedIncAngle_deg = -incAngleMag_deg;
                }
                else
                {
                    lockedIncAngle_deg = Random.value < 0.5f ? incAngleMag_deg : -incAngleMag_deg;
                }

                directionLocked = true;
            }

            incAngle_deg = lockedIncAngle_deg;
        }

        if (avoidObstacle)
        {
            turnAngle_deg += incAngle_deg;
            transform.Rotate(0, incAngle_deg, 0);

            if (Mathf.Abs(turnAngle_deg) >= Mathf.Abs(avoidanceTurnAngle_deg))
            {
                turnAngle_deg = 0;
                avoidObstacle = false;
            }
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(initialDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            transform.Translate(transform.forward * 8 * Time.deltaTime, Space.World);
        }
    }


    bool CheckRay(Ray ray, float avoidDistance)
    {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit, maxSensingDistance);

        if (hitSomething)
        {
            // Skip objects with tag "Floor" or "Chest"
            if (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Chest"))
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxSensingDistance, Color.green);
                return false; // Skip this object
            }

            hitObject = hit.collider.gameObject;
            if (hitObject.GetComponent<Renderer>() != null)
                hitObject.GetComponent<Renderer>().material.color = Color.red;

            Debug.DrawLine(ray.origin, hit.point, hit.distance < avoidDistance ? Color.red : Color.yellow);

            return hit.distance < avoidDistance;
        }
        else
        {
            if (hitObject != null && hitObject.GetComponent<Renderer>() != null)
                hitObject.GetComponent<Renderer>().material.color = Color.yellow;

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxSensingDistance, Color.green);
            return false;
        }
    }


    float GetRayDistance(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxSensingDistance))
        {
            // Skip objects with tag "Floor" or "Chest"
            if (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Chest"))
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxSensingDistance, Color.green);
                return maxSensingDistance; // Skip this object and return max distance
            }

            Debug.DrawLine(ray.origin, hit.point, Color.cyan);
            return hit.distance;
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxSensingDistance, Color.cyan);
            return maxSensingDistance;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Chest"))
        {
            SceneManager.LoadScene("WinScene"); 
        }
    }
}
