using UnityEngine;

namespace TD.Features.FlowField.Managed
{
    public class FlowFieldModifier : MonoBehaviour
    {
        public uint Cost;
        public Vector2Int Size;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
            Gizmos.DrawCube(transform.position + new Vector3(Size.x, Size.y, 0.0f) / 2.0f, new Vector3(Size.x, Size.y, 1.0f));
        }
    }
}