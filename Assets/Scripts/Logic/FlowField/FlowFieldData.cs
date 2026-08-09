using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Logic.FlowField
{
    [CreateAssetMenu(fileName = "DefaultFlowFieldData", menuName = "FlowField/Data")]
    public class FlowFieldData : ScriptableObject
    {
        private static FlowFieldCell[] neighbourArray;

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
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= size.y)
                throw new ArgumentOutOfRangeException("y");

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

                if (maxCostValue < current.Cost)
                    maxCostValue = current.Cost;

                if (current.Modified)
                    continue;

                GetNeighbours(current.GridPosition.x, current.GridPosition.y, ref neighbourArray);

                for (int i = 0; i < neighbourArray.Length; i++)
                {
                    var neighbour = neighbourArray[i];

                    if (neighbour == null)
                        continue;

                    if (neighbour.Modified)
                        continue;

                    float cost = 1.41f;

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
            queue = null;
            visited.Clear();
            visited = null;
            current = null;

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var cell = GetValue(i, j);

                    if (cell.GridPosition == targetPosition)
                        continue;

                    GetNeighbours(i, j, ref neighbourArray);
                    float minCost = float.MaxValue;
                    FlowFieldCell minNeighbour = null;

                    for (int k = 0; k < neighbourArray.Length; k++)
                    {
                        var neighbour = neighbourArray[k];

                        if (neighbour == null)
                            continue;

                        if (neighbour.Cost < minCost)
                        {
                            minCost = neighbour.Cost;
                            minNeighbour = neighbour;
                        }
                    }

                    if (minNeighbour != null)
                    {
                        cell.Direction = new Vector3(minNeighbour.GridPosition.x - cell.GridPosition.x, minNeighbour.GridPosition.y - cell.GridPosition.y, 0.0f);
                        cell.Direction.Normalize();
                    }
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

        private void GetNeighbours(int x, int y, ref FlowFieldCell[] neighbourArray)
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
    }
}