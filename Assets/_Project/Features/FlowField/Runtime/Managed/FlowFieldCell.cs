using System;
using TD.Features.FlowField.ECS.Components;
using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    [Serializable]
    public class FlowFieldCell
    {
        public Vector2Int GridPosition;
        public bool IsObstacle;
        public float Cost;
        public float Eikonal;
        public Vector3 Direction;

        public FlowFieldCell(FlowFieldCellData cellData)
        {
            GridPosition = cellData.GridPosition;
            IsObstacle = cellData.IsObstacle;
            Cost = cellData.Cost;
            Eikonal = cellData.Eikonal;
            Direction = cellData.Direction;
        }
    }
}