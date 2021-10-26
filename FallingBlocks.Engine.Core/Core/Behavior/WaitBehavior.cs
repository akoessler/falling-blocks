namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Changes a member of a render object to an absolute end value.
    /// The change is animated.
    /// </summary>
    public class WaitBehavior : ABehavior
    {
        private long durationMs;

        public WaitBehavior(long durationMs)
        {
            this.durationMs = durationMs;
        }

        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            return this.durationMs < this.getDurationMs(timestamp);
        }
    }
}