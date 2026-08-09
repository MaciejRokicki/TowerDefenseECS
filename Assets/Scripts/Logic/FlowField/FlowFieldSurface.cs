using System;
using UnityEngine;

namespace TD.Logic.FlowField
{
    public class FlowFieldSurface : MonoBehaviour
    {
        public static FlowFieldSurface Instance { get; private set; }

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

        private void Awake()
        {
            Instance = this;
        }

        public static Vector2Int ToGridPosition(Vector3 gridPosition, float cellSize, Vector3 worldPosition)
        {
            return new Vector2Int(
                (int)Math.Round((worldPosition.x - gridPosition.x - cellSize / 2.0f) / cellSize, MidpointRounding.AwayFromZero),
                (int)Math.Round((worldPosition.y - gridPosition.y - cellSize / 2.0f) / cellSize, MidpointRounding.AwayFromZero)
            );
        }

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
                        UnityEditor.Handles.Label(pos, string.Concat(data.GetValue(i, j).Cost.ToString("0.0"), " (", i, ", ", j, ")"), debugStyle);
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
                Gizmos.DrawCube(tester.position, Vector3.one * cellSize);
                testerPos = ToGridPosition(position, cellSize, tester.position);
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
            var modifiers = GameObject.FindObjectsByType<FlowFieldModifier>(FindObjectsInactive.Exclude);
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
            so.FindProperty("targetPosition").vector2IntValue = ToGridPosition(transform.position, cellSize, targetPosition);
            so.FindProperty("modifiers").arraySize = modifiers.Length;

            for (int i = 0; i < modifiers.Length; i++)
            {
                var modifier = so.FindProperty("modifiers").GetArrayElementAtIndex(i);
                modifier.FindPropertyRelative("Cost").floatValue = modifiers[i].Cost;
                modifier.FindPropertyRelative("Position").vector3Value = modifiers[i].transform.position;
                modifier.FindPropertyRelative("Size").vector2IntValue = modifiers[i].Size;
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