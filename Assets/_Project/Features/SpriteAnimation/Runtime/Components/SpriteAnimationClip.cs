using Unity.Entities;

namespace TD.Features.SpriteAnimation.Runtime.Components
{
	public struct SpriteAnimationClip : IBufferElementData
	{
		public int startIndex;
		public int count;
	} 
}
