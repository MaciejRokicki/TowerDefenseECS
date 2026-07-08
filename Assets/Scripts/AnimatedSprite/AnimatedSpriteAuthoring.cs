using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[Serializable]
public struct AnimationCollection
{
    public Sprite[] spriteFrames;
}

[MaterialProperty("_OffsetXYScaleZW")]
public struct MaterialOverrideOffsetScale : IComponentData
{
    public float2 Offset;
    public float2 Scale;
}

public struct SpriteCurrentAnimationSelected : IComponentData
{
    public int AnimationIndex;
}

public struct SpriteAnimationClip : IBufferElementData
{
    public int startIndex;
    public int count;
}

public struct SpriteFrameElement : IBufferElementData
{
    public float2 offset;
}

public class AnimatedSpriteAuthoring : MonoBehaviour
{
    [SerializeField]
    private Texture2D spriteSheetTexture;

    [SerializeField]
    private int gridPixelSize;
    [SerializeField]
    private AnimationCollection[] animations;

    class Baker : Baker<AnimatedSpriteAuthoring>
    {
        public override void Bake(AnimatedSpriteAuthoring authoring)
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

            foreach (AnimationCollection animation in authoring.animations)
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

            AddComponent<SpriteCurrentAnimationSelected>(entity);
        }
    }
}