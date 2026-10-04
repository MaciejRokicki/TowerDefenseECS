using TD.Features.FlowField.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.FlowField.ECS.Authorings
{
    public class FlowFieldModifierAuthoring : MonoBehaviour
    {
        class Baker : Baker<FlowFieldModifierAuthoring>
        {
            public override void Bake(FlowFieldModifierAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FlowFieldModifierData()
                {
                    WorldPosition = authoring.transform.position,
                    Size = authoring.Size,
                    Cost = authoring.Cost,
                    IsObstacle = authoring.IsObstacle
                });
            }
        }

        public Vector2Int Size;
        public float Cost;
        public bool IsObstacle;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
            Gizmos.DrawCube(transform.position + new Vector3(Size.x, Size.y, 0.0f) / 2.0f, new Vector3(Size.x, Size.y, 1.0f));
        }
    }
}