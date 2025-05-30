using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private AICognition aiCognition;
    private AIMovement aiMovement;
    private CollectablePoolManager collectablePoolManager;
    private PlayerData playerData;
    private GridSystem gridSystem;


    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
        aiCognition = GetComponent<AICognition>();
        aiMovement = GetComponent<AIMovement>();
        gridSystem = FindAnyObjectByType<GridSystem>();
        collectablePoolManager = FindAnyObjectByType<CollectablePoolManager>();
    }

    

    public List<Collectable> GetActiveCollectables()
    {
        if (collectablePoolManager == null)
        {
           // Debug.Log("Fetching active collectables...");
            return new List<Collectable>();
        }

        return collectablePoolManager.GetActiveCollectables() ?? new List<Collectable>();
    }
    public GridSystem GetGridSystem() => gridSystem;
    public PlayerData GetPlayerData() => playerData;
    public AICognition GetCognitionSystem()=>aiCognition;

    public void SetTargetNode(Node targetNode) => aiMovement.SetTargetNode(targetNode);

    
    public bool ReachedTarget() => aiMovement.ReachedTarget();
}
