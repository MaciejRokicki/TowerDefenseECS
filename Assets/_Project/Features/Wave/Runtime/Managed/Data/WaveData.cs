namespace TD.Features.Wave.Managed.Data
{
    public class WaveData
    {
        public int Wave { get; internal set; }
        public int Power { get; internal set; }
        public WaveState State { get; internal set; }
        public float WaveTime { get; internal set; }
        public float WaveDuration { get; internal set; }
        public float BreakTime { get; internal set; }
        public float BreakDuration { get; internal set; }
    }
}