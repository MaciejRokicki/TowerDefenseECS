using Unity.Entities;

namespace TD.Logic.Components
{
	public struct SpriteAnimationClip : IBufferElementData
	{
		public int startIndex;
		public int count;
	} 
}
