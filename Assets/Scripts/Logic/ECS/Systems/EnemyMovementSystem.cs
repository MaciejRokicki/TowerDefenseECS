using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Enemy;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Logic.ECS.Systems
{
    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float3 TargetPosition;
        public float Time;

        void Execute(
            in MovementSpeed movementSpeed,
            ref LocalTransform transform)
        {
            var position = transform.Position;
            var direction = math.normalize(TargetPosition - transform.Position);
            position += direction * movementSpeed.Speed * Time;
            position.z = position.y;
            transform.Position = position;
        }
    }

    public partial struct EnemyMovementSystem : ISystem
    {
        private EntityQuery enemyQuery;
        private float3 basePosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<EnemyTag, MovementSpeed, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            state.RequireForUpdate<BaseTag>();
            basePosition = state.EntityManager.GetComponentData<LocalTransform>(SystemAPI.GetSingletonEntity<BaseTag>()).Position;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob()
            {
                TargetPosition = basePosition,
                Time = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(enemyQuery);
        }
    }
}