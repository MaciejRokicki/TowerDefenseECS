using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.SpriteAnimation.Runtime.Components
{
	public struct SpriteFrameElement : IBufferElementData
	{
		public float2 offset;
	} 
}
