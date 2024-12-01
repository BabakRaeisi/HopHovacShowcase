using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Collectable : MonoBehaviour  
{
    private Node assignedNode;
    protected CollectablePoolManager poolManager;
    protected float lifetime = 10f;
    protected float despawnTimer;
    private float rotationSpeed = 100f;

    [Header("VFX")]
    public GameObject SpawnVFX;         // For when it's uncollected
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
    }

    protected virtual void Update()
    {
        despawnTimer -= Time.deltaTime;
       /* RotateCollectable();*/

        if (despawnTimer <= 0)
        {
          PlayVFX(DespawnUncollectedVFX);
            PlayVFX(DespawnUncollectedVFX);
            StartCoroutine(DespawnAfterDelay());
        }
    }

    public virtual void Collect(PlayerData player)
    {
        Despawn();
    }
    protected virtual IEnumerator DespawnAfterDelay() 
    {

        yield return new WaitForSeconds(.50f);
        Despawn();
         
         
    }

    protected virtual void Despawn()
    {
      
      assignedNode?.ClearCollectable();
        poolManager.ReturnCollectable(this);
       
    }

   /* protected void RotateCollectable()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }*/
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