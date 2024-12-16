using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileCollectable : Collectable
{
    public AbilityType abilityType;
    public override void Collect(PlayerData player)
    {

        player.AddAbility(abilityType);
        Despawn();  
    }

     
}