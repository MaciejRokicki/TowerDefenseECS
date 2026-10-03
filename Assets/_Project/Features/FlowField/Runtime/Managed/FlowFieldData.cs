using System.Collections.Generic;
using System.Diagnostics;
using TD.Features.FlowField.ECS.Components;
using Unity.Collections;
using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    [CreateAssetMenu(fileName = "DefaultFlowFieldData", menuName = "FlowField/Data")]
    public partial class FlowFieldData : ScriptableObject
    {
        [SerializeField]
        private float cellSize;
        [SerializeField]
        private Vector2Int size;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Vector3 min;
        [SerializeField]
        private Vector3 max;
        [SerializeField]
        private Vector3 targetWorldPosition;
        [SerializeField]
        private Vector2Int targetPosition;
        [SerializeField]
        private float maxCostValue;
        [SerializeField]
        private FlowFieldCell[] cells;
        [SerializeField]
        private FlowFieldObstacleData[] obstacles;
        [SerializeField]
        private Vector2Int[] obstacleCells;

        public float CellSize => cellSize;
        public Vector2Int Size => size;
        public Vector3 Position => position;
        public Vector3 TargetWorldPosition => targetWorldPosition;
        public Vector2Int TargetPosition => targetPosition;
        public float MaxCostValue => maxCostValue;
        public IReadOnlyList<FlowFieldCell> Cells => cells;
        public IReadOnlyList<Vector2Int> ObstacleCells => obstacleCells;

        public FlowFieldCell GetValue(int x, int y)
        {
            if (x < 0 || x >= size.x)
            {
                return null;
            }

            if (y < 0 || y >= size.y)
            {
                return null;
            }

            return cells[x * size.y + y];
        }

        public void Bake()
        {
            NativeArray<FlowFieldObstalceData> obstacles = new NativeArray<FlowFieldObstalceData>(this.obstacles.Length, Allocator.TempJob);

            for (int i = 0; i < obstacles.Length; i++)
            {
                var obstacle = this.obstacles[i];
                obstacles[i] = new FlowFieldObstalceData()
                {
                    Position = obstacle.Position,
                    Size = obstacle.Size,
                };
            }

            var sw = new Stopwatch();
            sw.Start();
            FlowFieldDataBaker.Calculate(position, cellSize, size, targetWorldPosition, obstacles, out var cells, out var obstacleCells, out maxCostValue);
            sw.Stop();
            UnityEngine.Debug.Log(sw.Elapsed.TotalMilliseconds);
            this.cells = new FlowFieldCell[cells.Length];

            for (int i = 0; i < cells.Length; i++)
            {
                this.cells[i] = new FlowFieldCell(cells[i]);
            }

            this.obstacleCells = new Vector2Int[obstacleCells.Length];

            for (int i = 0; i < obstacleCells.Length; i++)
            {
                this.obstacleCells[i] = obstacleCells[i];
            }

            obstacles.Dispose();
            cells.Dispose();
            obstacleCells.Dispose();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}