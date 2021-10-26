namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// An object capable of a behavior being added.
    /// </summary>
    public interface IObjectWithBehavior
    {
        T AddBehavior<T>(T beh) where T : ABehavior;

        IObjectWithBehavior Loop();
    }
}