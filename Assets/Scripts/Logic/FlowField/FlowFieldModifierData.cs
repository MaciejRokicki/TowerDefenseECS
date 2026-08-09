using System;
using UnityEngine;

namespace TD.Logic.FlowField
{
    [Serializable]
    public record FlowFieldModifierData
    {
        public float Cost;
        public Vector3 Position;
        public Vector2Int Size;
    }
}