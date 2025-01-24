using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Thunder : Ability
{

    public override void Initialize(PlayerData player, AbilityPoolManager abilityPoolManager)
    {
        base.Initialize(player,abilityPoolManager);
        
        gameObject.SetActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        PlayerData opponent = playerData.OpponentsWithUI
      .OrderByDescending(o => o.Item1.Points) // Use Item1 to access PlayerData
      .Select(o => o.Item1)                  // Extract PlayerData
      .FirstOrDefault();
        if (opponent != null)
        {
            transform.position = opponent.transform.position;
            abilityPoolManager.GetEffect(VisualizerType, transform.position);
            
        }
       Deactivate();
    }
 
    

    private void Deactivate()
    {

        abilityPoolManager.ReturnAbility(this, abilityType);
        isActive = false;
        gameObject.SetActive(false);
    }

}

     
 
