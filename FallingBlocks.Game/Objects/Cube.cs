using System;
using System.Drawing;
using System.Threading;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Util;
using FallingBlocks.Game.Scene;

namespace FallingBlocks.Game.Objects
{
    internal class Cube : IFallingBlocksObject
    {
        private static int cubeCounter;
        private int cubeIndex;

        private ShapeStyle shapeStyle;
        private Image2D image;
        private PointF position;
        private Shape shape;

        public PointF Position
        {
            get
            {
                return this.shape != null ? this.shape.ShapePosition.Add(this.position) : this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public PointF RelativePosition
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public float Opacity
        {
            get { return this.image.Opacity; }
            set { this.image.Opacity = value; }
        }

        public Cube(Shape shape, ShapeStyle shapeStyle, Point startPosition)
        {
            this.cubeIndex = Interlocked.Increment(ref cubeCounter);

            this.shape = shape;
            this.shapeStyle = shapeStyle;
            this.position = startPosition;
            this.image = CreateImage(shapeStyle);
            this.image.Name = this + "::Image";
        }

        public override string ToString()
        {
            return "Cube#" + this.cubeIndex + ", " + this.shapeStyle + ", " + this.Position;
        }

        private static Image2D CreateImage(ShapeStyle shapeStyle)
        {
            switch (shapeStyle)
            {
                case ShapeStyle.I:
                    return new Image2D(FallingBlocksGameScene.CubeCyan);
                case ShapeStyle.L:
                    return new Image2D(FallingBlocksGameScene.CubeOrange);
                case ShapeStyle.J:
                    return new Image2D(FallingBlocksGameScene.CubeBlue);
                case ShapeStyle.O:
                    return new Image2D(FallingBlocksGameScene.CubeYellow);
                case ShapeStyle.S:
                    return new Image2D(FallingBlocksGameScene.CubeLime);
                case ShapeStyle.T:
                    return new Image2D(FallingBlocksGameScene.CubeMagenta);
                case ShapeStyle.Z:
                    return new Image2D(FallingBlocksGameScene.CubeRed);
                default:
                    throw new ArgumentOutOfRangeException("shapeStyle", shapeStyle, null);
            }
        }

        public void AddToScene(FallingBlocksGameScene scene)
        {
            scene.AddObject(this.image);
        }

        public void RemoveFromScene(FallingBlocksGameScene scene)
        {
            scene.RemoveObject(this.image);
        }

        public void UpdateImagePositions(FallingBlocksGameScene scene)
        {
            var cubePosition = this.Position;

            var newImagePosition = new PointF
            (
                scene.GameFieldLeft + cubePosition.X * FallingBlocksGameScene.ImageSize,
                scene.GameFieldTop + cubePosition.Y * FallingBlocksGameScene.ImageSize
            );

            this.image.Position = newImagePosition;
            this.image.Scale = FallingBlocksGameScene.ImageSize / (float)this.image.Height;
        }

        public void SplitFromShape()
        {
            this.position = this.Position;
            this.shape = null;
        }
    }
}