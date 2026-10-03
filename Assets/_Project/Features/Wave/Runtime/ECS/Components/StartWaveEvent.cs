using Unity.Entities;

namespace TD.Features.Wave.ECS.Components
{
    public struct StartWaveEvent : IComponentData
    {
        public int Wave;
        public int Power;
    }
}
