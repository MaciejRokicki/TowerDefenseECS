using TD.Features.FlowField.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    [BurstCompile]
    public static class FlowFieldDataBaker
    {
        const float diagonalCost = 1.41421356237f;

        [BurstCompile]
        private struct BakeJob : IJob
        {
            public float3 position;
            public float cellSize;
            public int2 size;
            public float3 targetWorldPosition;
            [ReadOnly]
            public NativeArray<FlowFieldObstalceData> obstacles;
            public NativeArray<FlowFieldCellData> cells;
            public NativeList<int2> obstacleCells;
            public NativeReference<float> maxCostValue;

            public void Execute()
            {
                FlowFieldUtility.WorldToGridPosition(targetWorldPosition, position, cellSize, out int2 targetPosition);

                int index = 0;
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        index = i * size.y + j;
                        var cell = new FlowFieldCellData()
                        {
                            GridPosition = new Vector2Int(i, j),
                            IsObstacle = false,
                            Cost = 0.0f,
                            Eikonal = float.PositiveInfinity,
                            Direction = Vector3.zero
                        };
                        cells[index] = cell;
                    }
                }

                for (int i = 0; i < obstacles.Length; i++)
                {
                    var obstacle = obstacles[i];
                    var gridPosition = FlowFieldUtility.WorldToGridPosition(obstacle.Position, position, cellSize);

                    for (int j = 0; j < obstacle.Size.x; j++)
                    {
                        for (int k = 0; k < obstacle.Size.y; k++)
                        {
                            GetValue(gridPosition.x + j, gridPosition.y + k, size, cells, out var cell);
                            cell.Cost = float.PositiveInfinity;
                            cell.Eikonal = float.PositiveInfinity;
                            cell.Direction = float3.zero;
                            cell.IsObstacle = true;
                            ToIndex(gridPosition.x + j, gridPosition.y + k, size, out index);
                            cells[index] = cell;
                            obstacleCells.Add(cell.GridPosition);
                        }
                    }
                }

                NativeQueue<FlowFieldCellData> queue = new NativeQueue<FlowFieldCellData>(Allocator.Temp);
                NativeHashSet<FlowFieldCellData> visited = new NativeHashSet<FlowFieldCellData>(cells.Length, Allocator.Temp);
                GetValue(targetPosition.x, targetPosition.y, size, cells, out var current);
                queue.Enqueue(current);
                visited.Add(current);

                while (queue.Count > 0)
                {
                    current = queue.Dequeue();
                    current.Eikonal = float.PositiveInfinity;

                    if (maxCostValue.Value < current.Cost)
                        maxCostValue.Value = current.Cost;

                    if (current.IsObstacle)
                        continue;

                    for (int dx = -1; dx < 2; dx++)
                    {
                        int nx = current.GridPosition.x + dx;

                        if (nx < 0 || nx >= size.x)
                            continue;

                        int columnStart = nx * size.y;

                        for (int dy = -1; dy < 2; dy++)
                        {
                            if (dx == 0 && dy == 0)
                                continue;

                            int ny = current.GridPosition.y + dy;

                            if (ny < 0 || ny >= size.y)
                                continue;

                            var neighbour = cells[columnStart + ny];

                            if (neighbour.IsObstacle)
                                continue;

                            float cost = diagonalCost;
                            int2 dir = current.GridPosition - neighbour.GridPosition;

                            if (dir.x == 0 || dir.y == 0)
                            {
                                cost = 1.0f;
                            }

                            if (math.all(neighbour.GridPosition == targetPosition))
                            {
                                neighbour.Cost = 0.0f;
                                ToIndex(neighbour.GridPosition.x, neighbour.GridPosition.y, size, out index);
                                cells[index] = neighbour;
                            }
                            else
                            {
                                float newCost = current.Cost + cost;

                                if (neighbour.Cost == 0.0f || neighbour.Cost > newCost)
                                {
                                    neighbour.Cost = newCost;
                                    ToIndex(neighbour.GridPosition.x, neighbour.GridPosition.y, size, out index);
                                    cells[index] = neighbour;
                                    queue.Enqueue(neighbour);
                                }
                            }
                        }
                    }
                }

                queue.Clear();
                visited.Clear();

                var sortedList = new NativeList<FlowFieldCellData>(Allocator.Temp);
                GetValue(targetPosition.x, targetPosition.y, size, cells, out current);
                sortedList.Add(current);
                visited.Add(current);

                current.Eikonal = 0.0f;

                while (sortedList.Length > 0)
                {
                    sortedList.Sort(new FlowFieldCellDataCostComparer());
                    current = sortedList[0];
                    sortedList.RemoveAt(0);

                    if (current.IsObstacle)
                        continue;

                    if (math.all(current.GridPosition == targetPosition))
                    {
                        current.Eikonal = 0.0f;
                        ToIndex(current.GridPosition.x, current.GridPosition.y, size, out index);
                        cells[index] = current;
                    }
                    else
                    {
                        GetNeighbourForEikonalCalculation(current.GridPosition.x - 1, current.GridPosition.y, size, cells, out var x1);
                        GetNeighbourForEikonalCalculation(current.GridPosition.x + 1, current.GridPosition.y, size, cells, out var x2);
                        GetNeighbourForEikonalCalculation(current.GridPosition.x, current.GridPosition.y - 1, size, cells, out var y1);
                        GetNeighbourForEikonalCalculation(current.GridPosition.x, current.GridPosition.y + 1, size, cells, out var y2);

                        SolveEikonal(Mathf.Min(x1, x2), Mathf.Min(y1, y2), 1.0f, out current.Eikonal);
                        ToIndex(current.GridPosition.x, current.GridPosition.y, size, out index);
                        cells[index] = current;
                    }

                    for (int dx = -1; dx < 2; dx++)
                    {
                        int nx = current.GridPosition.x + dx;

                        if (nx < 0 || nx >= size.x)
                            continue;

                        int columnStart = nx * size.y;

                        for (int dy = -1; dy < 2; dy++)
                        {
                            if (math.abs(dx) + math.abs(dy) != 1)
                                continue;

                            if (dx == 0 && dy == 0)
                                continue;

                            int ny = current.GridPosition.y + dy;

                            if (ny < 0 || ny >= size.y)
                                continue;

                            var neighbour = cells[columnStart + ny];

                            if (neighbour.IsObstacle)
                                continue;

                            if (visited.Contains(neighbour))
                                continue;

                            if (neighbour.IsObstacle)
                                continue;

                            visited.Add(neighbour);
                            sortedList.Add(neighbour);
                        }
                    }
                }

                queue.Dispose();
                visited.Dispose();
                sortedList.Dispose();

                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        GetValue(i, j, size, cells, out var cell);

                        if (cell.IsObstacle || math.all(cell.GridPosition == targetPosition) || float.IsInfinity(cell.Eikonal) || float.IsNaN(cell.Eikonal))
                        {
                            cell.Direction = float3.zero;
                            ToIndex(cell.GridPosition.x, cell.GridPosition.y, size, out index);
                            cells[index] = cell;
                            continue;
                        }

                        GetValue(i - 1, j, size, cells, out var x1);
                        GetValue(i + 1, j, size, cells, out var x2);
                        GetValue(i, j - 1, size, cells, out var y1);
                        GetValue(i, j + 1, size, cells, out var y2);

                        var x = Mathf.Min(!x1.Equals(default) ? x1.Eikonal : float.PositiveInfinity, !x2.Equals(default) ? x2.Eikonal : float.PositiveInfinity);
                        var y = Mathf.Min(!y1.Equals(default) ? y1.Eikonal : float.PositiveInfinity, !y2.Equals(default) ? y2.Eikonal : float.PositiveInfinity);

                        CalculateDirectionField(
                            cell.Eikonal,
                            x1.Equals(default) ? cell.Eikonal : x1.Eikonal,
                            x2.Equals(default) ? cell.Eikonal : x2.Eikonal,
                            out var directionX);
                        CalculateDirectionField(
                            cell.Eikonal,
                            y1.Equals(default) ? cell.Eikonal : y1.Eikonal,
                            y2.Equals(default) ? cell.Eikonal : y2.Eikonal,
                            out var directionY);

                        cell.Direction = new float3(directionX, directionY, 0.0f);
                        ToIndex(cell.GridPosition.x, cell.GridPosition.y, size, out index);
                        cells[index] = cell;
                    }
                }
            }
        }

        [BurstCompile]
        public static void Calculate(
            in float3 position,
            in float cellSize,
            in int2 size,
            in float3 targetWorldPosition,
            in NativeArray<FlowFieldObstalceData> obstacles,
            out NativeArray<FlowFieldCellData> cells,
            out NativeList<int2> obstacleCells,
            out float maxCostValue)
        {
            cells = new NativeArray<FlowFieldCellData>(size.x * size.y, Allocator.TempJob);
            obstacleCells = new NativeList<int2>(Allocator.TempJob);
            var maxCost = new NativeReference<float>(0.0f, Allocator.TempJob);

            var job = new BakeJob()
            {
                position = position,
                cellSize = cellSize,
                size = size,
                targetWorldPosition = targetWorldPosition,
                obstacles = obstacles,
                cells = cells,
                obstacleCells = obstacleCells,
                maxCostValue = maxCost
            }.Schedule();

            job.Complete();

            maxCostValue = maxCost.Value;
            maxCost.Dispose();
        }

        [BurstCompile]
        private static void ToIndex(in int x, in int y, in int2 size, out int result) => result = x * size.y + y;

        [BurstCompile]
        private static void GetValue(in int x, in int y, in int2 size, in NativeArray<FlowFieldCellData> cells, out FlowFieldCellData cell)
        {
            cell = new FlowFieldCellData();
            if (x < 0 || x >= size.x)
                return;

            if (y < 0 || y >= size.y)
                return;

            ToIndex(x, y, size, out int index);
            cell = cells[index];
        }

        [BurstCompile]
        private static void GetNeighbourForEikonalCalculation(in int x, in int y, in int2 size, in NativeArray<FlowFieldCellData> cells, out float result)
        {
            GetValue(x, y, size, cells, out var cell);

            if (cell.Equals(default))
            {
                result = float.PositiveInfinity;
                return;
            }

            result = cell.Eikonal;
        }

        [BurstCompile]
        private static void SolveEikonal(in float tx, in float ty, in float cost, out float result)
        {
            if (float.IsPositiveInfinity(tx) || tx == float.MaxValue)
            {
                result = ty + cost;
                return;
            }

            if (float.IsPositiveInfinity(ty) || ty == float.MaxValue)
            {
                result = tx + cost;
                return;
            }

            float delta = 2.0f * (cost * cost) - (tx - ty) * (tx - ty);

            if (delta >= 0.0f)
            {
                float t = (tx + ty + (float)math.sqrt(delta)) / 2.0f;

                if (t > tx && t > ty)
                {
                    result = t;
                    return;
                }
            }

            result = math.min(tx, ty) + cost;
        }

        [BurstCompile]
        private static void CalculateDirectionField(in float current, in float left, in float right, out float result)
        {
            if (right < current)
            {
                result = current - right;
                return;
            }

            if (left < current)
            {
                result = -(current - left);
                return;
            }

            result = 0.0f;
        }
    }
}