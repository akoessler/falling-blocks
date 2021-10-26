using FallingBlocks.Engine.Core.Core.Behavior;

namespace FallingBlocks.Engine.Core.Core
{
    public abstract class ABehavior : IObjectWithBehavior
    {
        protected bool AutoStart { get; set; }

        protected long StartTime { get; set; }

        private bool loop = false;

        private ABehavior previosBehavior;
        public ABehavior nextBehavior;
        public bool Finished { get; private set; }

        protected ABehavior()
        {
            this.AutoStart = true;
        }

        public ABehavior After(ABehavior behavior)
        {
            this.nextBehavior = behavior;
            if (behavior != null)
            {
                behavior.previosBehavior = this;
            }

            return behavior;
        }

        public IObjectWithBehavior Loop()
        {
            this.loop = true;
            return this;
        }

        private ABehavior GetRootBehavior()
        {
            if (this.previosBehavior != null)
            {
                return this.previosBehavior.GetRootBehavior();
            }

            return this;
        }

        public virtual void Reset()
        {
            this.StartTime = 0;
            this.Finished = false;
            if (this.nextBehavior != null)
            {
                this.nextBehavior.Reset();
            }
        }

        protected virtual void OnStart(long timestamp, ARenderObject ro)
        {
            this.StartTime = timestamp;
        }

        /// <summary>
        /// Is called, whenever a
        /// </summary>
        public bool UpdateObject(long timestamp, ARenderObject ro)
        {
            if (this.Finished && this.nextBehavior != null)
            {
                return this.nextBehavior.UpdateObject(timestamp, ro);
            }

            if (this.StartTime == 0 && this.AutoStart)
            {
                this.OnStart(timestamp, ro);
            }

            bool finished = this.UpdateOjectInternal(timestamp, ro);
            if (finished)
            {
                this.StartTime = 0;
                this.Finished = true;
                if (this.loop && this.nextBehavior == null)
                {
                    this.GetRootBehavior().Reset();
                }
            }

            return finished && this.nextBehavior == null && !this.loop;
        }

        /// <summary>
        /// Delivers the duration in milliseconds.
        /// </summary>
        protected long getDurationMs(long timeStamp)
        {
            return (timeStamp - this.StartTime);
        }

        protected float getRelativeProgress(long timeStamp, long maxDurationMs)
        {
            long curDur = this.getDurationMs(timeStamp);
            //Console.WriteLine("" + timeStamp +" => dur: " + curDur);
            var ret = (float) curDur / (float) maxDurationMs;
            if (ret > 1.0f)
            {
                return 1.0f;
            }

            return ret;
        }

        public virtual T AddBehavior<T>(T beh) where T : ABehavior
        {
            After(beh);
            return beh;
        }

        protected abstract bool UpdateOjectInternal(long timestamp, ARenderObject ro);
    }
}