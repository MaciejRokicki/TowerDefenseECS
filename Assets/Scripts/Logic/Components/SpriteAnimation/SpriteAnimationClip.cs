using Unity.Entities;

namespace TD.Logic.Components.SpriteAnimation
{
	public struct SpriteAnimationClip : IBufferElementData
	{
		public int startIndex;
		public int count;
	} 
}
