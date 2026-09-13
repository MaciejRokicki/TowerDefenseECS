using TD.Features.Combat.ECS.Systems;
using TD.Features.Health.Systems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace TD.Features.Combat.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EnemyReachedBaseSystem))]
    [UpdateBefore(typeof(HealthSystem))]
    public partial class CombatEntityCommandBufferSystem : EntityCommandBufferSystem
    {
        public unsafe struct Singleton : IComponentData, IECBSingleton
        {
            internal UnsafeList<EntityCommandBuffer>* pendingBuffers;
            internal AllocatorManager.AllocatorHandle allocator;

            public EntityCommandBuffer CreateCommandBuffer(WorldUnmanaged world)
            {
                return EntityCommandBufferSystem.CreateCommandBuffer(ref *pendingBuffers, allocator, world);
            }

            // Required by IECBSingleton
            public void SetPendingBufferList(ref UnsafeList<EntityCommandBuffer> buffers)
            {
                var ptr = UnsafeUtility.AddressOf(ref buffers);
                pendingBuffers = (UnsafeList<EntityCommandBuffer>*)ptr;
            }

            // Required by IECBSingleton
            public void SetAllocator(Allocator allocatorIn)
            {
                allocator = allocatorIn;
            }

            // Required by IECBSingleton
            public void SetAllocator(AllocatorManager.AllocatorHandle allocatorIn)
            {
                allocator = allocatorIn;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            this.RegisterSingleton<Singleton>(ref PendingBuffers, World.Unmanaged);
        }
    }
}