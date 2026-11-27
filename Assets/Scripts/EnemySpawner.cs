using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARAnchorManager anchorManager;

    [Header("Spawn Settings")]
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 6f;
    [SerializeField] private float spawnRadius = 3f;

    [Header("Spawn Timing")]
    [SerializeField] private float initialDelay = 3f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 5;

    private int currentEnemies = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (currentEnemies < maxEnemies && !isSpawning)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    async void SpawnEnemy()
    {
        if (arCamera == null || anchorManager == null ||
            enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("Missing camera, anchor manager or prefabs!");
            return;
        }

        isSpawning = true;

        try
        {
            GameObject prefab =
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            float distance = Random.Range(minDistance, maxDistance);
            float horizontal = Random.Range(-spawnRadius, spawnRadius);
            float vertical = Random.Range(-1f, 1f);

            Vector3 spawnPosition =
                arCamera.transform.position +
                arCamera.transform.forward * distance +
                arCamera.transform.right * horizontal +
                arCamera.transform.up * vertical;

            Pose spawnPose = new Pose(
                spawnPosition,
                Quaternion.identity
            );

            var result = await anchorManager.TryAddAnchorAsync(spawnPose);

            if (!result.status.IsSuccess() || result.value == null)
            {
                Debug.LogWarning("Failed to create AR Anchor: " + result.status);
                return;
            }

            ARAnchor anchor = result.value;

            GameObject enemy = Instantiate(prefab, anchor.transform);

            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localRotation = Quaternion.identity;

            currentEnemies++;

            Debug.Log("Enemy spawned at " + enemy.transform.position);
            StartCoroutine(LogEnemyPositionLater(enemy, 5f));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Enemy spawning error: " + e.Message);
        }
        finally
        {
            isSpawning = false;
        }
    }

    IEnumerator LogEnemyPositionLater(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null)
            Debug.Log("Enemy position after " + delay + "s: " + enemy.transform.position);
    }

    public void EnemyDestroyed()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }
}
