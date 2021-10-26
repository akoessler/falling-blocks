using FallingBlocks.Game.Scene;

namespace FallingBlocks.Game.Objects
{
    internal interface IFallingBlocksObject
    {
        void AddToScene(FallingBlocksGameScene scene);
        void RemoveFromScene(FallingBlocksGameScene scene);
        void UpdateImagePositions(FallingBlocksGameScene scene);
    }
}