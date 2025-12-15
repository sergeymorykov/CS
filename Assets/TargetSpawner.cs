using System;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public static TargetSpawner Instance;

    public int maxTargets = 20;
    public float spawnInterval = 2f;
    public GameObject targetPrefab;
    
    [Header("Параметры спавна")]
    [Tooltip("Радиус области спавна по горизонтали (X и Z)")]
    public float spawnRadius = 10f;
    [Tooltip("Минимальная высота появления мишеней")]
    public float minHeight = 2f;
    [Tooltip("Максимальная высота появления мишеней")]
    public float maxHeight = 8f;

    private int aliveTargets = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, spawnInterval);
    }

    void TrySpawn()
    {
        if (aliveTargets >= maxTargets)
            return;

        Spawn();
    }

    void Spawn()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("TargetSpawner: targetPrefab не назначен!");
            return;
        }

        // Случайная позиция по горизонтали
        Vector3 pos = UnityEngine.Random.insideUnitSphere * spawnRadius;
        
        // Случайная высота в заданном диапазоне
        pos.y = UnityEngine.Random.Range(minHeight, maxHeight);

        GameObject obj = Instantiate(targetPrefab, pos, Quaternion.identity);

        Target target = obj.GetComponent<Target>();
        if (target != null)
        {
            target.OnDestroyed += OnTargetDestroyed;
            aliveTargets++;
        }
        else
        {
            Debug.LogError("TargetSpawner: У prefab нет компонента Target!");
            Destroy(obj);
        }
    }

    void OnTargetDestroyed(Target t)
    {
        aliveTargets--;
    }
}
