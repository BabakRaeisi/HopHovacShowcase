using System.Collections;
using UnityEngine;

public abstract class TopDownEffect : Effect
{
    [SerializeField] protected float radius;  // Radius of the disabling effect
    [SerializeField] protected float StartTime;  
    protected SFXSelector sfxSelector;
    protected bool isActive = false;

    public override void Activate(Vector3 position)
    {
        base.Activate(position);
       
        sfxSelector = GetComponent<SFXSelector>();
        isActive = true;
        sfxSelector.Initialize();
      
    }

    protected virtual IEnumerator ContinuouslyDisablePlayersInRadius()
    {
        yield return new WaitForSeconds(StartTime);
        float elapsedTime = 0f;

        while (elapsedTime < duration && isActive)
        {

            Effect();
            elapsedTime += 0.2f; // Check every 0.2 seconds (adjust as needed)
            yield return new WaitForSeconds(0.2f);
        }

        isActive = false;
        Deactivate();
        
    }

    protected virtual void Effect()
    {
      sfxSelector.Finish(  this.transform.position);
    }

    public override void Deactivate()
    {
      
        isActive = false;
        StopAllCoroutines();
        base.Deactivate();
    }

    protected  override void OnDrawGizmosSelected()
    {
        // Optional: Visualize the radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
