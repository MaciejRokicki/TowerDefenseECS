using System.Collections.Generic;
using TD.Features.FlowField.Managed.Data;
using TD.Features.FlowField.Managed.Logic;
using Unity.Collections;
using UnityEngine;

namespace TD.Features.FlowField.Managed.Config
{
    [CreateAssetMenu(fileName = "DefaultFlowFieldData", menuName = "FlowField/Data")]
    public partial class FlowFieldData : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField]
        private Vector3 worldPosition;
        [SerializeField]
        private Vector2Int size;
        [SerializeField]
        private float cellSize;
        [SerializeField]
        private Vector3 targetWorldPosition;
        [SerializeField]
        private Vector2Int targetGridPosition;

        [Header("Data")]
        [SerializeField]
        private float maxTime;
        [SerializeField]
        private FlowFieldCell[] cells;
        [SerializeField]
        private FlowFieldModifierData[] modifiers;

        public Vector3 WorldPosition => worldPosition;
        public Vector2Int Size => size;
        public float CellSize => cellSize;
        public Vector3 TargetWorldPosition => targetWorldPosition;
        public Vector2Int TargetGridPosition => targetGridPosition;

        public float MaxTime => maxTime;
        public IReadOnlyList<FlowFieldCell> Cells => cells;

        public FlowFieldCell GetValue(int x, int y)
        {
            if (x < 0 || x >= size.x)
            {
                return null;
            }

            if (y < 0 || y >= size.y)
            {
                return null;
            }

            return cells[x * size.y + y];
        }

        public void Bake()
        {
            NativeArray<ECS.Components.FlowFieldModifierData> modifiers = new NativeArray<ECS.Components.FlowFieldModifierData>(this.modifiers.Length, Allocator.TempJob);

            for (int i = 0; i < modifiers.Length; i++)
            {
                var modifier = this.modifiers[i];
                modifiers[i] = new ECS.Components.FlowFieldModifierData()
                {
                    WorldPosition = modifier.WorldPosition,
                    Size = modifier.Size,
                    Cost = modifier.Cost,
                    IsObstacle = modifier.IsObstacle,
                };
            }

            FlowFieldDataBaker.Calculate(worldPosition, cellSize, size, targetWorldPosition, modifiers, out var cells, out maxTime);

            this.cells = new FlowFieldCell[cells.Length];

            for (int i = 0; i < cells.Length; i++)
            {
                this.cells[i] = new FlowFieldCell(cells[i]);
            }

            modifiers.Dispose();
            cells.Dispose();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}