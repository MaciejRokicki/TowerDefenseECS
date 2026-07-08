using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct UnitSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rng = new Unity.Mathematics.Random(92354145);

        foreach (var spawner in SystemAPI.Query<RefRO<UnitSpawner>>())
        {
            for (int i = 0; i < spawner.ValueRO.Amount; i++)
            {
                var entity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
                state.EntityManager.SetComponentData(entity,
                    LocalTransform.FromPosition(
                        rng.NextFloat3(
                            new Unity.Mathematics.float3(spawner.ValueRO.Min),
                            new Unity.Mathematics.float3(spawner.ValueRO.Max)
                        )
                    )
                );
            }
        }

        var systemHandle = World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingUnmanagedSystem<UnitSpawnerSystem>();
        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(systemHandle).Enabled = false;
    }
}
