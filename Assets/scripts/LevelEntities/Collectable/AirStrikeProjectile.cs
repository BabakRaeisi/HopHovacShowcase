using UnityEngine;

public class AirStrikeProjectile : Projectile
{
    [SerializeField] protected EffectType HitEffect;
    public override void Initialize(PlayerData player, AbilityPoolManager abilityPoolManager)
    {
        base.Initialize(player, abilityPoolManager);
        
        gameObject.SetActive(false);
    }

    Vector3 offsetVisualizer = new Vector3(0, 2f, 0);
    protected override void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        // Prevent hitting the shooter
        if (collision.transform.CompareTag("Player"))
        {
            abilityPoolManager.GetEffect( VisualizerType, collision.transform.position );
            abilityPoolManager.GetEffect( HitEffect, collision.transform.position + offsetVisualizer);

          SFXSelector.Finish(collision.transform.position);
           

        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
            abilityPoolManager.GetEffect(HitEffect, collision.transform.position + offsetVisualizer);
           SFXSelector.Finish(collision.transform.position);
        }


        Deactivate();


    }

}
