using UnityEngine;

public class ThunderCollectable : Collectable
{
    public AbilityType abilityType;
    public override void Collect(PlayerData player)
    {

        player.AddAbility(abilityType);
        Despawn();
    }
}
