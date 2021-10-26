namespace FallingBlocks.Engine.Core.Core
{
    /// <summary>
    /// Starts and displays a scene graph.
    /// 
    /// This interface should be implemented by all
    /// platform specific Game implementations.
    /// </summary>
    public interface IGameLauncher
    {
        /// <summary>
        /// Start and display the given scene graph.
        /// </summary>
        /// <param name="sceneGraph"></param>
        void Start(AGame sceneGraph);
    }
}