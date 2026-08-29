using TD.Features.Base.Runtime.Components;
using TD.Features.Health.Runtime.Components;
using Unity.Entities;
using Unity.Scripting.LifecycleManagement;

namespace TD.Features.Base.Runtime.Systems
{
    public partial class PlayerHealthSystem : SystemBase
    {
        [AutoStaticsCleanup]
        public static HealthComponent Health { get; private set; }

        protected override void OnUpdate()
        {
            foreach (var health in SystemAPI.Query<RefRO<HealthComponent>>().WithAny<BaseSingleton>())
            {
                Health = health.ValueRO;
            }
        }
    }
}
