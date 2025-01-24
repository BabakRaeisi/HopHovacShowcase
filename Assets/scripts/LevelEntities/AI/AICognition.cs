using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AIMovement))]
[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(CollectableStrategy))]
[RequireComponent(typeof(ExploreStrategy))]
[RequireComponent(typeof(CompeteStrategy))]
[RequireComponent(typeof(AIAbilityHandler))]
public class AICognition : MonoBehaviour
{
    private PlayerData playerData;
    private AIController aiController;
    private StrategyBase activeStrategy;
    private StrategyEvaluator strategyEvaluator;

    private List<StrategyBase> strategies = new List<StrategyBase>();
    private StrategyAvailabilityManager sam;

    private bool isWaitingForReenable;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
        aiController = GetComponent<AIController>();

        // Initialize strategies
        strategies.Add(GetComponent<ExploreStrategy>());
        strategies.Add(GetComponent<CollectableStrategy>());
        strategies.Add(GetComponent<CompeteStrategy>());
    }

    private void Start()
    {
        sam = new StrategyAvailabilityManager();
        strategyEvaluator = new StrategyEvaluator(sam, aiController, playerData, this);

        // Initialize each strategy
        foreach (var strategy in strategies)
        {
            strategy.Initialize(sam, playerData, aiController);
        }
    }

    private void OnEnable()
    {
        playerData.OnReset += ResetCognition;
    }

    private void OnDisable()
    {
        playerData.OnReset -= ResetCognition;
    }

    private void ResetCognition()
    {
        ExitCurrentStrategy();
    }

    private void Update()
    {
        if (playerData.State == PlayerState.Disabled)
        {
            HandleDisabledState();
            return;
        }

        if (isWaitingForReenable && playerData.State == PlayerState.Active)
        {
            isWaitingForReenable = false;
            EvaluateAndSetActiveStrategy();
        }

        sam.Update();
        EvaluateAndSetActiveStrategy();
        activeStrategy?.Execute();
    }

    private void HandleDisabledState()
    {
        if (!isWaitingForReenable)
        {
            ExitCurrentStrategy();
            isWaitingForReenable = true;
        }
    }

    public void EvaluateAndSetActiveStrategy()
    {
        StrategyBase bestStrategy = strategyEvaluator.EvaluateBestStrategy();
        if (bestStrategy != null && bestStrategy != activeStrategy)
        {
            SetActiveStrategy(bestStrategy);
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
