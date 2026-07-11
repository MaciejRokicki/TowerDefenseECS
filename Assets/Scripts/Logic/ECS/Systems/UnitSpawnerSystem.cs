using TD.Logic.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace TD.Logic.ECS.Systems
{
    public partial struct UnitSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var rng = new Unity.Mathematics.Random(92354145);

            foreach (var (spawner, entity) in SystemAPI.Query<RefRO<UnitSpawner>>().WithEntityAccess())
            {
                for (int i = 0; i < spawner.ValueRO.Amount; i++)
                {
                    var unitEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
                    state.EntityManager.SetComponentData(unitEntity,
                        LocalTransform.FromPosition(
                            rng.NextFloat3(
                                new Unity.Mathematics.float3(spawner.ValueRO.Min),
                                new Unity.Mathematics.float3(spawner.ValueRO.Max)
                            )
                        )
                    );
                }

                ecb.RemoveComponent<UnitSpawner>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
