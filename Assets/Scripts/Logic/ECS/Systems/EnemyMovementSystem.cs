using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Enemy;
using TD.Logic.FlowField.ECS;
using Unity.Burst;
using Unity.Collections;
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
        [ReadOnly]
        public FlowFieldSurfaceData FlowFieldSurfaceData;

        void Execute(
            in MovementSpeed movementSpeed,
            ref Velocity velocity,
            ref LocalTransform transform)
        {
            var position = transform.Position;
            //position.z = 0.0f;

            var gridPosition = ToGridPosition(FlowFieldSurfaceData.Position, FlowFieldSurfaceData.CellSize, transform.Position);

            //position += new float3(FlowFieldSurfaceData.Directions[gridPosition.x * FlowFieldSurfaceData.Size.y + gridPosition.y] * movementSpeed.Speed * Time, 0.0f);

            velocity.Target = new float3(FlowFieldSurfaceData.Directions[gridPosition.x * FlowFieldSurfaceData.Size.y + gridPosition.y] * movementSpeed.Speed * Time, 0.0f);
            velocity.Current = math.lerp(velocity.Current, velocity.Target, Time / 2.0f);
            position += velocity.Current;

            position.z = position.y;
            transform.Position = position;
        }

        private int2 ToGridPosition(float3 gridPosition, float cellSize, float3 worldPosition)
        {
            return new int2(
                (int)math.round((worldPosition.x - gridPosition.x - cellSize / 2.0f) / cellSize),
                (int)math.round((worldPosition.y - gridPosition.y - cellSize / 2.0f) / cellSize)
            );
        }
    }

    public partial struct EnemyMovementSystem : ISystem
    {
        private EntityQuery enemyQuery;
        private float3 basePosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BaseSingleton>();
            state.RequireForUpdate<FlowFieldSurfaceData>();

            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<EnemyTag, Velocity, MovementSpeed, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            basePosition = state.EntityManager.GetComponentData<LocalTransform>(SystemAPI.GetSingletonEntity<BaseSingleton>()).Position;
            basePosition.z = 0.0f;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var flowFieldSurfaceData = SystemAPI.GetSingleton<FlowFieldSurfaceData>();

            new MoveJob()
            {
                TargetPosition = basePosition,
                Time = SystemAPI.Time.DeltaTime,
                FlowFieldSurfaceData = flowFieldSurfaceData
            }.ScheduleParallel(enemyQuery);
        }
    }
}