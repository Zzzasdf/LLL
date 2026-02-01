using UnityEngine;

public interface IEntityPoolService
{
    Transform Transform();
    void Add(EntityPoolType entityPoolType, IEntityPool entityPool);
}
