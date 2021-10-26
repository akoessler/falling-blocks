namespace FallingBlocks.Engine.Core.Core.Events
{
    /// <summary>
    /// Simply removes the object from the render tree.
    /// It's handy if you e.g hava a fade out animation (ChangeOpacity) and after the object is no longer
    /// visible simply remove it.
    /// </summary>
    public class OnEvent : ABehavior
    {
        private EventType expectedEventType;
        private ABehavior actualBehavior;

        public OnEvent(EventType expectedEventType)
        {
            this.expectedEventType = expectedEventType;
        }

        public override T AddBehavior<T>(T beh)
        {
            actualBehavior = beh;
            return beh;
        }

        /// <inheritdoc/>
        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            if (ro.Root.IsPressed(this.expectedEventType))
            {
                if (this.actualBehavior != null)
                {
                    this.actualBehavior.UpdateObject(timestamp, ro);
                }

                return false;
            }

            return false;
        }
    }
}