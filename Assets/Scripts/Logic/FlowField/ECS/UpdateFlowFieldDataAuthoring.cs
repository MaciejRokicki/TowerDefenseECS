using Unity.Entities;
using UnityEngine;

namespace TD.Logic.FlowField.ECS
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