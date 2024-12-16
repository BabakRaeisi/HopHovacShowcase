using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class AICognition : MonoBehaviour
{
    private AIController aiController;
    private StrategyBase activeStrategy;
    public List<StrategyBase> strategies;
    public StrategyAvailabilityManager sam;
    public PlayerData playerData;
    private StrategyEvaluator strategyEvaluator;
    private TextMeshPro stateText;

    private bool isWaitingForReenable;

    private void Start()
    {
        aiController = GetComponent<AIController>();
        sam = new StrategyAvailabilityManager();
        strategyEvaluator = new StrategyEvaluator(sam, aiController, playerData, this);
        stateText = GetComponentInChildren<TextMeshPro>();

        // Initialize strategies
        foreach (var strategy in strategies)
        {
            strategy.Initialize(sam, playerData, aiController);
        }
    }

    private void Update()
    {
        // Check if player is disabled
        if (playerData.Disabled)
        {
            if (!isWaitingForReenable)
            {
                ExitCurrentStrategy();
                isWaitingForReenable = true;
                if (stateText != null)
                {
                    stateText.text = "State: Disabled";
                }
            }
            return; // Do nothing while disabled
        }

        // Re-enable when player is no longer disabled
        if (isWaitingForReenable && !playerData.Disabled)
        {
            isWaitingForReenable = false;
            EvaluateAndSetActiveStrategy(); // Re-evaluate strategy upon re-enabling
        }

        sam.Update();
        EvaluateAndSetActiveStrategy();
        activeStrategy?.Execute();
    }

    public void EvaluateAndSetActiveStrategy()
    {
        StrategyBase bestStrategy = strategyEvaluator.EvaluateBestStrategy();
        if (bestStrategy != null && bestStrategy != activeStrategy)
        {
            SetActiveStrategy(bestStrategy);

            if (stateText != null)
            {
                stateText.text = $"State: {bestStrategy.GetType().Name}";
            }
        }
    }

    private void SetActiveStrategy(StrategyBase newStrategy)
    {
        if (activeStrategy != newStrategy)
        {
            activeStrategy?.Exit();
            activeStrategy = newStrategy;
            activeStrategy.Enter();
        }
    }

    private void ExitCurrentStrategy()
    {
        if (activeStrategy != null)
        {
            activeStrategy.Exit();
            activeStrategy = null;
        }
    }

    public T GetStrategyByType<T>() where T : StrategyBase
    {
        return strategies.OfType<T>().FirstOrDefault();
    }
}
