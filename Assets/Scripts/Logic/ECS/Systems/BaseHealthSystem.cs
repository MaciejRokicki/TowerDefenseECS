using System;
using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Events;
using Unity.Entities;
using Unity.Scripting.LifecycleManagement;

namespace TD.Logic.ECS.Systems
{
    public partial class BaseHealthSystem : SystemBase
    {
        [AutoStaticsCleanup]
        public static Health Health { get; private set; }

        [NoAutoStaticsCleanup]
        public static event Action<float, float> OnHealthChanged;
        [NoAutoStaticsCleanup]
        public static event Action<float, float> OnMaxHealthChanged;

        protected override void OnCreate()
        {
            RequireForUpdate<BaseSingleton>();

            OnHealthChanged = delegate { };
            OnMaxHealthChanged = delegate { };
        }

        protected override void OnStartRunning()
        {
            var baseSingletonEntity = SystemAPI.GetSingletonEntity<BaseSingleton>();
            var baseHealth = SystemAPI.GetComponent<Health>(baseSingletonEntity);

            Health = baseHealth;

            OnMaxHealthChanged(0.0f, baseHealth.MaxValue);
            OnHealthChanged(0.0f, baseHealth.Value);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var baseSingletonEntity = SystemAPI.GetSingletonEntity<BaseSingleton>();
            var baseHealth = SystemAPI.GetComponent<Health>(baseSingletonEntity);

            float currentHealthDelta = 0;
            float currentMaxHealthDelta = 0;

            foreach (var (currentHealthEvent, entity) in SystemAPI.Query<RefRO<BaseCurrentHealthEvent>>().WithEntityAccess())
            {
                currentHealthDelta += currentHealthEvent.ValueRO.Value;
                ecb.DestroyEntity(entity);
            }

            foreach (var (maxHealthEvent, entity) in SystemAPI.Query<RefRO<BaseMaxHealthEvent>>().WithEntityAccess())
            {
                currentMaxHealthDelta++;
                ecb.DestroyEntity(entity);
            }

            if (currentMaxHealthDelta != 0)
            {
                float newValue = baseHealth.MaxValue + currentMaxHealthDelta;
                OnMaxHealthChanged(baseHealth.MaxValue, newValue);
                baseHealth.MaxValue = newValue;

                Health = baseHealth;
                ecb.SetComponent(baseSingletonEntity, baseHealth);
            }

            if (currentHealthDelta != 0)
            {
                float newValue = baseHealth.Value + currentHealthDelta;
                OnHealthChanged(baseHealth.Value, newValue);
                baseHealth.Value = newValue;

                Health = baseHealth;
                ecb.SetComponent(baseSingletonEntity, baseHealth);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnDestroy()
        {
            OnMaxHealthChanged = null;
            OnHealthChanged = null;
        }
    }
}