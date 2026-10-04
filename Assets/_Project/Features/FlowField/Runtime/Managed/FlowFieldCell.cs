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
        public byte State; // 0 - far, 1 - trial 2 - accepted
        public float Cost;
        public float Time;
        public Vector3 Direction;

        public FlowFieldCell(FlowFieldCellData cellData)
        {
            GridPosition = cellData.GridPosition;
            IsObstacle = cellData.IsObstacle;
            State = cellData.State;
            Cost = cellData.Cost;
            Time = cellData.Time;
            Direction = cellData.Direction;
        }
    }
}