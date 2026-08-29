using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Features.FlowField.Runtime
{
    [CreateAssetMenu(fileName = "DefaultFlowFieldData", menuName = "FlowField/Data")]
    public partial class FlowFieldData : ScriptableObject
    {
        [AutoStaticsCleanup] 
        private static FlowFieldCell[] neighbourArray8;
        [AutoStaticsCleanup] 
        private static FlowFieldCell[] neighbourArray4;
        [AutoStaticsCleanup] 
        private static float sqrtOfTwo = Mathf.Sqrt(2);

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
        private FlowFieldModifierData[] modifiers;

        public float CellSize => cellSize;
        public Vector2Int Size => size;
        public Vector3 Position => position;
        public Vector3 TargetWorldPosition => targetWorldPosition;
        public Vector2Int TargetPosition => targetPosition;
        public float MaxCostValue => maxCostValue;
        public IReadOnlyList<FlowFieldCell> Cells => cells;

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

        public void Calculate()
        {
            int index = 0;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    index = i * size.y + j;
                    cells[index].GridPosition = new Vector2Int(i, j);
                }
            }

            for (int i = 0; i < modifiers.Length; i++)
            {
                var modifier = modifiers[i];
                var gridPosition = FlowFieldSurface.ToGridPosition(position, cellSize, modifier.Position);

                for (int j = 0; j < modifier.Size.x; j++)
                {
                    for (int k = 0; k < modifier.Size.y; k++)
                    {
                        var cell = GetValue(gridPosition.x + j, gridPosition.y + k);
                        cell.Cost = modifier.Cost;
                        cell.Eikonal = float.PositiveInfinity;
                        cell.Direction = Vector2.Normalize(targetPosition - cell.GridPosition);
                        cell.Modified = true;
                    }
                }
            }

            Queue<FlowFieldCell> queue = new Queue<FlowFieldCell>(cells.Length);
            HashSet<FlowFieldCell> visited = new HashSet<FlowFieldCell>(cells.Length);
            var current = GetValue(targetPosition.x, targetPosition.y);
            queue.Enqueue(current);
            visited.Add(current);

            while (queue.Count > 0)
            {
                current = queue.Dequeue();
                current.Eikonal = float.PositiveInfinity;

                if (maxCostValue < current.Cost)
                    maxCostValue = current.Cost;

                if (current.Modified)
                    continue;

                GetNeighbours8(current.GridPosition.x, current.GridPosition.y, ref neighbourArray8);

                for (int i = 0; i < neighbourArray8.Length; i++)
                {
                    var neighbour = neighbourArray8[i];

                    if (neighbour == null)
                        continue;

                    if (neighbour.Modified)
                        continue;

                    float cost = sqrtOfTwo;

                    Vector2Int dir = current.GridPosition - neighbour.GridPosition;

                    if (dir.x == 0 || dir.y == 0)
                    {
                        cost = 1.0f;
                    }

                    if (neighbour.GridPosition == targetPosition)
                    {
                        neighbour.Cost = 0.0f;
                    }
                    else
                    {
                        float newCost = current.Cost + cost;

                        if (neighbour.Cost == 0.0f || neighbour.Cost > newCost)
                        {
                            neighbour.Cost = newCost;
                            queue.Enqueue(neighbour);
                        }
                    }
                }
            }

            queue.Clear();
            visited.Clear();

            var sortedList = new List<FlowFieldCell>();
            current = GetValue(targetPosition.x, targetPosition.y);
            sortedList.Add(current);
            visited.Add(current);
            int c = 0;

            current.Eikonal = 0.0f;

            while (sortedList.Count > 0)
            {
                c++;
                sortedList = sortedList.OrderBy(x => x.Cost).ToList();
                current = sortedList[0];
                sortedList.RemoveAt(0);

                if (c == 300_000)
                {
                    Debug.Log("C");
                    break;
                }

                if (current.Modified)
                    continue;

                if (current.GridPosition == targetPosition)
                {
                    current.Eikonal = 0.0f;
                }
                else
                {
                    var x1 = GetNeighbourForEikonalCalculation(current.GridPosition.x - 1, current.GridPosition.y);
                    var x2 = GetNeighbourForEikonalCalculation(current.GridPosition.x + 1, current.GridPosition.y);
                    var y1 = GetNeighbourForEikonalCalculation(current.GridPosition.x, current.GridPosition.y - 1);
                    var y2 = GetNeighbourForEikonalCalculation(current.GridPosition.x, current.GridPosition.y + 1);

                    current.Eikonal = SolveEikonal(Mathf.Min(x1, x2), Mathf.Min(y1, y2), 1.0f);
                }

                GetNeighbours4(current.GridPosition.x, current.GridPosition.y, ref neighbourArray4);

                for (int i = 0; i < neighbourArray4.Length; i++)
                {
                    var neighbour = neighbourArray4[i];

                    if (neighbour == null)
                        continue;

                    if (visited.Contains(neighbour))
                        continue;

                    if (neighbour.Modified)
                        continue;

                    visited.Add(neighbour);
                    sortedList.Add(neighbour);
                }
            }

            queue.Clear();
            queue = null;
            visited.Clear();
            visited = null;
            current = null;

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var cell = GetValue(i, j);

                    if (cell.Modified)
                        continue;

                    if (cell.GridPosition == targetPosition)
                        continue;

                    var x1 = GetValue(i - 1, j);
                    var x2 = GetValue(i + 1, j);
                    var y1 = GetValue(i, j - 1);
                    var y2 = GetValue(i, j + 1);

                    var x = Mathf.Min(x1 != null ? x1.Eikonal : float.PositiveInfinity, x2 != null ? x2.Eikonal : float.PositiveInfinity);
                    var y = Mathf.Min(y1 != null ? y1.Eikonal : float.PositiveInfinity, y2 != null ? y2.Eikonal : float.PositiveInfinity);

                    cell.Direction = new Vector3(
                        CalculateDirectionField(cell.Eikonal, x1 == null ? cell.Eikonal : x1.Eikonal, x2 == null ? cell.Eikonal : x2.Eikonal),
                        CalculateDirectionField(cell.Eikonal, y1 == null ? cell.Eikonal : y1.Eikonal, y2 == null ? cell.Eikonal : y2.Eikonal)
                    );
                }
            }
        }

        private bool ValidatePosition(Vector2Int position)
        {
            if (position.x < 0 || position.x >= size.x ||
                position.y < 0 || position.y >= size.y)
                return false;

            return true;
        }

        private void GetNeighbours4(int x, int y, ref FlowFieldCell[] neighbourArray)
        {
            if (neighbourArray == null)
                neighbourArray = new FlowFieldCell[4];

            Vector2Int neighbourPosition = new Vector2Int(x, y + 1);
            neighbourArray[0] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x + 1, y);
            neighbourArray[1] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x, y - 1);
            neighbourArray[2] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x - 1, y);
            neighbourArray[3] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;
        }

        private void GetNeighbours8(int x, int y, ref FlowFieldCell[] neighbourArray)
        {
            if (neighbourArray == null)
                neighbourArray = new FlowFieldCell[8];

            Vector2Int neighbourPosition = new Vector2Int(x, y + 1);
            neighbourArray[0] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x + 1, y + 1);
            neighbourArray[1] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x + 1, y);
            neighbourArray[2] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x + 1, y - 1);
            neighbourArray[3] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x, y - 1);
            neighbourArray[4] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x - 1, y - 1);
            neighbourArray[5] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x - 1, y);
            neighbourArray[6] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;

            neighbourPosition = new Vector2Int(x - 1, y + 1);
            neighbourArray[7] = ValidatePosition(neighbourPosition) ? GetValue(neighbourPosition.x, neighbourPosition.y) : null;
        }

        private float GetNeighbourForEikonalCalculation(int x, int y)
        {
            var res = GetValue(x, y);

            if (res == null)
                return float.PositiveInfinity;

            return res.Eikonal;
        }

        private float SolveEikonal(float tx, float ty, float cost)
        {
            if (float.IsPositiveInfinity(tx) || tx == float.MaxValue) return ty + cost;
            if (float.IsPositiveInfinity(ty) || ty == float.MaxValue) return tx + cost;

            float delta = 2f * (cost * cost) - (tx - ty) * (tx - ty);

            if (delta >= 0f)
            {
                float t = (tx + ty + (float)Math.Sqrt(delta)) / 2f;

                if (t > tx && t > ty)
                {
                    return t;
                }
            }

            return Math.Min(tx, ty) + cost;
        }

        private float CalculateDirectionField(float current, float left, float right)
        {
            if (right < current)
                return current - right;

            if (left < current)
                return -(current - left);

            return 0.0f;
        }
    }
}