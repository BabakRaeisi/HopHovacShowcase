using UnityEngine;

public class PlayerAbilityActivator : MonoBehaviour
{
    private PlayerData playerData;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
         
            playerData.UseAbility();
        }
    }
}