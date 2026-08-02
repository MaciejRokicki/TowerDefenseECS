using System;
using UnityEngine;

namespace TD.Logic.FlowField
{
    [Serializable]
    public record FlowFieldCell
    {
        public Vector2Int GridPosition;
        public bool Modified;
        public uint Cost;
        public Vector3 Direction;
    }
}