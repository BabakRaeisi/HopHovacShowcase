using UnityEngine;
using System.Collections.Generic;

public class CollectablePoolManager : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private List<Collectable> collectablePrefabs;

    private List<Collectable> collectablePool = new List<Collectable>();
    private List<Collectable> activeCollectables = new List<Collectable>();

    [SerializeField] private int minCollectablesPerWave = 2;
    [SerializeField] private int maxCollectablesPerWave = 4;
    [SerializeField] private float waveDelay = 6f;

    public static event System.Action<Collectable> OnCollectableSpawned;
    public static event System.Action<Collectable> OnCollectableDespawned;

    private bool isSpawningWave = false;
    private bool isPaused = false;

    public void Initialize()
    {
        foreach (var prefab in collectablePrefabs)
        {
            Collectable newCollectable = Instantiate(prefab, this.transform);
            newCollectable.gameObject.SetActive(false);
            collectablePool.Add(newCollectable);
        }
        ScheduleNextWave();
    }

    private void Update()
    {
        if (isPaused) return;

        // Check if all collectables are returned before starting a new wave
        if (activeCollectables.Count == 0 && !isSpawningWave)
        {
            isSpawningWave = true;
            Invoke(nameof(ScheduleNextWave), waveDelay);
        }
    }

    private void ScheduleNextWave()
    {
        if (isPaused) return;

        int collectablesToSpawn = Random.Range(minCollectablesPerWave, maxCollectablesPerWave + 1);
        for (int i = 0; i < collectablesToSpawn; i++)
        {
            SpawnCollectable();
        }
        isSpawningWave = false;
    }

    private void SpawnCollectable()
    {
        if (isPaused) return;

        Collectable collectable = GetRandomInactiveCollectable();
        if (collectable != null)
        {
            Node randomNode = GetRandomUnoccupiedNode();

            if (randomNode != null)
            {
                collectable.transform.position = gridSystem.GetWorldPositionFromGrid(randomNode.Coordinates);
                collectable.Initialize(randomNode, this);
                randomNode.AssignCollectable(collectable);
                collectable.gameObject.SetActive(true);
                activeCollectables.Add(collectable);

                // Trigger the event for AI agents
                OnCollectableSpawned?.Invoke(collectable);
            }
        }
    }

    private Collectable GetRandomInactiveCollectable()
    {
        List<Collectable> inactiveCollectables = new List<Collectable>();
        foreach (var collectable in collectablePool)
        {
            if (!collectable.gameObject.activeInHierarchy)
            {
                inactiveCollectables.Add(collectable);
            }
        }

        if (inactiveCollectables.Count > 0)
        {
            return inactiveCollectables[Random.Range(0, inactiveCollectables.Count)];
        }
        return null;
    }

    public void ReturnCollectable(Collectable collectable)
    {
        if (collectable != null)
        {
            OnCollectableDespawned?.Invoke(collectable);
            collectable.gameObject.SetActive(false);
            activeCollectables.Remove(collectable);
            collectable.AssignedNode?.ClearCollectable();
            collectable.transform.SetParent(this.transform);
        }
    }

    public void ReclaimAllCollectables()
    {
        foreach (var collectable in new List<Collectable>(activeCollectables))
        {
            ReturnCollectable(collectable);
        }
     
    }

    public void PauseSpawning()
    {
        isPaused = true;
        CancelInvoke(nameof(ScheduleNextWave));
    }

    public void ResumeSpawning()
    {
        isPaused = false;
        if (activeCollectables.Count == 0) ScheduleNextWave();
    }

    private Node GetRandomUnoccupiedNode()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Node randomNode = gridSystem.GetRandomUnoccupiedNode();
            if (randomNode != null && !randomNode.HasCollectable)
            {
                return randomNode;
            }
        }
        return null;
    }

    public List<Collectable> GetActiveCollectables()
    {
        return activeCollectables;
    }
}
