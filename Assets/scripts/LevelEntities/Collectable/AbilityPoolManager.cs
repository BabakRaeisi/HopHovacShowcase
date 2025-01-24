using System.Collections.Generic;
using UnityEngine;

public class AbilityPoolManager : MonoBehaviour
{
    private Dictionary<AbilityType, Queue<Ability>> abilityPools = new Dictionary<AbilityType, Queue<Ability>>();
    private Dictionary<EffectType, Queue<GameObject>> effectPools = new Dictionary<EffectType, Queue<GameObject>>();

    [SerializeField] private List<AbilityPrefabMapping> abilityPrefabs;
    [SerializeField] private List<EffectPrefabMapping> effectPrefabs;

    public static AbilityPoolManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        // Initialize pools for abilities and effects
        foreach (var mapping in abilityPrefabs)
        {
            InitializeAbilityPool(mapping.abilityType, mapping.prefab, mapping.poolSize);
        }

        foreach (var mapping in effectPrefabs)
        {
         
            InitializeEffectPool(mapping.effectType, mapping.prefab, mapping.poolSize);
        }
    }

    private void InitializeAbilityPool(AbilityType abilityType, GameObject prefab, int poolSize)
    {
        Queue<Ability> pool = new Queue<Ability>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(prefab, transform);
            Ability ability = instance.GetComponent<Ability>();
            ability.gameObject.SetActive(false);
            pool.Enqueue(ability);
        }

        abilityPools[abilityType] = pool;
    }

    private void InitializeEffectPool(EffectType effectType, GameObject prefab, int poolSize)
    {
        Queue<GameObject> pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(prefab, transform);
            Effect effect = instance.GetComponent<Effect>();
            if (effect != null)
            {
                effect.Initialize(this); // Pass the pool manager
              
            }
            else
            {
                Debug.LogError($"Prefab for {effectType} does not have an Effect script attached. Check your prefab.");
            }

            instance.SetActive(false);
            pool.Enqueue(instance);
        }

        effectPools[effectType] = pool;
       
    }
    public Ability GetAbility(AbilityType abilityType, PlayerData player)
    {
        if (abilityPools.TryGetValue(abilityType, out Queue<Ability> pool))
        {
          
            if (pool.Count > 0)
            {
                Ability ability = pool.Dequeue();
                if (ability == null)
                {
                    Debug.LogError($"[GetAbility] Dequeued ability is null for {abilityType}. Check initialization.");
                    return null;
                }

                ability.Initialize(player, this);
                
                return ability;
            }
        }

        Debug.LogWarning($"[GetAbility] No available abilities of type {abilityType} in the pool.");
        return null;
    }


    public GameObject GetEffect(EffectType effectType, Vector3 position)
    {
        
        if (effectPools.TryGetValue(effectType, out Queue<GameObject> pool) && pool.Count > 0)
        {
            // Dequeue an effect from the pool
            GameObject effect = pool.Dequeue();

            if (effect == null)
            {
             //   Debug.LogError($"Effect for {effectType} is null. Check your prefab or initialization.");
                return null;
            }

            // Set the effect's position
            effect.transform.position = position;

            // Activate the effect
            effect.SetActive(true);

            // Activate the Effect script
            var pooledEffect = effect.GetComponent<Effect>();
            if (pooledEffect != null)
            {
                pooledEffect.Activate(position);
            }
            else
            {
                Debug.LogWarning($"Effect of type {effectType} does not have an Effect script attached.");
            }

            return effect;
        }

        Debug.LogWarning($"No available effects of type {effectType} in the pool.");
        return null;
    }


    public void ReturnAbility(Ability ability, AbilityType abilityType)
    {
        if (abilityPools.TryGetValue(abilityType, out Queue<Ability> pool))
        {
         
            ability.gameObject.transform.SetParent(this.transform);
            ability.gameObject.SetActive(false);
            pool.Enqueue(ability);
           
        }
        else
        {
            Debug.LogError($"[ReturnAbility] Ability type '{abilityType}' not found in the pool. Cannot return ability.");
        }
    }

    public void ReturnEffect(GameObject effect, EffectType effectType)
    {
        if (effectPools.TryGetValue(effectType, out Queue<GameObject> pool))
        {
           
            effect.transform.SetParent(this.transform);
            effect.SetActive(false);
            pool.Enqueue(effect);
        
        }
        else
        {
            Debug.LogError($"Effect type '{effectType}' not found in the pool. Ensure it is initialized in InitializeEffectPool.");
        }
    }

    public bool IsAbilityAvailable(AbilityType abilityType)
    {
        return abilityPools.TryGetValue(abilityType, out Queue<Ability> pool) && pool.Count > 0;
    }

    public bool IsEffectAvailable(EffectType effectType)
    {
        return effectPools.TryGetValue(effectType, out Queue<GameObject> pool) && pool.Count > 0;
    }
}

[System.Serializable]
public struct AbilityPrefabMapping
{
    public AbilityType abilityType;
    public GameObject prefab;
    public int poolSize;
}

[System.Serializable]
public struct EffectPrefabMapping
{
    public EffectType effectType;
    public GameObject prefab;
    public int poolSize;
}

public enum AbilityType
{
    Projectile,
    AirStrikeProjectile,
    Thunder,
    Shield,
    None
}

public enum EffectType
{
    AirStrikeEffect,
    AirStrikeHitEffect,
    ThunderEffect,
    ThunderHitEffect, 
    ShieldEffect,
    FireEffect,
    PointEffect,
    None
}
