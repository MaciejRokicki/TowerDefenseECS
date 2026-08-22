using System;
using TD.Logic.ECS.Components.Events;
using Unity.Entities;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Logic.ECS.Systems
{
    public partial class ExperienceSystem : SystemBase
    {
        [AutoStaticsCleanup]
        public static int Experience { get; private set; }
        [AutoStaticsCleanup]
        public static int MaxExperience { get; private set; }

        [NoAutoStaticsCleanup]
        public static event Action<int, int> OnExperienceChanged;

        protected override void OnCreate()
        {
            RequireForUpdate<ExperienceEvent>();

            OnExperienceChanged = delegate { };
        }

        protected override void OnStartRunning()
        {
            MaxExperience = 1_000;

            OnExperienceChanged(0, Experience);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            int currentExperienceDelta = 0;

            foreach (var (experienceEvent, entity) in SystemAPI.Query<RefRO<ExperienceEvent>>().WithEntityAccess())
            {
                currentExperienceDelta += experienceEvent.ValueRO.Value;
                ecb.DestroyEntity(entity);
            }

            if (currentExperienceDelta != 0)
            {
                int newValue = Experience + currentExperienceDelta;
                int levels = newValue / MaxExperience;

                if (levels > 0)
                {
                    Debug.Log("LEVEL");
                    newValue %= MaxExperience;
                }

                OnExperienceChanged(Experience, newValue);
                Experience = newValue;
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnDestroy()
        {
            OnExperienceChanged = null;
        }
    }
}