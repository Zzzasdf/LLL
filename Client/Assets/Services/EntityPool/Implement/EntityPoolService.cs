using System.Collections.Generic;
using UnityEngine;

public class EntityPoolService : MonoBehaviour, IEntityPoolService
{
    private Dictionary<EntityPoolType, IEntityPool> entityPools = new Dictionary<EntityPoolType, IEntityPool>();
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public IEntityPool Get(EntityPoolType entityPoolType) => entityPools[entityPoolType];
    Transform IEntityPoolService.Transform() => transform;
    void IEntityPoolService.Add(EntityPoolType entityPoolType, IEntityPool entityPool)
    {
        entityPools.Add(entityPoolType, entityPool);
    }
}
