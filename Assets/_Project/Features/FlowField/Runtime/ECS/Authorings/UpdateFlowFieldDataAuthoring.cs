using TD.Features.FlowField.Runtime.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.FlowField.Runtime.ECS.Authorings
{
    public class UpdateFlowFieldDataAuthoring : MonoBehaviour
    {
        class Baker : Baker<UpdateFlowFieldDataAuthoring>
        {
            public override void Bake(UpdateFlowFieldDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<UpdateFlowFieldData>(entity);
            }
        }
    }
}