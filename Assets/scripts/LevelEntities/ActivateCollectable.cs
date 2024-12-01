using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCollectable : MonoBehaviour
{
    public PlayerData playerData;
    public Movement movement;
    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
        movement = GetComponent<Movement>();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)&&playerData.hasMissile)
        {
            playerData.ActiveCollectable.Activate(movement.directionVector);
            playerData.hasMissile = false;
        }
    }
}
