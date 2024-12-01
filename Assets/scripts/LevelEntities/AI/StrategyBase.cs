using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StrategyBase : MonoBehaviour
{
    protected StrategyAvailabilityManager sam;
    protected AIController aiController;
    protected PlayerData playerData;    
 

    public virtual void  Initialize(StrategyAvailabilityManager sam, PlayerData playerData, AIController aiController)
    {
        this.sam = sam;
        this.playerData = playerData;
        this.aiController = aiController;
    }
    protected virtual void Update()
    {
        EvaluateAvailability();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
    public abstract void EvaluateAvailability();


    protected void NotifyAvailability(string strategyName, bool isAvailable)
    {
        if (sam != null)
        {
            sam.SetStrategyAvailability(strategyName, isAvailable);
        }
    }


}
