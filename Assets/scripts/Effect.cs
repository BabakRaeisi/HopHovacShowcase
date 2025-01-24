using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    [SerializeField] protected float duration = 2f; // Default duration for how long the effect lasts
    [SerializeField]protected EffectType HitTypeEffect;
    [SerializeField]protected EffectType ReturnEffectType;
    protected AbilityPoolManager poolManager;

    /// <summary>
    /// Sets the pool manager reference for returning the effect to the pool.
    /// </summary>
    public virtual void Initialize(AbilityPoolManager manager)
    {
        poolManager = manager;
       
    }

    /// <summary>
    /// Activates the effect at the specified position and rotation.
    /// </summary>
    public virtual void Activate(Vector3 position )
    {
        // Set the position and rotation
        transform.position = position;
       

        // Enable the effect
        gameObject.SetActive(true);

        // Schedule deactivation
        Invoke(nameof(Deactivate), duration);
    }

    /// <summary>
    /// Deactivates the effect, preparing it for pooling.
    /// </summary>
    public virtual void Deactivate()
    {
        CancelInvoke(nameof(Deactivate)); // Cancel any pending Invoke
        gameObject.SetActive(false);

        if (poolManager != null)
        {
     
            poolManager.ReturnEffect(gameObject,ReturnEffectType);
        }
        else
        {
            Debug.LogWarning("PoolManager reference is missing. Cannot return the effect to the pool.");
        }
    }

    /// <summary>
    /// Set the duration for this effect.
    /// </summary>
    /// <param name="newDuration">The new duration in seconds.</param>
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    /// <summary>
    /// Draws gizmos in the editor for debugging purposes.
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        // Override in derived classes for specific visualization (e.g., radius)
    }
}
