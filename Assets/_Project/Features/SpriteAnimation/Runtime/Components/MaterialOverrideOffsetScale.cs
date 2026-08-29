using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace TD.Features.SpriteAnimation.Runtime.Components
{
	[MaterialProperty("_OffsetXYScaleZW")]
	public struct MaterialOverrideOffsetScale : IComponentData
	{
		public float2 Offset;
		public float2 Scale;
	} 
}
