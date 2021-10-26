using System;
using System.Collections.Generic;
using System.Drawing;
using FallingBlocks.Engine.Core.Core.Behavior;
using FallingBlocks.Engine.Core.Core.Propertyaccess;
using FallingBlocks.Engine.Core.Util;

namespace FallingBlocks.Engine.Core.Core
{
    /// <summary>
    /// Base class for all render objects.
    /// </summary>
    public abstract class ARenderObject : IObjectWithBehavior
    {
        public static PropertyAccessFloat<ARenderObject> RotationAccess =
            new PropertyAccessFloat<ARenderObject>(obj => obj.Rotation, (obj, value) => obj.Rotation = value);

        public static PropertyAccessFloat<ARenderObject> ScaleAccess =
            new PropertyAccessFloat<ARenderObject>(obj => obj.Scale, (obj, value) => obj.Scale = value);

        public static PropertyAccessFloat<ARenderObject> OpacityAccess =
            new PropertyAccessFloat<ARenderObject>(obj => obj.Opacity, (obj, value) => obj.Opacity = value);

        public static PropertyAccessFloat<ARenderObject> RotationSpeedAccess =
            new PropertyAccessFloat<ARenderObject>(obj => obj.RotationSpeed, (obj, value) => obj.RotationSpeed = value);

        public static PropertyAccessPointF<ARenderObject> PositionAccess =
            new PropertyAccessPointF<ARenderObject>(obj => obj.Position, (obj, value) => obj.Position = value);

        public static PropertyAccessPointF<ARenderObject> SpeedAccess =
            new PropertyAccessPointF<ARenderObject>(obj => obj.Speed, (obj, value) => obj.Speed = value);

        private List<ARenderObject> renderObjects;
        private bool hasChildren;

        private long previousTimeStamp;

        /// <summary>
        /// The sceene graph the object belongs to.
        /// </summary>
        public ASceneGraph Root
        {
            get { return this.root; }
            set
            {
                this.root = value;
                if (this.renderObjects != null)
                {
                    foreach (var obj in this.renderObjects)
                    {
                        obj.Root = value;
                    }
                }
            }
        }

        private ASceneGraph root;

        public ARenderObject Parent { get; private set; }

        /// <summary>
        /// For debugging it's handy to have a name of the object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Can hold arbitrary, user-defined data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Currently we only use the color of the root object to set the background color of our ogl window.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Position of the render object.
        /// </summary>
        public PointF Position { get; set; }

        /// <summary>
        /// If set the Position is changed every frame
        /// Speed is in delta position per second.
        /// </summary>
        public PointF Speed { get; set; }

        /// <summary>
        /// Rotation of the render object.
        /// Remark: the rotation point is always the center point of the sprite.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// If set the Position is changed every frame
        /// Speed is in degrees per second.
        /// </summary>
        public float RotationSpeed { get; set; }

        /// <summary>
        /// Scale of the renderobject.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Opacity.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Behaviors of the render object.
        /// </summary>
        public BehaviorCollection Behaviors { get; private set; }


        /// <summary>
        /// Center point for scale and rotate operations.
        /// Todo: Remove center if Position is the center.
        /// </summary>
        //public abstract PointF Center { get; }


        /// <summary>
        /// ctor.
        /// </summary>
        protected ARenderObject()
        {
            this.Behaviors = new BehaviorCollection();
            this.Scale = 1.0f;
            this.Opacity = 1.0f;
        }

        public T AddBehavior<T>(T beh) where T : ABehavior
        {
            this.Behaviors.Behaviors.Add(beh);
            return beh;
        }


        /// <summary>
        /// Updates the object.
        /// </summary>
        public virtual void Update(long timestamp)
        {
            // Before we update things via the behaviors we do the updates based on the speed.
            if (this.previousTimeStamp != 0)
            {
                float dTimeSeconds = (timestamp - this.previousTimeStamp) / 1000.0f;
                if (!FloatHelper.FloatEquals(this.RotationSpeed, 0.0f))
                {
                    this.Rotation += this.RotationSpeed * dTimeSeconds;
                    if (this.Rotation > 360f)
                    {
                        this.Rotation -= 360f;
                    }
                }

                if (!FloatHelper.FloatEquals(this.Speed.X, 0.0f) || !FloatHelper.FloatEquals(this.Speed.Y, 0.0f))
                {
                    this.Position = new PointF(
                        this.Position.X + this.Speed.X * dTimeSeconds,
                        this.Position.Y + this.Speed.Y * dTimeSeconds
                    );
                }
            }

            this.Behaviors.UpdateObject(timestamp, this);

            // I available, update child objects.
            if (this.hasChildren)
            {
                foreach (var cur in this.renderObjects)
                {
                    cur.Update(timestamp);
                }
            }

            this.previousTimeStamp = timestamp;
        }

        /// <summary>
        /// Adds a render object.
        /// </summary>
        public T Add<T>(T renderObject) where T : ARenderObject
        {
            if (this.renderObjects == null)
            {
                this.renderObjects = new List<ARenderObject>();
            }

            this.renderObjects.Add(renderObject);
            renderObject.Parent = this;
            if (!this.hasChildren)
            {
                this.hasChildren = true;
            }

            renderObject.Root = this.Root;
            return renderObject;
        }

        /// <summary>
        /// Removes a child object.
        /// </summary>
        public void Remove(ARenderObject renderObject)
        {
            if (this.renderObjects != null)
            {
                this.renderObjects.Remove(renderObject);
                renderObject.Parent = null;
                renderObject.Root = null;
                if (this.renderObjects.Count == 0)
                {
                    this.hasChildren = false;
                }
            }
        }

        /// <summary>
        /// Looping here is currently not supported.
        /// </summary>
        /// <returns></returns>
        public IObjectWithBehavior Loop()
        {
            throw new NotImplementedException();
        }


        public virtual void Render(IRenderContext context)
        {
            this.RenderInternal(context);

            //Console.WriteLine("Render:" + this.GetType().Name + ": " + this.Name);
            // Render possible children:
            if (this.hasChildren)
            {
                bool needRestore = context.Transform(this.Position, this.Scale, this.Rotation, this.Position);
                foreach (var renderObject in this.renderObjects)
                {
                    renderObject.Render(context);
                }

                if (needRestore)
                {
                    context.PopMatrix();
                }
            }
        }

        /// <summary>
        /// Is caled to render the object.
        /// </summary>
        protected abstract void RenderInternal(IRenderContext context);
    }
}