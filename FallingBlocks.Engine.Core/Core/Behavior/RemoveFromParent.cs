namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Simply removes the object from the render tree.
    /// It's handy if you e.g hava a fade out animation (ChangeOpacity) and after the object is no longer
    /// visible simply remove it.
    /// </summary>
    public class RemoveFromParent : ABehavior
    {
        /// <inheritdoc/>
        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            // We remove the object from its parent, so that is not rendered anymore.
            // Remark: We do this within the after update, to ensure that we are allowed to modify the render tree.
            ro.Root.AfterUpdate += () => ro.Parent.Remove(ro);

            // As this behavior is immediatelly finished, return true.
            return true;
        }
    }
}