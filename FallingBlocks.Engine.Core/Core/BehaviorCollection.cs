using System.Collections.Generic;

namespace FallingBlocks.Engine.Core.Core
{
    public class BehaviorCollection : ABehavior
    {
        public List<ABehavior> Behaviors { get; private set; }

        public BehaviorCollection()
        {
            this.Behaviors = new List<ABehavior>();
        }

        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            List<ABehavior> behaviors2Remove = null;
            foreach (var cur in this.Behaviors)
            {
                if (cur.UpdateObject(timestamp, ro))
                {
                    if (behaviors2Remove == null)
                    {
                        behaviors2Remove = new List<ABehavior>();
                    }

                    behaviors2Remove.Add(cur);
                }
            }

            if (behaviors2Remove != null)
            {
                behaviors2Remove.ForEach(cur => this.Behaviors.Remove(cur));
            }

            return this.Behaviors.Count == 0;
        }
    }
}