using FallingBlocks.Engine.Core.Core.Behavior;

namespace FallingBlocks.Engine.Core.Core.Events
{
    public static class EventHandling
    {
        public static IObjectWithBehavior On(this IObjectWithBehavior ro, EventType eventType)
        {
            return ro.AddBehavior(new OnEvent(eventType));
        }
    }
}