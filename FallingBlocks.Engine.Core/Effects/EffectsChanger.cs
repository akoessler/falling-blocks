using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Core.Behavior;
using FallingBlocks.Engine.Core.Core.Primitive;

namespace FallingBlocks.Engine.Core.Effects
{
    public static class EffectsChanger
    {
        /// <summary>
        /// Starts a particle animation, and a fade out and removes the object is finished.
        /// </summary>
        /// <returns></returns>
        public static ARenderObject Explode(this ARenderObject renderObject)
        {
            renderObject.ChangeOpacity(0.0f, 1000)
                .AddBehavior(new RemoveFromParent());
            renderObject.Add(new Particles(renderObject.Root));
            return renderObject;
        }
    }
}