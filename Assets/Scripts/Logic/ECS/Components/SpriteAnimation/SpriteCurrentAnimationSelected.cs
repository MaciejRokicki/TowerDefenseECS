using Unity.Entities;

namespace TD.Logic.ECS.Components.SpriteAnimation
{
	public struct SpriteCurrentAnimationSelected : IComponentData
	{
		public int AnimationIndex;
	} 
}
