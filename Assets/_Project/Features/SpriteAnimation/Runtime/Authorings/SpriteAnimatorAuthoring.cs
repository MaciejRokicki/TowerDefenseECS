using System;
using TD.Features.SpriteAnimation.Runtime.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TD.Features.SpriteAnimation.Runtime.Authorings
{
    [Serializable]
    public struct SpriteAnimationClipFrames
    {
        public Sprite[] spriteFrames;
    }

    public class SpriteAnimatorAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Texture2D spriteSheetTexture;

        [SerializeField]
        private int gridPixelSize;
        [SerializeField]
        private SpriteAnimationClipFrames[] animations;

        class Baker : Baker<SpriteAnimatorAuthoring>
        {
            public override void Bake(SpriteAnimatorAuthoring authoring)
            {
                Vector2 texelSize = DependsOn(authoring.spriteSheetTexture).texelSize;

                var entity = GetEntity(TransformUsageFlags.Renderable);

                AddComponent(entity, new MaterialOverrideOffsetScale()
                {
                    Offset = authoring.animations.Length > 0 && authoring.animations[0].spriteFrames.Length > 0
                        ? authoring.animations[0].spriteFrames[0].rect.position * texelSize
                        : float2.zero,
                    Scale = new float2(texelSize * authoring.gridPixelSize)
                });

                var frameElements = AddBuffer<SpriteFrameElement>(entity);
                var animationClips = AddBuffer<SpriteAnimationClip>(entity);

                foreach (SpriteAnimationClipFrames animation in authoring.animations)
                {
                    animationClips.Add(new SpriteAnimationClip()
                    {
                        startIndex = frameElements.Length,
                        count = animation.spriteFrames.Length
                    });

                    foreach (var spriteFrame in animation.spriteFrames)
                    {
                        frameElements.Add(new SpriteFrameElement()
                        {
                            offset = spriteFrame.rect.position * texelSize
                        });
                    }
                }

                AddComponent(entity, new SpriteCurrentAnimationSelected()
                {
                    AnimationIndex = 2
                });
            }
        }
    }
}