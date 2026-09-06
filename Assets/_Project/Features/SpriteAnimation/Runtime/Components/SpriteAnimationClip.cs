using Unity.Entities;

namespace TD.Features.SpriteAnimation.Components
{
	public struct SpriteAnimationClip : IBufferElementData
	{
		public int startIndex;
		public int count;
	} 
}
