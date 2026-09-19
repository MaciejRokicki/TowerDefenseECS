using System;
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
    }
}