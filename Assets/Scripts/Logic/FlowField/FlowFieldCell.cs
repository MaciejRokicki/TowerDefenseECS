using System;
using UnityEngine;

namespace TD.Logic.FlowField
{
    [Serializable]
    public record FlowFieldCell
    {
        public Vector2Int GridPosition;
        public bool Modified;
        public float Cost;
        public Vector3 Direction;
    }
}