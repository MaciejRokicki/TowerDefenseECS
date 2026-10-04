using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Shared;
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
        public float Response;
        public float Time;
        public float3 TargetPosition;
        [ReadOnly]
        public FlowFieldSurfaceData FlowFieldSurfaceData;

        void Execute(
            in MovementSpeed movementSpeed,
            ref Velocity velocity,
            ref LocalTransform transform)
        {
            var position = transform.Position;
            FlowFieldUtility.WorldToGridPosition(
                new float3(position.x, position.y, 0.0f),
                new float3(FlowFieldSurfaceData.WorldPosition.x, FlowFieldSurfaceData.WorldPosition.y, 0.0f),
                FlowFieldSurfaceData.CellSize,
                out int2 gridPosition);
            var direction = FlowFieldSurfaceData.Cells[gridPosition.x * FlowFieldSurfaceData.Size.y + gridPosition.y].Direction;
            direction = math.normalizesafe(direction);
            velocity.Target = new float3(direction * movementSpeed.Speed);
            float alpha = 1.0f - math.exp(-Response * Time);
            velocity.Current = math.lerp(velocity.Current, velocity.Target, alpha);
            var pos = transform.Position;
            pos += velocity.Current * Time;
            pos.z = transform.Position.y;
            transform.Position = pos;
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
                Response = 10.0f,
                Time = SystemAPI.Time.DeltaTime,
                TargetPosition = basePosition,
                FlowFieldSurfaceData = flowFieldSurfaceData
            }.ScheduleParallel(enemyQuery);
        }
    }
}