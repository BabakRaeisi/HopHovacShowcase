using System.Collections;
using UnityEngine;

public class Projectile : Ability 
{
   
    [Header("Ability Settings")]
    protected float OriginalSpeed = 20f;
    protected float speed;
    protected float maxLifetime = 5f; 

    [SerializeField]protected  Rigidbody rb;
    [SerializeField] protected  SphereCollider sphereCollider;
    [SerializeField] protected GameObject launchVFX;
    




    public override void Initialize(PlayerData player, AbilityPoolManager abilityPoolManager)
    {
        base.Initialize(player,abilityPoolManager);
       
        gameObject.SetActive(false);
        
       
    }

    public override void Activate( )
    {
        base.Activate();
        StopAllCoroutines();
        SFXSelector.Initialize();
        // Setup projectile properties
        this.direction = playerData.DirectionVec.normalized;
        isActive = true;
        currentLifetime = maxLifetime;
        speed = OriginalSpeed;
        // Position and rotation
        transform.position = playerData.GetFrontGridPosition();
        transform.rotation = Quaternion.LookRotation(direction);

        // Activate launch VFX
        if (launchVFX != null)
            launchVFX.SetActive(true);
      
       
        StartCoroutine(MoveForward());
    }

   
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        // Prevent hitting the shooter
        if (collision.transform.CompareTag("Player"))
        {
            PlayerData hitPlayer = collision.transform.GetComponent<PlayerData>();
            if (hitPlayer != null)
            {
                hitPlayer.HitDisable(VisualizerType);
                SFXSelector.Finish(collision.transform.position);

            }
        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
           SFXSelector.Finish(collision.transform.position);
           abilityPoolManager.GetEffect(VisualizerType, this.transform.position );
        }


        Deactivate();


    }

    protected virtual IEnumerator MoveForward()
    {
        while (isActive)
        {
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }

    protected virtual void Deactivate()
    {
        isActive = false;
        speed = 0;
        abilityPoolManager.ReturnAbility(this, abilityType);
    }

}
