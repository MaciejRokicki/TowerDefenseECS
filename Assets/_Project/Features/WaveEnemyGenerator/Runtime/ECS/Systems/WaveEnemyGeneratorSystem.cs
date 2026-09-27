using TD.Features.Random.ECS.Components;
using TD.Features.Wave.ECS.Components;
using TD.Features.WaveEnemyGenerator.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TD.Features.WaveEnemyGenerator.ECS.Systems
{
    partial struct WaveEnemyGeneratorSystem : ISystem
    {
        private ComponentLookup<WaveEnemyData> waveEnemyDataLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomState>();
            state.RequireForUpdate<WaveEnemyDatabase>();
            state.RequireForUpdate<StartWaveEvent>();

            waveEnemyDataLookup = SystemAPI.GetComponentLookup<WaveEnemyData>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var buffer = SystemAPI.GetSingletonBuffer<WaveEnemyDatabaseEntryBufferElement>(true);
            var randomStateEntity = SystemAPI.GetSingletonEntity<RandomState>();
            var randomState = SystemAPI.GetSingleton<RandomState>();
            waveEnemyDataLookup.Update(ref state);
            var data = waveEnemyDataLookup[buffer[0].Entity];

            foreach (var startWaveEvent in SystemAPI.Query<StartWaveEvent>())
            {
                PrepareWave(ref state, ref ecb, ref randomState, startWaveEvent, buffer);
            }

            SystemAPI.SetComponent(randomStateEntity, new RandomState() { Random = randomState.Random });
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        private void PrepareWave(ref SystemState state, ref EntityCommandBuffer ecb, ref RandomState randomState, StartWaveEvent startWaveEvent, DynamicBuffer<WaveEnemyDatabaseEntryBufferElement> waveEnemyDatabase)
        {
            NativeList<Entity> validEnemies = new NativeList<Entity>(Allocator.Temp);
            int totalWeight = 0;

            NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);

            for (int i = 0; i < waveEnemyDatabase.Length; i++)
            {
                var entity = waveEnemyDatabase[i].Entity;
                var waveEnemyData = waveEnemyDataLookup[entity];

                if (startWaveEvent.Wave < waveEnemyData.MinWave)
                    continue;

                validEnemies.Add(entity);
                totalWeight += waveEnemyData.Weight;
            }

            int currentPower = 0;

            while (currentPower < startWaveEvent.Power)
            {
                var powerDelta = startWaveEvent.Power - currentPower;

                for (int i = validEnemies.Length - 1; i > -1; i--)
                {
                    var entity = validEnemies[i];
                    var waveEnemyData = waveEnemyDataLookup[entity];

                    if (powerDelta >= waveEnemyData.Power)
                        continue;

                    totalWeight -= waveEnemyData.Weight;
                    validEnemies.RemoveAt(i);
                }

                int roll = randomState.Random.NextInt(totalWeight);

                for (int i = 0; i < validEnemies.Length; i++)
                {
                    var entity = validEnemies[i];
                    var waveEnemyData = waveEnemyDataLookup[entity];

                    roll -= waveEnemyData.Weight;

                    if (roll < 0)
                    {
                        currentPower += waveEnemyData.Power;
                        enemies.Add(entity);
                        break;
                    }
                }
            }

            var waveEnemyPreparedEventEntity = ecb.CreateEntity();
            ecb.AddComponent(waveEnemyPreparedEventEntity, new WaveEnemyPreparedEvent()
            {
                Enemies = new NativeArray<Entity>(enemies.AsArray(), Allocator.TempJob)
            });

            validEnemies.Dispose();
            enemies.Dispose();
        }
    }
}
