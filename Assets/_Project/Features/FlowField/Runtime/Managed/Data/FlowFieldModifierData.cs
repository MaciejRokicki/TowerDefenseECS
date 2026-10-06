using System;
using UnityEngine;

namespace TD.Features.FlowField.Managed.Data
{
    [Serializable]
    public record FlowFieldModifierData
    {
        public Vector3 WorldPosition;
        public Vector2Int Size;
        public float Cost;
        public bool IsObstacle;
    }
}