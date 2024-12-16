using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

 
public class AbilityPoolManager : MonoBehaviour
{
    private Dictionary<AbilityType, Queue<Ability>> abilityPools = new Dictionary<AbilityType, Queue<Ability>>();

    [SerializeField] private List<AbilityPrefabMapping> abilityPrefabs;

    private void Start()
    {
        // Initialize pools for each ability type
        foreach (var mapping in abilityPrefabs)
        {
            InitializePool(mapping.abilityType, mapping.prefab, mapping.poolSize);
        }
    }

    private void InitializePool(AbilityType abilityType, GameObject prefab, int poolSize)
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

    public Ability GetAbility(AbilityType abilityType ,PlayerData player )
    {
        if (abilityPools.TryGetValue(abilityType, out Queue<Ability> pool) && pool.Count > 0)
        {
            Ability ability = pool.Dequeue();
            ability.Initialize(player);
  
            return ability;
        }

        Debug.LogWarning($"No available abilities of type {abilityType} in the pool.");
        return null;
    }

    public void ReturnAbility(Ability ability, AbilityType abilityType)
    {
        ability.gameObject.transform.SetParent(this.transform);
        ability.gameObject.SetActive(false);
        abilityPools[abilityType].Enqueue(ability);

    }

    public bool IsAbilityAvailable(AbilityType abilityType)
    {
        return abilityPools.TryGetValue(abilityType, out Queue<Ability> pool) && pool.Count > 0;
    }
}


[System.Serializable]
public struct AbilityPrefabMapping
{
    public AbilityType abilityType;
    public GameObject prefab;
    public int poolSize;
}

public enum AbilityType
{
    Projectile,
    Thunder,
    None
}
