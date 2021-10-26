using System.Drawing;

namespace FallingBlocks.Engine.Core.Core
{
    /// <summary>
    /// Base class for a game.
    /// </summary>
    public abstract class AGame
    {
        private static readonly Size DefaultWindowSize = new Size(1024, 768);

        /// <summary>
        /// Every game should provide a name.
        /// </summary>
        /// <returns></returns>
        public abstract string GetName();

        /// <summary>
        /// The icon for the main window.
        /// </summary>
        public abstract Icon GetIcon();

        /// <summary>
        /// The scene graph of the game.
        /// </summary>
        /// <returns></returns>
        public abstract ASceneGraph GetScene();

        /// <summary>
        /// Determines if the game requires landscape or portrait.
        /// </summary>
        /// <returns></returns>
        public virtual Size GetWindowSize()
        {
            return DefaultWindowSize;
        }
    }
}