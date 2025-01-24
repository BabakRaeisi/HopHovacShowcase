using System.Collections;
using UnityEngine;

public class AirstrikeEffect : TopDownEffect
{
    public override void Initialize(AbilityPoolManager manager)
    {
        base.Initialize(manager);
 
    }
    public override void Activate(Vector3 position)
    {
        base.Activate(position);
      
        StartCoroutine(ContinuouslyDisablePlayersInRadius());
    }
    protected override void Effect() 
    {
    base.Effect();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider belongs to a player by tag
            if (hitCollider.CompareTag("Player"))
            {
                PlayerData player = hitCollider.GetComponent<PlayerData>();
                if (player != null)
                {
                    player.HitDisable(HitTypeEffect); // Call the disable method


                }
            }
        }
       
    }
}
