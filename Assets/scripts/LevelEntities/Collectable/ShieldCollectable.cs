using System.Collections;
using System.Linq;
using UnityEngine;

public class ShieldCollectable : Collectable
{
    public override void Collect(PlayerData player)
    {
        base.Collect(player);
        player.SetShield(true);
       
    }

}
