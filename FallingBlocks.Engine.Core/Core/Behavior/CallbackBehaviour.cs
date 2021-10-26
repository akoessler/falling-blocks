using System;

namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Invokes the given callback
    /// </summary>
    public class CallbackBehaviour : ABehavior
    {
        private readonly Action callback;

        public CallbackBehaviour(Action callback)
            : base()
        {
            this.callback = callback;
        }

        /// <summary>
        /// Immediately invokes the given callback, then exits (return == TRUE)
        /// </summary>
        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            callback();
            return true;
        }
    }
}