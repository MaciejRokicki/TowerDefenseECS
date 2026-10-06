using Unity.Burst;
using Unity.Entities;

namespace TD.Event
{
    public partial struct EventCleanupSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EventTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (eventTag, eventEntity) in SystemAPI.Query<EventTag>().WithEntityAccess())
            {
                ecb.DestroyEntity(eventEntity);
            }
        }
    }
}
