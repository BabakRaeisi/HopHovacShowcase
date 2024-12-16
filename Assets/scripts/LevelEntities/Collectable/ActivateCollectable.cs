using UnityEngine;

public class ActivateCollectable : MonoBehaviour
{
    public PlayerData playerData;
    

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
         
            playerData.UseAbility();
        }
    }
}