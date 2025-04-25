namespace ScaleShooter
{
    public readonly struct ScaleInfo
    {
        public readonly float Size;
        public readonly float Duration;

        public ScaleInfo(float size, float duration)
        {
            Size = size;
            Duration = duration;
        }
    }
}