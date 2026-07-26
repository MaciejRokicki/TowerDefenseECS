using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Enemy;
using TD.Logic.ECS.Components.Events;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Logic.ECS.Systems
{
    public struct Missile
    {
        public Entity Entity;
        public LocalTransform LocalTransform;
    }

    public struct Enemy
    {
        public Entity Entity;
        public LocalTransform LocalTransform;
    }

    public struct Hit
    {
        public Entity Missile;
        public Entity Enemy;
    }

    [BurstCompile]
    public partial struct GetMissilesJob : IJobEntity
    {
        public NativeArray<Missile> missiles;

        void Execute(
            [EntityIndexInQuery] int entityIndexInQuery,
            in Entity entity,
            in LocalTransform transform)
        {
            missiles[entityIndexInQuery] = new Missile()
            {
                Entity = entity,
                LocalTransform = transform
            };
        }
    }

    [BurstCompile]
    public partial struct GetEnemiesJob : IJobEntity
    {
        public NativeArray<Enemy> enemies;

        void Execute(
            [EntityIndexInQuery] int entityIndexInQuery,
            in Entity entity,
            in LocalTransform transform)
        {
            enemies[entityIndexInQuery] = new Enemy()
            {
                Entity = entity,
                LocalTransform = transform
            };
        }
    }

    [BurstCompile]
    public partial struct GetHitsJob : IJobFor
    {
        [ReadOnly]
        public NativeArray<Missile> missiles;
        [ReadOnly]
        public NativeArray<Enemy> enemies;
        public NativeList<Hit> hits;

        public void Execute(int index)
        {
            var enemy = enemies[index];
            for (int i = 0; i < missiles.Length; i++)
            {
                var missile = missiles[i];
                var enemyPosition = enemy.LocalTransform.Position;
                enemyPosition.z = 0.0f;
                var missilePosition = missile.LocalTransform.Position;
                missilePosition.z = 0.0f;

                var distance = math.lengthsq(enemyPosition - missilePosition);

                if (distance < 1.0f)
                {
                    hits.Add(new Hit()
                    {
                        Missile = missile.Entity,
                        Enemy = enemy.Entity
                    });
                }
            }
        }
    }

    //[DisableAutoCreation]
    public partial struct MissileSystem : ISystem
    {
        private EntityQuery missileQuery;
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            missileQuery = SystemAPI
                .QueryBuilder()
                .WithAll<MissileTag, LocalTransform>()
                .Build();

            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<Missile> missiles = new NativeArray<Missile>(missileQuery.CalculateEntityCount(), Allocator.TempJob);
            NativeArray<Enemy> enemies = new NativeArray<Enemy>(enemyQuery.CalculateEntityCount(), Allocator.TempJob);
            NativeList<Hit> hits = new NativeList<Hit>(Allocator.TempJob);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var handle = new GetMissilesJob()
            {
                missiles = missiles
            }.ScheduleParallel(missileQuery, state.Dependency);
            handle.Complete();

            handle = new GetEnemiesJob()
            {
                enemies = enemies
            }.ScheduleParallel(enemyQuery, state.Dependency);
            handle.Complete();

            handle = new GetHitsJob()
            {
                enemies = enemies,
                missiles = missiles,
                hits = hits
            }.Schedule(enemies.Length, handle);
            handle.Complete();

            for (int i = 0; i < hits.Length; i++)
            {
                var entity = ecb.CreateEntity();
                ecb.AddComponent<KilledEnemiesCountEvent>(entity);
                ecb.DestroyEntity(hits[i].Enemy);
            }

            missiles.Dispose();
            enemies.Dispose();
            hits.Dispose();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}