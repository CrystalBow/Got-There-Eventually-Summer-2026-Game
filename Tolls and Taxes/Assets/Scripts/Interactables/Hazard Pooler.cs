using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawnerPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Hazards hazardPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float hazardLifetime = 4f;

    [Header("Hazard Overrides (Direction & Stats)")]
    [SerializeField] private Vector2 launchVelocity = new Vector2(-5f, 0f);
    [SerializeField] private int damageOverride = 10;
    [SerializeField] private float cooldownOverride = 1f;

    // Upgraded to a List so we can easily search for inactive objects
    private List<Hazards> pool = new List<Hazards>();

    private void Awake()
    {
        // Pre-warm the pool at the very start
        for (int i = 0; i < poolSize; i++)
        {
            Hazards newHazard = Instantiate(hazardPrefab, transform.position, Quaternion.identity, transform);
            newHazard.gameObject.SetActive(false);
            pool.Add(newHazard);
        }
    }

    private void OnEnable()
    {
        // Spawner starts shooting whenever it gets turned on (like returning from combat!)
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        // Clean up coroutines safely when entering combat
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnHazard();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnHazard()
    {
        // Find the first available inactive hazard in the pool
        Hazards hazard = null;
        foreach (Hazards h in pool)
        {
            if (!h.gameObject.activeSelf)
            {
                hazard = h;
                break;
            }
        }

        // Apply Inspector settings and shoot it
        if (hazard != null)
        {
            hazard.Speed = launchVelocity;
            hazard.Damage = damageOverride;
            hazard.cooldown = cooldownOverride;
            hazard.maxLifetime = hazardLifetime; // Pass the lifetime data to the laser

            hazard.transform.position = transform.position;
            hazard.gameObject.SetActive(true);
        }
    }
}