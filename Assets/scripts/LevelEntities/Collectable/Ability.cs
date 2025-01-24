using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPatterns;

public abstract class Ability : MonoBehaviour
{
     
    
    protected Vector3 direction;
    protected bool isActive = false;
    protected float currentLifetime;
    [SerializeField]protected AbilityType abilityType;
    [SerializeField] protected EffectType VisualizerType;
    protected PlayerData playerData;
    protected AbilityPoolManager abilityPoolManager;
      protected SFXSelector SFXSelector;
    public AbilityType AbilityType { get { return abilityType; } }
    

    public virtual void Initialize(PlayerData player,AbilityPoolManager abilityPoolManager)
    {
        playerData = player;
        this.abilityPoolManager = abilityPoolManager;
        SFXSelector = GetComponent<SFXSelector>();
    }
    

 
    protected virtual  void Update()
    {
        
    }
    
    public virtual void Activate()
    {
        gameObject.transform.SetParent(null);
    }

}
