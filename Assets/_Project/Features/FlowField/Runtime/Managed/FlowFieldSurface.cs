using System;
using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    public class FlowFieldSurface : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float cellSize;
        [SerializeField]
        private Vector2Int size;
        [SerializeField]
        private Vector3 targetPosition;
        [SerializeField]
        private FlowFieldData data;

        [Header("Debug")]
        [SerializeField]
        private bool debug;
        [SerializeField]
        private bool drawData;
        [SerializeField]
        private bool drawCost;
        [SerializeField]
        private bool drawEikonal;
        [SerializeField]
        private bool drawHeatmap;
        [SerializeField]
        private bool drawDirection;
        [SerializeField]
        private Transform tester;
        [SerializeField]
        private Vector2Int testerPos;

        [NonSerialized]
        private GUIStyle debugStyle;

        public FlowFieldData Data => data;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            void DrawGrid(Vector3 position, float cellSize, Vector2Int size)
            {
                Vector3 pos;

                for (int i = 0; i < size.x; i++)
                {
                    pos = position + Vector3.right * i * cellSize;
                    Gizmos.DrawLine(pos, pos + Vector3.up * size.y * cellSize);

                    for (int j = 0; j < size.y; j++)
                    {
                        pos = position + Vector3.up * j * cellSize;
                        Gizmos.DrawLine(pos, pos + Vector3.right * size.x * cellSize);
                    }
                }

                pos = position + Vector3.right * size.x * cellSize;
                Gizmos.DrawLine(pos, pos + Vector3.up * size.y * cellSize);
                pos = position + Vector3.up * size.y * cellSize;
                Gizmos.DrawLine(pos, pos + Vector3.right * size.x * cellSize);
            }

            void DrawCosts(FlowFieldData data)
            {
                for (int i = 0; i < data.Size.x; i++)
                {
                    for (int j = 0; j < data.Size.y; j++)
                    {
                        var pos = data.Position + new Vector3(i * data.CellSize, j * data.CellSize, 0.0f) + new Vector3(data.CellSize / 2.0f, data.CellSize / 2.0f, 0.0f);
                        pos.y += 0.25f;
                        UnityEditor.Handles.Label(pos, string.Concat(data.GetValue(i, j).Cost.ToString("0.00"), " (", i, ", ", j, ")"), debugStyle);
                    }
                }
            }

            void DrawEikonal(FlowFieldData data)
            {
                for (int i = 0; i < data.Size.x; i++)
                {
                    for (int j = 0; j < data.Size.y; j++)
                    {
                        var pos = data.Position + new Vector3(i * data.CellSize, j * data.CellSize, 0.0f) + new Vector3(data.CellSize / 2.0f, data.CellSize / 2.0f, 0.0f);
                        pos.y -= 0.25f;
                        UnityEditor.Handles.Label(pos, data.GetValue(i, j).Eikonal.ToString("0.00"), debugStyle);
                    }
                }
            }

            void DrawHeatmap(FlowFieldData data)
            {
                for (int i = 0; i < data.Size.x; i++)
                {
                    for (int j = 0; j < data.Size.y; j++)
                    {
                        var pos = data.Position + new Vector3(i * data.CellSize, j * data.CellSize, 0.0f) + new Vector3(data.CellSize / 2.0f, data.CellSize / 2.0f, 0.0f);
                        Gizmos.color = Color.Lerp(Color.green, Color.red, data.GetValue(i, j).Cost / data.MaxCostValue);
                        var c = Gizmos.color;
                        c.a = 0.75f;
                        Gizmos.color = c;
                        Gizmos.DrawCube(pos, Vector3.one * cellSize);
                    }
                }
            }

            void DrawDirections(FlowFieldData data)
            {
                for (int i = 0; i < data.Size.x; i++)
                {
                    for (int j = 0; j < data.Size.y; j++)
                    {
                        var pos = data.Position + new Vector3(i * data.CellSize, j * data.CellSize, 0.0f) + new Vector3(data.CellSize / 2.0f, data.CellSize / 2.0f, 0.0f);
                        UnityEditor.Handles.DrawLine(pos, pos + data.GetValue(i, j).Direction / 2.0f, 2.0f);
                    }
                }
            }

            void DrawTester(Vector3 position, float cellSize, Vector2Int size)
            {
                if (tester == null)
                    return;

                Gizmos.DrawCube(tester.position, Vector3.one * cellSize);
                testerPos = FlowFieldUtility.WorldToGridPosition(tester.position, position, cellSize);
            }

            if (!debug)
                return;

            if (drawData)
            {
                if (data == null)
                    return;

                if (debugStyle == null)
                {
                    debugStyle = new GUIStyle();
                    debugStyle.normal.textColor = Color.white;
                    debugStyle.fontSize = 16;
                    debugStyle.alignment = TextAnchor.MiddleCenter;
                }

                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);

                DrawGrid(data.Position, data.CellSize, data.Size);

                if (drawCost)
                    DrawCosts(data);

                if (drawEikonal)
                    DrawEikonal(data);

                if (drawHeatmap)
                    DrawHeatmap(data);

                if (drawDirection)
                    DrawDirections(data);

                DrawTester(data.Position, data.CellSize, data.Size);
            }
            else
            {
                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);

                DrawGrid(transform.position, cellSize, size);
                DrawTester(transform.position, cellSize, size);
            }
        }

        [ContextMenu("Bake")]
        private void BakeData()
        {
            var obstacles = GameObject.FindObjectsByType<FlowFieldObstacle>(FindObjectsInactive.Exclude);
            var currentPath = UnityEditor.AssetDatabase.GetAssetPath(this.data);

            bool isDataNull = string.IsNullOrEmpty(currentPath);

            if (!isDataNull)
            {
                UnityEditor.AssetDatabase.DeleteAsset(currentPath);
            }

            var data = ScriptableObject.CreateInstance<FlowFieldData>();

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(data);
            so.Update();
            so.FindProperty("cellSize").floatValue = cellSize;
            so.FindProperty("size").vector2IntValue = size;
            so.FindProperty("position").vector3Value = transform.position;
            so.FindProperty("cells").arraySize = size.x * size.y;
            so.FindProperty("min").vector3Value = transform.position - new Vector3(size.x, 0.0f, size.y) * cellSize / 2.0f;
            so.FindProperty("max").vector3Value = transform.position + new Vector3(size.x, 0.0f, size.y) * cellSize / 2.0f;
            so.FindProperty("targetWorldPosition").vector3Value = targetPosition;
            so.FindProperty("targetPosition").vector2IntValue = FlowFieldUtility.WorldToGridPosition(targetPosition, transform.position, cellSize);
            so.FindProperty("obstacles").arraySize = obstacles.Length;

            for (int i = 0; i < obstacles.Length; i++)
            {
                var obstacle = so.FindProperty("obstacles").GetArrayElementAtIndex(i);
                obstacle.FindPropertyRelative("Position").vector3Value = obstacles[i].transform.position;
                obstacle.FindPropertyRelative("Size").vector2IntValue = obstacles[i].Size;
            }

            so.ApplyModifiedProperties();

            data.Calculate();

            var newPath = string.Concat("Assets/Settings/", gameObject.scene.name, ".asset");
            UnityEditor.AssetDatabase.CreateAsset(data, UnityEditor.AssetDatabase.GenerateUniqueAssetPath(newPath));

            UnityEditor.EditorUtility.SetDirty(data);
            this.data = data;
            UnityEditor.EditorUtility.SetDirty(gameObject);

            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}