using System.Collections.Generic;
using UnityEngine;

public class StrategyAvailabilityManager
{
    private float collectableCooldownTimer;
    private float collectableCooldownDuration;
    private Dictionary<string, bool> strategyAvailability = new Dictionary<string, bool>();

    public StrategyAvailabilityManager()
    {
        collectableCooldownTimer = 0f;
        collectableCooldownDuration = Random.Range(1.5f, 3.5f);
        strategyAvailability["CollectableStrategy"] = false;
        strategyAvailability["CompeteStrategy"] = false;
    }

    public void Update()
    {
        if (collectableCooldownTimer > 0)
        {
            collectableCooldownTimer -= Time.deltaTime;
        }
    }

    public bool IsStrategyAvailable(string strategyName)
    {
        if (strategyAvailability.ContainsKey(strategyName))
        {
            if (strategyName == "CollectableStrategy")
            {
                return strategyAvailability[strategyName] && collectableCooldownTimer <= 0;
            }
            return strategyAvailability[strategyName];
        }
        return false;
    }

    public void ResetCollectableCooldown()
    {
        collectableCooldownDuration = Random.Range(1.5f, 3.5f);
        collectableCooldownTimer = collectableCooldownDuration;
    }

    public void SetStrategyAvailability(string strategyName, bool available)
    {
        if (strategyAvailability.ContainsKey(strategyName))
        {
            strategyAvailability[strategyName] = available;
        }
        else
        {
            strategyAvailability.Add(strategyName, available);
        }
    }
}
