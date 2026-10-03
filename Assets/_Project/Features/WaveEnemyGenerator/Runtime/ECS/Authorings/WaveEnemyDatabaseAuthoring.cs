using TD.Features.WaveEnemyGenerator.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.WaveEnemyGenerator.ECS.Authroings
{
    public class WaveEnemyDatabaseAuthoring : MonoBehaviour
    {
        public GameObject[] Enemies;

        class Baker : Baker<WaveEnemyDatabaseAuthoring>
        {
            public override void Bake(WaveEnemyDatabaseAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent<WaveEnemyDatabase>(entity);
                var enemies = AddBuffer<WaveEnemyDatabaseEntryBufferElement>(entity);

                for (int i = 0; i < authoring.Enemies.Length; i++)
                {
                    enemies.Add(new WaveEnemyDatabaseEntryBufferElement()
                    {
                        Entity = GetEntity(authoring.Enemies[i], TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}