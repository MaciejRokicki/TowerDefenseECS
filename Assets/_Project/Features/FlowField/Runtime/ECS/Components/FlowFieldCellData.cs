using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldCellData : IComponentData, IEquatable<FlowFieldCellData>
    {
        public int2 GridPosition;
        [MarshalAs(UnmanagedType.U1)]
        public bool IsObstacle;
        public float Cost;
        public float Eikonal;
        public float3 Direction;

        public bool Equals(FlowFieldCellData other)
        {
            return math.all(GridPosition == other.GridPosition) && IsObstacle == other.IsObstacle && Cost == other.Cost && Eikonal == other.Eikonal && math.all(Direction == other.Direction);
        }

        public override bool Equals(object obj)
        {
            return obj is FlowFieldCellData other && Equals(other);
        }

        public override int GetHashCode()
        {
            var h1 = (int)math.hash(GridPosition);
            var h2 = (int)math.hash(new float3(IsObstacle ? 1.0f : 0.0f, Cost, Eikonal));
            var h3 = (int)math.hash(Direction);
            var res = unchecked((int)math.hash(new int3(h1, h2, h3)));
            return res;
        }
    }
}