using Unity.Entities;

namespace TD.Logic.ECS.Components.SpriteAnimation
{
	public struct SpriteAnimationClip : IBufferElementData
	{
		public int startIndex;
		public int count;
	} 
}
