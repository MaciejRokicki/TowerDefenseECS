using TD.Features.EnemySpawnerArea.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.EnemySpawnerArea.Authorings
{
    public class EnemySpawnerAreaAuthroing : MonoBehaviour
    {
        public Vector2Int Size;

        class UnitSpawnerBaker : Baker<EnemySpawnerAreaAuthroing>
        {
            public override void Bake(EnemySpawnerAreaAuthroing authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new AddEnemySpawnerAreaCommand()
                {
                    Size = authoring.Size,
                });
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);
            Gizmos.DrawCube(transform.position + new Vector3(Size.x, Size.y, 0.0f) / 2.0f, new Vector3(Size.x, Size.y, 1.0f));
        }
    }
}
