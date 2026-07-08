using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct UnitSpawner : IComponentData
{
    public Entity Prefab;
    public int Amount;
    public float3 Min;
    public float3 Max;
}

public class UnitSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public int Amount;
    public float3 Min;
    public float3 Max;

    class UnitSpawnerBaker : Baker<UnitSpawnerAuthoring>
    {
        public override void Bake(UnitSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitSpawner()
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                Amount = authoring.Amount,
                Min = authoring.Min,
                Max = authoring.Max
            });
        }
    }
}
