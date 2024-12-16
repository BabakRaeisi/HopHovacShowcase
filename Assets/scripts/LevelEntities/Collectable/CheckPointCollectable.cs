using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCollectable : Collectable 
{

    public override void Collect(PlayerData player)
    {
        base.Collect(player);
      
        player.AddPoints();   
    }
}
