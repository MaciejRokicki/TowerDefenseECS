using Unity.Entities;

namespace TD.Features.SpriteAnimation.Runtime.Components
{
	public struct SpriteCurrentAnimationSelected : IComponentData
	{
		public int AnimationIndex;
	} 
}
