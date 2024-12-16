using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
     
    
    protected Vector3 direction;
    protected bool isActive = false;
    protected float currentLifetime;
    protected AbilityType abilityType;
    [SerializeField]protected PlayerData player;

    public AbilityType AbilityType { get { return abilityType; } }

    public virtual void Initialize(PlayerData player)
    {
        this.player = player;
    }
    protected virtual void Start()
    {
        
    }

 
    protected virtual  void Update()
    {
        
    }
    public virtual void Activate(Vector3 direction, Vector3 playerTransform)
    {
        gameObject.transform.SetParent(null);
    }

}
