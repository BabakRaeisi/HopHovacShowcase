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

    public T GetStrategyByType<T>() where T : StrategyBase
    {
        return strategies.OfType<T>().FirstOrDefault();
    }
}








