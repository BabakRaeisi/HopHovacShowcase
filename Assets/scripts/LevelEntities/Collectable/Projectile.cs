using System.Collections;
using UnityEngine;

public class Projectile : Ability 
{

    [Header("Ability Settings")]
    public float speed = 20f;
    public float maxLifetime = 5f; 

    public  Rigidbody rb;
    public  SphereCollider sphereCollider;
    public GameObject launchVFX;
 
    public override void Initialize(PlayerData player)
    {
        base.Initialize(player);
        abilityType = AbilityType.Projectile;
        gameObject.SetActive(false);
       
    }

    public override void Activate(Vector3 direction, Vector3 pos)
    {
        base.Activate(direction, pos);
        StopAllCoroutines();

        // Setup projectile properties
        this.direction = direction.normalized;
        isActive = true;
        currentLifetime = maxLifetime;

        // Position and rotation
        transform.position = pos ;
        transform.rotation = Quaternion.LookRotation(direction);

        // Activate launch VFX
        if (launchVFX != null)
            launchVFX.SetActive(true);

        StartCoroutine(MoveForward());
    }

   
    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        // Prevent hitting the shooter
        if (collision.transform.CompareTag("Player"))
        {
            PlayerData hitPlayer = collision.transform.GetComponent<PlayerData>();
            if (hitPlayer != null)
            {
                
                hitPlayer.HitDisable();
            }
        }
        else if (collision.transform.CompareTag("Wall"))
        {
           
        }

 

        Deactivate();
    }

    private IEnumerator MoveForward()
    {
        while (isActive)
        {
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }

    private void Deactivate()
    {
      
        
        player.abilityPoolManager.ReturnAbility(this, AbilityType.Projectile);
        player = null;  
        isActive = false;
        gameObject.SetActive(false);

       
      
    }

}
