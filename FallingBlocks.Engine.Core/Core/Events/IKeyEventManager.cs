namespace FallingBlocks.Engine.Core.Core.Events
{
    public interface IKeyEventManager
    {
        bool HasFocus();
        bool IsPressed(EventType eventType);
    }
}