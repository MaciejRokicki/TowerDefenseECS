using System;
using TD.Features.Health.Components;
using TD.Features.Health.Systems;
using TD.Features.Player.Components;
using Unity.Entities;
using Unity.Scripting.LifecycleManagement;

namespace TD.UI.HUD.ECS.Systems
{
    [UpdateAfter(typeof(HealthSystem))]
    public partial class PlayerHealthPresentationSystem : SystemBase
    {
        [AutoStaticsCleanup]
        public static HealthComponent Health { get; private set; }

        [NoAutoStaticsCleanup]
        public static event Action<float, float> OnHealthChanged;
        [NoAutoStaticsCleanup]
        public static event Action<float, float> OnMaxHealthChanged;

        private Entity playerEntity;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerSingleton>();

            OnHealthChanged = delegate { };
            OnMaxHealthChanged = delegate { };
        }

        protected override void OnStartRunning()
        {
            playerEntity = SystemAPI.GetSingletonEntity<PlayerSingleton>();

            Health = SystemAPI.GetComponent<HealthComponent>(playerEntity);

            OnMaxHealthChanged(0.0f, Health.MaxValue);
            OnHealthChanged(0.0f, Health.Value);
        }

        protected override void OnUpdate()
        {
            bool updateHealthComponent = false;
            var currentHealth = SystemAPI.GetComponent<HealthComponent>(playerEntity);

            if (currentHealth.Value != Health.Value)
            {
                updateHealthComponent = true;
                OnHealthChanged(Health.Value, currentHealth.Value);
            }

            if (currentHealth.MaxValue != Health.MaxValue)
            {
                updateHealthComponent = true;
                OnMaxHealthChanged(Health.MaxValue, currentHealth.MaxValue);
            }

            if (updateHealthComponent)
            {
                Health = currentHealth;
            }
        }
    }
}
