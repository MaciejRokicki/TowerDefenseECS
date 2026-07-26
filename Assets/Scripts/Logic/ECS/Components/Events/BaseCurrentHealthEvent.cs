using Unity.Entities;

namespace TD.Logic.ECS.Components.Events
{
    public struct BaseCurrentHealthEvent : IComponentData
    {
        public float Value;
    }
}