using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileCollectable : Collectable  , IActivatable
{
   
    
    public GameObject LaunchVFX;      // For when it's launched
     
   


    private float basespeed = 20f;
    private float speed;
    public  bool isLaunched;
    private Rigidbody rb;
    private PlayerData Ownerplayer;
    private Vector3 Direction;

    public Transform playerTransform;

    private Vector3 launchOffset = new Vector3(0, 0.4f, 0);
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        rb.velocity = Vector3.zero;  
        rb.angularVelocity = Vector3.zero;
    }

    public override void Collect(PlayerData player)
    {
     
        // replace current collectable if the is 
        if (!player.hasMissile)
        {
            despawnTimer = 15f;
            player.AddMissile();
            transform.SetParent(player.transform);
            transform.localPosition = new Vector3(0, 3f, 0);

           
            Ownerplayer = player; // used when is shot
            Ownerplayer.ActiveCollectable = this;
            playerTransform = player.transform;
            rb.velocity = Vector3.zero; // Reset Rigidbody velocity
            rb.angularVelocity = Vector3.zero;
            CollectedVFX.SetActive(true);
        }
        else { Despawn(); }

    }
    public void Activate(Vector3 Direction)
    {
        Ownerplayer.
        StopAllCoroutines();
        despawnTimer = 5f;
        transform.position = playerTransform.position+launchOffset;
        
        isLaunched = true;
        rb.velocity = Vector3.zero; // Reset Rigidbody velocity
        rb.angularVelocity = Vector3.zero;
        speed = basespeed;
        transform.SetParent(null);

        this.Direction = Direction;
        LaunchVFX.SetActive(true);
        StartCoroutine(MoveForward());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isLaunched) 
        {
            rb.velocity = Vector3.zero;  
            rb.angularVelocity = Vector3.zero;
            return;
        }  

        // Check if the collided object is the owner player
        if (collision.gameObject == Ownerplayer.gameObject)
        {
            return; // Ignore collision with the owner
        }

        // Handle collision with other objects or players
        if (collision.gameObject.CompareTag("Player"))
        {
           
            PlayerData hitPlayer = collision.gameObject.GetComponent<PlayerData>();
            if (hitPlayer != Ownerplayer)
            {
                isLaunched = false;
                Debug.Log($"Projectile hit player: {hitPlayer.name}");
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                hitPlayer.HitDisable();
                ResetVFX();
                Despawn();
            }
        }

     
    }
   
    protected override void ResetVFX()
    {
        base.ResetVFX();
        LaunchVFX.SetActive(false );
    }

    private IEnumerator MoveForward()
    {
        while (isLaunched)
        {
            transform.position += Direction * speed * Time.deltaTime; // Use stored direction
            yield return null;
        }
    }

   
}
