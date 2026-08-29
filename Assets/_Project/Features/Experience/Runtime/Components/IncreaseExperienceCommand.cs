using Unity.Entities;

namespace TD.Features.Experience.Runtime.Components
{
    public struct IncreaseExperienceCommand : IComponentData
    {
        public int Value;
    }
}