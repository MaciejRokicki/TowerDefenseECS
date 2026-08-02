using UnityEngine;

namespace TD.Logic.FlowField
{
    public class FlowFieldModifier : MonoBehaviour
    {
        public uint Cost;
        public Vector2Int Size;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
            Gizmos.DrawCube(transform.position + new Vector3(Size.x, 0.0f, Size.y) / 2.0f, new Vector3(Size.x, 1.0f, Size.y));
        }
    }
}