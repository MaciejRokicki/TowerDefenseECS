//using TD.Logic.ECS.Components.Enemy;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Physics;
//using Unity.Physics.Systems;

//[DisableAutoCreation]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(PhysicsSimulationGroup))] // We are updating after `PhysicsSimulationGroup` - this means that we will get the events of the current frame.
//public partial struct PhysicsTestSystem : ISystem
//{
//    [BurstCompile]
//    public struct CountNumTriggerEvents : ITriggerEventsJob
//    {
//        [ReadOnly]
//        public ComponentLookup<EnemyTag> EnemyTag;
//        public NativeList<Entity> Enemies;

//        public void Execute(TriggerEvent collisionEvent)
//        {
//            if (EnemyTag.HasComponent(collisionEvent.EntityA))
//            {
//                Enemies.Add(collisionEvent.EntityA);
//            }

//            if (EnemyTag.HasComponent(collisionEvent.EntityB))
//            {
//                Enemies.Add(collisionEvent.EntityB);
//            }
//        }
//    }

//    private ComponentLookup<EnemyTag> enemyTagLookup;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        enemyTagLookup = state.GetComponentLookup<EnemyTag>(true);
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        var ecb = new EntityCommandBuffer(Allocator.Temp);
//        NativeList<Entity> enemies = new NativeList<Entity>(Allocator.TempJob);
//        enemyTagLookup.Update(ref state);
//        state.Dependency = new CountNumTriggerEvents
//        {
//            EnemyTag = enemyTagLookup,
//            Enemies = enemies,
//        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
//        state.Dependency.Complete();

//        for (int i = 0; i < enemies.Length; i++)
//        {
//            ecb.DestroyEntity(enemies[i]);
//        }

//        ecb.Playback(state.EntityManager);
//        ecb.Dispose();
//        enemies.Dispose();
//    }
//}