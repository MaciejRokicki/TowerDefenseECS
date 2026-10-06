using Unity.Entities;

namespace TD.Features.FlowField.ECS.Components
{
    public struct UpdateFlowFieldData : IComponentData
    {
        public bool Bake;
    }
}