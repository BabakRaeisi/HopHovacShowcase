using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Collectable : MonoBehaviour  
{
    private Node assignedNode;
    protected CollectablePoolManager poolManager;
    protected float lifetime = 10f;
    protected float despawnTimer;
 
    [Header("VFX")]
    public GameObject SpawnVFX;        // For when it's uncollected
    public GameObject idleVFX;         // For when it's uncollected
    public GameObject DespawnUncollectedVFX; // For when it times out
    public GameObject CollectedVFX; // For when it times out

    public Node AssignedNode { get => assignedNode; set => assignedNode = value; }
    public Vector2Int GridPosition { get; private set; }

    protected virtual void OnEnable()
    {
        despawnTimer = lifetime;
        PlayVFX(SpawnVFX);
    }
    protected virtual void OnDisable()
    {
        ResetVFX();
    }

    public void Initialize(Node node, CollectablePoolManager poolManager)
    {
        this.poolManager = poolManager;
        this.assignedNode = node;
        GridPosition = node.Coordinates;
        assignedNode.AssignCollectable(this);
        PlayVFX(SpawnVFX);
        PlayVFX(idleVFX);
    }
 
    protected virtual void Update()
    {
        despawnTimer -= Time.deltaTime;
        

        if (despawnTimer <= 0)
        {
          PlayVFX(DespawnUncollectedVFX);
          
           
        }
    }

    public virtual void Collect(PlayerData player)
    {
        PlayVFX(CollectedVFX);
        StartCoroutine(DespawnAfterDelay());
    }
    protected virtual IEnumerator DespawnAfterDelay() 
    {

        yield return new WaitForSeconds(.20f);
        Despawn();
         
         
    }

    protected virtual void Despawn()
    {
      
      assignedNode?.ClearCollectable();
      poolManager.ReturnCollectable(this);
       
    }

 
    protected virtual void PlayVFX(GameObject vfx)
    {
        vfx.SetActive(true);
    }
    protected virtual void ResetVFX() 
    {
    SpawnVFX.SetActive(false );
    idleVFX.SetActive(false );
    CollectedVFX.SetActive(false );
    DespawnUncollectedVFX.SetActive(false );
    }
}
 
public interface IActivatable 
{
   public void Activate(Vector3 direction);  
}