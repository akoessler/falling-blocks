using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using FallingBlocks.Engine.Core.Util;
using FallingBlocks.Game.Scene;

namespace FallingBlocks.Game.Objects
{
    internal class Shape : IFallingBlocksObject
    {
        private static int shapeCounter;
        private int shapeIndex;

        public ShapeStyle ShapeStyle;
        public PointF ShapePosition;
        public List<Cube> cubes;
        public Shape GhostShape;

        public RectangleF BoundingRectangle
        {
            get
            {
                var x = this.cubes.Select(cube => cube.Position.X).Min();
                var y = this.cubes.Select(cube => cube.Position.Y).Min();
                var x2 = this.cubes.Select(cube => cube.Position.X).Max();
                var y2 = this.cubes.Select(cube => cube.Position.Y).Max();
                return new RectangleF(x, y, x2 - x, y2 - y);
            }
        }

        public Shape(ShapeStyle shapeStyle, float opacity)
        {
            this.shapeIndex = Interlocked.Increment(ref shapeCounter);
            this.ShapeStyle = shapeStyle;
            this.cubes = new List<Cube>();

            switch (shapeStyle)
            {
                case ShapeStyle.I:
                    cubes.Add(new Cube(this, shapeStyle, new Point(-2, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    break;

                case ShapeStyle.L:
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 1)));
                    break;

                case ShapeStyle.J:
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 1)));
                    break;

                case ShapeStyle.O:
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 1)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 1)));
                    break;

                case ShapeStyle.S:
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 1)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 1)));
                    break;

                case ShapeStyle.T:
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 1)));
                    break;

                case ShapeStyle.Z:
                    cubes.Add(new Cube(this, shapeStyle, new Point(-1, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 0)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(0, 1)));
                    cubes.Add(new Cube(this, shapeStyle, new Point(1, 1)));
                    break;
            }

            foreach (var cube in this.cubes)
            {
                cube.Opacity = opacity;
            }
        }

        public override string ToString()
        {
            return "Shape#" + this.shapeIndex + ", " + this.ShapeStyle + ", " + this.ShapePosition;
        }

        public void AddToScene(FallingBlocksGameScene scene)
        {
            foreach (var cube in this.cubes)
            {
                cube.AddToScene(scene);
            }
        }

        public void RemoveFromScene(FallingBlocksGameScene scene)
        {
            foreach (var cube in this.cubes)
            {
                cube.RemoveFromScene(scene);
            }
        }

        public List<Cube> Split()
        {
            foreach (var cube in this.cubes)
            {
                cube.SplitFromShape();
            }

            return this.cubes;
        }

        public void UpdateImagePositions(FallingBlocksGameScene scene)
        {
            foreach (var cube in this.cubes)
            {
                cube.UpdateImagePositions(scene);
            }
        }

        public bool IsCurrentPositionBlocked(List<Cube> currentStaticCubes)
        {
            foreach (var shapeCube in this.cubes)
            foreach (var cube in currentStaticCubes)
            {
                if (FloatHelper.FloatEquals(shapeCube.Position.X, cube.Position.X))
                {
                    if (FloatHelper.FloatEquals(shapeCube.Position.Y, cube.Position.Y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private int currentRotationIndex;

        public void RotateLeft()
        {
            if (this.ShapeStyle != ShapeStyle.O)
            {
                foreach (var cube in this.cubes)
                {
                    var x = cube.RelativePosition.X;
                    var y = cube.RelativePosition.Y;
                    cube.RelativePosition = new PointF(y, -x);
                }
            }

            if (this.ShapeStyle == ShapeStyle.I)
            {
                this.currentRotationIndex -= 1;
                if (this.currentRotationIndex < 0) this.currentRotationIndex = 3;

                switch (this.currentRotationIndex)
                {
                    case 0:
                        this.ShapePosition = this.ShapePosition.Add(0, -1);
                        break;
                    case 1:
                        this.ShapePosition = this.ShapePosition.Add(1, 0);
                        break;
                    case 2:
                        this.ShapePosition = this.ShapePosition.Add(0, 1);
                        break;
                    case 3:
                        this.ShapePosition = this.ShapePosition.Add(-1, 0);
                        break;
                }
            }
        }

        public void RotateRight()
        {
            if (this.ShapeStyle != ShapeStyle.O)
            {
                foreach (var cube in this.cubes)
                {
                    var x = cube.RelativePosition.X;
                    var y = cube.RelativePosition.Y;
                    cube.RelativePosition = new PointF(-y, x);
                }
            }

            if (this.ShapeStyle == ShapeStyle.I)
            {
                this.currentRotationIndex += 1;
                if (this.currentRotationIndex > 3) this.currentRotationIndex = 0;

                switch (this.currentRotationIndex)
                {
                    case 0:
                        this.ShapePosition = this.ShapePosition.Add(1, 0);
                        break;
                    case 1:
                        this.ShapePosition = this.ShapePosition.Add(0, 1);
                        break;
                    case 2:
                        this.ShapePosition = this.ShapePosition.Add(-1, 0);
                        break;
                    case 3:
                        this.ShapePosition = this.ShapePosition.Add(0, -1);
                        break;
                }
            }
        }

        public void SetOpacity(float opacity)
        {
            foreach (var cube in this.cubes)
            {
                cube.Opacity = opacity;
            }
        }
    }
}