using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Managed;
using TD.Features.Movement.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.Movement.Systems
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
            float response = 10.0f;

            var position = transform.Position;
            FlowFieldUtility.WorldToGridPosition(
                new float2(position.x, position.y),
                new float2(FlowFieldSurfaceData.Position.x, FlowFieldSurfaceData.Position.y),
                FlowFieldSurfaceData.CellSize,
                out int2 gridPosition);
            var direction = FlowFieldSurfaceData.Directions[gridPosition.x * FlowFieldSurfaceData.Size.y + gridPosition.y];
            direction = math.normalizesafe(direction);
            velocity.Target = new float3(direction * movementSpeed.Speed, 0.0f);
            float alpha = 1.0f - math.exp(-response * Time);
            velocity.Current = math.lerp(velocity.Current, velocity.Target, alpha);
            transform.Position += velocity.Current * Time;
        }
    }

    public partial struct EnemyMovementSystem : ISystem
    {
        private EntityQuery enemyQuery;
        private float3 basePosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementSpeed>();
            state.RequireForUpdate<FlowFieldSurfaceData>();

            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<Velocity, MovementSpeed, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            basePosition = SystemAPI.GetSingleton<FlowFieldSurfaceData>().TargetWorldPosition;
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