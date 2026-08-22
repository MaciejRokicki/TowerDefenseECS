using System;
using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Events;
using Unity.Entities;
using Unity.Scripting.LifecycleManagement;

namespace TD.Logic.ECS.Systems
{
    public partial class EnemyStatisticsSystem : SystemBase
    {
        [NoAutoStaticsCleanup]
        public static event Action<int, int> OnKilledEnemiesCountChanged;
        [NoAutoStaticsCleanup]
        public static event Action<int, int> OnEnemiesCountChanged;
        [NoAutoStaticsCleanup]
        public static event Action<int, int> OnTotalEnemiesCountChanged;

        protected override void OnCreate()
        {
            RequireForUpdate<EnemyStatisticsSingleton>();

            OnKilledEnemiesCountChanged = delegate { };
            OnEnemiesCountChanged = delegate { };
            OnTotalEnemiesCountChanged = delegate { };
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var enemyStatisticsEntity = SystemAPI.GetSingletonEntity<EnemyStatisticsSingleton>();
            var enemyStatistics = SystemAPI.GetSingleton<EnemyStatisticsSingleton>();

            int killedEnemies = 0;
            int totalEnemiesCount = 0;

            foreach (var (killedEnemiesCountEvent, entity) in SystemAPI.Query<RefRO<KilledEnemiesCountEvent>>().WithEntityAccess())
            {
                killedEnemies++;
                ecb.DestroyEntity(entity);
            }

            foreach (var (totalEnemiesCountEvent, entity) in SystemAPI.Query<RefRO<TotalEnemiesCountEvent>>().WithEntityAccess())
            {
                totalEnemiesCount++;
                ecb.DestroyEntity(entity);
            }

            if (killedEnemies != 0)
            {
                int newValue = enemyStatistics.KilledEnemiesCount + killedEnemies;
                OnKilledEnemiesCountChanged(enemyStatistics.KilledEnemiesCount, newValue);
                enemyStatistics.KilledEnemiesCount = newValue;

                newValue = enemyStatistics.TotalEnemiesCount - enemyStatistics.KilledEnemiesCount;
                enemyStatistics.EnemiesCount = newValue;
                OnEnemiesCountChanged(enemyStatistics.EnemiesCount, newValue);

                ecb.SetComponent(enemyStatisticsEntity, enemyStatistics);
            }

            if (totalEnemiesCount != 0)
            {
                int newValue = enemyStatistics.TotalEnemiesCount + totalEnemiesCount;
                OnTotalEnemiesCountChanged(enemyStatistics.TotalEnemiesCount, newValue);
                enemyStatistics.TotalEnemiesCount = newValue;

                newValue = enemyStatistics.TotalEnemiesCount - enemyStatistics.KilledEnemiesCount;
                enemyStatistics.EnemiesCount = newValue;
                OnEnemiesCountChanged(enemyStatistics.EnemiesCount, newValue);

                ecb.SetComponent(enemyStatisticsEntity, enemyStatistics);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnDestroy()
        {
            OnKilledEnemiesCountChanged = null;
            OnEnemiesCountChanged = null;
            OnTotalEnemiesCountChanged = null;
        }
    }
}