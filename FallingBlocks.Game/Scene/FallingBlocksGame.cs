using System.Drawing;
using FallingBlocks.Engine.Core.Core;

namespace FallingBlocks.Game.Scene
{
    public class FallingBlocksGame : AGame
    {
        private readonly FallingBlocksGameScene scene = new FallingBlocksGameScene();

        /// <inheritdoc/>
        public override string GetName()
        {
            return "Falling Blocks";
        }

        public override Icon GetIcon()
        {
            return Resources.Icon;
        }

        /// <inheritdoc/>
        public override ASceneGraph GetScene()
        {
            return this.scene;
        }

        /// <inheritdoc/>
        public override Size GetWindowSize()
        {
            return new Size(1200, 900);
        }
    }
}
