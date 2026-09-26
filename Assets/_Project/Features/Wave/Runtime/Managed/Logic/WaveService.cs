using System;
using TD.Core.Input.ActionMaps;
using TD.Features.Wave.ECS.Components;
using TD.Features.Wave.Managed.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using VContainer.Unity;

namespace TD.Features.Wave.Managed.Logic
{
    public class WaveService : IInitializable, ITickable, IDisposable
    {
        private readonly StartWaveInputActionMap startWaveInputActionMap;

        public WaveData Data { get; private set; }

        public event Action OnWaveStarted;
        public event Action OnBreakStarted;

        public WaveService(StartWaveInputActionMap startWaveInputActionMap)
        {
            this.startWaveInputActionMap = startWaveInputActionMap;
        }

        public void Initialize()
        {
            Data = new WaveData()
            {
                Wave = 0,
                Power = 10,
                State = WaveState.WaitingForStart,
                WaveTime = 0.0f,
                WaveDuration = 20.0f,
                BreakTime = 0.0f,
                BreakDuration = 10.0f
            };

            OnWaveStarted = delegate { };
            OnBreakStarted = delegate { };

            startWaveInputActionMap.OnStartWavePressed += StartWaveInputActionMap_OnStartWavePressed;
        }

        public void Tick()
        {
            switch (Data.State)
            {
                case WaveState.Running:
                    HandleRunningState();
                    break;
                case WaveState.Break:
                    HandleBreakState();
                    break;
            }
        }

        public void Dispose()
        {
            OnWaveStarted = null;
            OnBreakStarted = null;

            startWaveInputActionMap.OnStartWavePressed -= StartWaveInputActionMap_OnStartWavePressed;
        }

        private void InvokeStartWaveEvent()
        {
            OnWaveStarted();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var startWaveEventEntity = ecb.CreateEntity();
            ecb.AddComponent(startWaveEventEntity, new StartWaveEvent()
            {
                Wave = Data.Wave,
                Power = Data.Power
            });
            ecb.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
            ecb.Dispose();

            var world = World.DefaultGameObjectInjectionWorld;
            var ecbSystem = world.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var lateEcb = ecbSystem.CreateCommandBuffer();
            lateEcb.DestroyEntity(startWaveEventEntity);
        }

        private void InvokeStartBreakEvent()
        {
            OnBreakStarted();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var startBreakEventEntity = ecb.CreateEntity();
            ecb.AddComponent<StartBreakEvent>(startBreakEventEntity);
            ecb.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
            ecb.Dispose();

            var world = World.DefaultGameObjectInjectionWorld;
            var ecbSystem = world.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var lateEcb = ecbSystem.CreateCommandBuffer();
            lateEcb.DestroyEntity(startBreakEventEntity);
        }

        private void StartWave()
        {
            Data.Wave++;
            Data.Power += 1;
            Data.State = WaveState.Running;
            Data.WaveTime = 0.0f;
            Data.BreakTime = 0.0f;

            InvokeStartWaveEvent();
        }

        private void StartBreak()
        {
            Data.BreakTime = 0.0f;
            Data.State = WaveState.Break;

            InvokeStartBreakEvent();
        }

        private void HandleRunningState()
        {
            Data.WaveTime += Time.deltaTime;

            if (Data.WaveTime >= Data.WaveDuration)
            {
                StartBreak();
            }
        }

        private void HandleBreakState()
        {
            Data.BreakTime += Time.deltaTime;

            if (Data.BreakTime >= Data.BreakDuration)
            {
                StartWave();
            }
        }

        private void StartWaveInputActionMap_OnStartWavePressed()
        {
            if (Data.State is WaveState.WaitingForStart or WaveState.Break)
            {
                StartWave();
            }
        }
    }
}
