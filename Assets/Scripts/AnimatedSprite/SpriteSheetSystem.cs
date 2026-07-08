using Unity.Burst;
using Unity.Entities;

partial struct SpriteSheetSystem : ISystem
{
    private const int FRAME_RATE = 10;

    [BurstCompile]
    public partial struct AnimateSpritesJob : IJobEntity
    {
        public int time;

        void Execute(
            ref MaterialOverrideOffsetScale materialOverride,
            ref SpriteCurrentAnimationSelected animationSelected,
            ref DynamicBuffer<SpriteFrameElement> spriteFrames,
            ref DynamicBuffer<SpriteAnimationClip> animationClips)
        {
            var frameIndex = time % animationClips[animationSelected.AnimationIndex].count;
            var frame = spriteFrames[animationClips[animationSelected.AnimationIndex].startIndex + frameIndex];
            materialOverride = new MaterialOverrideOffsetScale
            {
                Offset = frame.offset,
                Scale = materialOverride.Scale
            };
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int time = (int)(SystemAPI.Time.ElapsedTime * FRAME_RATE);

        new AnimateSpritesJob()
        {
            time = time,
        }.ScheduleParallel();
    }
}