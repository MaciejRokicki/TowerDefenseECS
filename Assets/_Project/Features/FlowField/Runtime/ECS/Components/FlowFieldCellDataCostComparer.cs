using System.Collections.Generic;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldCellDataCostComparer : IComparer<FlowFieldCellData>
    {
        public int Compare(FlowFieldCellData x, FlowFieldCellData y)
        {
            return x.Cost.CompareTo(y.Cost);
        }
    }
}