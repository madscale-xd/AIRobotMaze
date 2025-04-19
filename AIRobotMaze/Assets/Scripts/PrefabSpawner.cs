using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;
    public int numberOfPrefabs = 10;
    public float minDistanceBetweenPrefabs = 1.5f;

    [Header("Z Distance Settings")]
    public float minZDistanceBetweenPrefabs = 1.0f; // NEW: minimum z-distance check

    [Header("Z Scale Settings")]
    public Vector2 zScaleRange = new Vector2(0.5f, 1.5f); // Min and Max Z scale

    [Header("Pathfinder Safety Settings")]
    public string purinrinTag = "Pathfinder";
    public float safeRadiusAroundPurinrin = 2.0f;

    private float spawnAreaWidth;
    private float spawnAreaHeight;
    private float spawnAreaDepth;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private Transform[] purinrinObjects;

    void Start()
    {
        purinrinObjects = GetPurinrinObjects();
        CalculateBounds();
        SpawnPrefabs();
    }

    Transform[] GetPurinrinObjects()
    {
        GameObject[] found = GameObject.FindGameObjectsWithTag(purinrinTag);
        List<Transform> transforms = new List<Transform>();
        foreach (GameObject obj in found)
        {
            transforms.Add(obj.transform);
        }
        return transforms.ToArray();
    }

    void CalculateBounds()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            spawnAreaWidth = rend.bounds.size.x;
            spawnAreaHeight = rend.bounds.size.y;
            spawnAreaDepth = rend.bounds.size.z;
            return;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            spawnAreaWidth = col.bounds.size.x;
            spawnAreaHeight = col.bounds.size.y;
            spawnAreaDepth = col.bounds.size.z;
            return;
        }

        spawnAreaWidth = spawnAreaHeight = spawnAreaDepth = 5f;
        Debug.LogWarning("No Renderer or Collider found, using default spawn area.");
    }

    void SpawnPrefabs()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to spawner!");
            return;
        }

        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = 1000;

        while (spawnedCount < numberOfPrefabs && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f),
                Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f),
                Random.Range(-spawnAreaDepth / 2f, spawnAreaDepth / 2f)
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            bool tooClose = false;

            foreach (Vector3 pos in spawnPositions)
            {
                float overallDistance = Vector3.Distance(pos, spawnPosition);
                float zDistance = Mathf.Abs(pos.z - spawnPosition.z);

                if (overallDistance < minDistanceBetweenPrefabs || zDistance < minZDistanceBetweenPrefabs)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                foreach (Transform purinrin in purinrinObjects)
                {
                    if (Vector3.Distance(purinrin.position, spawnPosition) < safeRadiusAroundPurinrin)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose)
            {
                GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(0f, 90f, 0f));

                Vector3 originalScale = instance.transform.localScale;
                float randomZScale = Random.Range(zScaleRange.x, zScaleRange.y);

                instance.transform.localScale = new Vector3(
                    originalScale.x,
                    originalScale.y,
                    randomZScale
                );

                spawnPositions.Add(spawnPosition);
                spawnedCount++;
            }
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Spawn attempt limit reached. Some prefabs may not have been placed.");
        }
    }

    void OnDrawGizmosSelected()
    {
        CalculateBounds();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaWidth, spawnAreaHeight, spawnAreaDepth));

        GameObject[] purins = GameObject.FindGameObjectsWithTag(purinrinTag);
        Gizmos.color = new Color(1f, 0.2f, 0.6f, 0.4f);
        foreach (GameObject purin in purins)
        {
            Gizmos.DrawWireSphere(purin.transform.position, safeRadiusAroundPurinrin);
        }
    }
}
