using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Shared;
using TD.Shared;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TD.Features.FlowField.Managed.Logic
{
    [BurstCompile]
    public static class FlowFieldDataBaker
    {
        [BurstCompile]
        private struct FlowFieldDataBakeJob : IJob
        {
            public float3 WorldPosition;
            public int2 Size;
            public float CellSize;
            public float3 TargetWorldPosition;
            [ReadOnly]
            public NativeArray<FlowFieldModifierData> Modifiers;
            public NativeArray<FlowFieldCellData> Cells;
            public NativeReference<float> MaxTimeValue;

            public void Execute()
            {
                FillCellsWithDefaultValues();
                FillObstacles();
                SetupTargetCell();
                FastMarching();
                CalculateDirections();
            }

            private void FillCellsWithDefaultValues()
            {
                int index = 0;

                for (int i = 0; i < Size.x; i++)
                {
                    for (int j = 0; j < Size.y; j++)
                    {
                        index = i * Size.y + j;
                        var cell = new FlowFieldCellData()
                        {
                            GridPosition = new int2(i, j),
                            IsObstacle = false,
                            State = 0,
                            Cost = 1.0f,
                            Time = float.PositiveInfinity,
                            Direction = float3.zero
                        };
                        Cells[index] = cell;
                    }
                }
            }

            private void FillObstacles()
            {
                for (int i = 0; i < Modifiers.Length; i++)
                {
                    var modifier = Modifiers[i];
                    var gridPosition = FlowFieldUtility.WorldToGridPosition(modifier.WorldPosition, WorldPosition, CellSize);

                    for (int j = 0; j < modifier.Size.x; j++)
                    {
                        for (int k = 0; k < modifier.Size.y; k++)
                        {
                            var cell = GetValue(gridPosition.x + j, gridPosition.y + k, Size, Cells);
                            if (modifier.IsObstacle)
                            {
                                cell.IsObstacle = true;
                                cell.Cost = float.PositiveInfinity;
                            }
                            else
                            {
                                cell.IsObstacle = false;
                                cell.Cost = modifier.Cost;
                            }
                            cell.Time = float.PositiveInfinity;
                            cell.Direction = float3.zero;
                            Cells[ToIndex(gridPosition.x + j, gridPosition.y + k, Size)] = cell;
                        }
                    }
                }
            }

            private void SetupTargetCell()
            {
                FlowFieldUtility.WorldToGridPosition(TargetWorldPosition, WorldPosition, CellSize, out int2 targetPosition);
                int index = ToIndex(targetPosition.x, targetPosition.y, Size);
                var targetCell = Cells[index];
                targetCell.Time = 0.0f;
                targetCell.State = 2;
                Cells[index] = targetCell;
            }

            private void FastMarching()
            {
                var minHeap = new NativeMinHeap<FlowFieldCellData>(4 * Cells.Length, Allocator.Temp);
                FlowFieldUtility.WorldToGridPosition(TargetWorldPosition, WorldPosition, CellSize, out int2 targetPosition);
                UpdateNeighbours(targetPosition, ref minHeap);

                while (minHeap.TryPop(out var queuedCell))
                {
                    int index = ToIndex(queuedCell.GridPosition.x, queuedCell.GridPosition.y, Size);
                    var currentCell = Cells[index];

                    if (currentCell.State == 2)
                        continue;

                    if (queuedCell.Time > currentCell.Time)
                        continue;

                    currentCell.State = 2;
                    Cells[index] = currentCell;
                    UpdateNeighbours(currentCell.GridPosition, ref minHeap);
                }

                minHeap.Dispose();
            }

            private void CalculateDirections()
            {
                for (int i = 0; i < Cells.Length; i++)
                {
                    var cell = Cells[i];
                    cell.Direction = float3.zero;

                    if (cell.IsObstacle || !math.isfinite(cell.Time) || cell.Time == 0.0f)
                    {
                        Cells[i] = cell;
                        continue;
                    }

                    int x = cell.GridPosition.x;
                    int y = cell.GridPosition.y;

                    float leftTime = GetAcceptedTime(x - 1, y);
                    float rightTime = GetAcceptedTime(x + 1, y);
                    float downTime = GetAcceptedTime(x, y - 1);
                    float upTime = GetAcceptedTime(x, y + 1);

                    float dx = GetDownhillComponent(cell.Time, leftTime, rightTime);
                    float dz = GetDownhillComponent(cell.Time, downTime, upTime);
                    cell.Direction = math.normalizesafe(new float3(dx, dz, 0.0f));
                    Cells[i] = cell;
                }
            }

            private void UpdateNeighbours(int2 gridPosition, ref NativeMinHeap<FlowFieldCellData> minHeap)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (math.abs(dx) + math.abs(dy) != 1)
                            continue;

                        int nx = gridPosition.x + dx;
                        int ny = gridPosition.y + dy;

                        if (nx < 0 || nx >= Size.x || ny < 0 || ny >= Size.y)
                            continue;

                        int neighbourIndex = ToIndex(nx, ny, Size);
                        var neighbour = Cells[neighbourIndex];

                        if (neighbour.IsObstacle || neighbour.State == 2)
                            continue;

                        float newTime = SolveEikonal(neighbour);

                        if (newTime < neighbour.Time)
                        {
                            neighbour.Time = newTime;
                            neighbour.State = 1;
                            Cells[neighbourIndex] = neighbour;
                            minHeap.Push(neighbour);

                            if (MaxTimeValue.Value < newTime)
                                MaxTimeValue.Value = newTime;
                        }
                    }
                }
            }

            private float GetAcceptedTime(int x, int y)
            {
                if (x < 0 || x >= Size.x || y < 0 || y >= Size.y)
                    return float.PositiveInfinity;

                var neighbour = Cells[ToIndex(x, y, Size)];

                if (neighbour.IsObstacle || neighbour.State != 2)
                    return float.PositiveInfinity;

                return neighbour.Time;
            }

            private float SolveEikonal(FlowFieldCellData cell)
            {
                int x = cell.GridPosition.x;
                int y = cell.GridPosition.y;

                float a = math.min(GetAcceptedTime(x - 1, y), GetAcceptedTime(x + 1, y));
                float b = math.min(GetAcceptedTime(x, y - 1), GetAcceptedTime(x, y + 1));
                float q = CellSize * cell.Cost;

                if (float.IsPositiveInfinity(a))
                {
                    return b + q;
                }

                if (float.IsPositiveInfinity(b))
                {
                    return a + q;
                }

                float low = math.min(a, b);
                float high = math.max(a, b);

                if (high - low >= q)
                {
                    return low + q;
                }

                float difference = high - low;
                float delta = 2.0f * q * q - difference * difference;
                return (low + high + math.sqrt(math.max(0.0f, delta))) * 0.5f;
            }

            private float GetDownhillComponent(float currentTime, float negativeSideTime, float positiveSideTime)
            {
                if (negativeSideTime <= positiveSideTime && negativeSideTime < currentTime)
                {
                    return -(currentTime - negativeSideTime) / CellSize;
                }

                if (positiveSideTime < currentTime)
                {
                    return (currentTime - positiveSideTime) / CellSize;
                }

                return 0.0f;
            }

            private int ToIndex(int x, int y, int2 size) => x * size.y + y;

            private FlowFieldCellData GetValue(int x, int y, int2 size, NativeArray<FlowFieldCellData> cells)
            {
                if (x < 0 || x >= size.x)
                    return default;

                if (y < 0 || y >= size.y)
                    return default;

                return cells[ToIndex(x, y, size)];
            }
        }

        [BurstCompile]
        public static void Calculate(
            in float3 position,
            in float cellSize,
            in int2 size,
            in float3 targetWorldPosition,
            in NativeArray<FlowFieldModifierData> modifiers,
            out NativeArray<FlowFieldCellData> cells,
            out float maxTimeValue)
        {
            cells = new NativeArray<FlowFieldCellData>(size.x * size.y, Allocator.TempJob);
            var maxTime = new NativeReference<float>(0.0f, Allocator.TempJob);

            var job = new FlowFieldDataBakeJob()
            {
                WorldPosition = position,
                CellSize = cellSize,
                Size = size,
                TargetWorldPosition = targetWorldPosition,
                Modifiers = modifiers,
                Cells = cells,
                MaxTimeValue = maxTime
            }.Schedule();

            job.Complete();

            maxTimeValue = maxTime.Value;
            maxTime.Dispose();
        }
    }
}