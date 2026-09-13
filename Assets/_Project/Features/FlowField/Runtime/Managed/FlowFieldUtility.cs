using System;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    [BurstCompile]
    public static class FlowFieldUtility
    {
        [BurstDiscard]
        public static Vector2Int WorldToGridPosition(Vector3 worldPosition, Vector3 gridWorldPosition, float cellSize)
        {
            var res = new Vector2Int(
                (int)Math.Round((worldPosition.x - gridWorldPosition.x - cellSize / 2.0f) / cellSize, MidpointRounding.AwayFromZero),
                (int)Math.Round((worldPosition.y - gridWorldPosition.y - cellSize / 2.0f) / cellSize, MidpointRounding.AwayFromZero)
            );

            res.x = Mathf.Clamp(res.x, 0, res.x);
            res.y = Mathf.Clamp(res.y, 0, res.y);

            return res;
        }

        //[BurstCompile]
        //public static void WorldToGridPosition(in float2 worldPosition, in float2 gridWorldPosition, in float cellSize, out int2 gridPosition)
        //{
        //    gridPosition = (int2)math.floor((worldPosition - gridWorldPosition) / cellSize);
        //}

        [BurstCompile]
        public static void WorldToGridPosition(in float2 worldPosition, in float2 gridWorldPosition, float cellSize, out int2 gridPosition)
        {
            float2 value = (worldPosition.xy - gridWorldPosition.xy - cellSize * 0.5f) / cellSize;
            float2 truncated = math.trunc(value);
            float2 fraction = value - truncated;

            int2 result = (int2)truncated;

            int2 direction = math.select(new int2(-1), new int2(1), value >= 0f);
            result += math.select(new int2(0), direction, math.abs(fraction) >= 0.5f);

            gridPosition = math.max(result, new int2(0));
        }
    }
}