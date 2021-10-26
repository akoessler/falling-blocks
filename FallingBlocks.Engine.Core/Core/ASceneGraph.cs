using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FallingBlocks.Engine.Core.Core.Events;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Core.Resource;

namespace FallingBlocks.Engine.Core.Core
{
    public abstract class ASceneGraph
    {
        private const int EventResetTimeEscape = 300; // Reset time for key events, after this ammount of milliseconds a new input is handled.

        private readonly DateTime start = DateTime.UtcNow;
        private long lastTimestamp;
        private long virtualTimestamp;
        private IKeyEventManager keyEventManager;
        private long nextAcceptTime_Escape;
        private bool isPauseByEscape;
        private bool isPauseByFocus;
        private bool isGameOver;
        private string gameOverText;
        private readonly ImageResource gameOverImageResource;
        private readonly ImageResource pauseImageResource;

        /// <summary>
        /// Resources.
        /// </summary>
        private readonly ResourceCollection resources;

        /// <summary>
        /// Render objects.
        /// </summary>
        private readonly RenderObjectCollection renderObjects;

        /// <summary>
        /// Some behaviors require to remove objects from the render tree, this is not safe
        /// if we are enumerating them within the update cycle, so we need a safe point for doing such a modification.
        /// </summary>
        public Action AfterUpdate
        {
            get { return this.renderObjects.AfterUpdate; }
            set { this.renderObjects.AfterUpdate = value; }
        }

        public bool IsGameOver
        {
            get { return this.isGameOver; }
        }

        /// <summary>
        /// ctor.
        /// </summary>
        protected ASceneGraph()
        {
            this.resources = new ResourceCollection();
            this.renderObjects = new RenderObjectCollection();
            this.renderObjects.Root = this;

            this.gameOverImageResource = this.AddResource(new ImageResource(Resources.GameOver));
            this.pauseImageResource = this.AddResource(new ImageResource(Resources.Pause));
        }

        /// <summary>
        /// Initializes the SceneGraph with the render context.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Init(IRenderContext context)
        {
            context.Init(this);
        }

        public Color? Background
        {
            get { return this.renderObjects.Color; }
            set { this.renderObjects.Color = value; }
        }

        public void SetKeyEventManager(IKeyEventManager newKeyEventManager)
        {
            this.keyEventManager = newKeyEventManager;
        }

        public bool IsPressed(EventType eventType)
        {
            return this.keyEventManager != null && this.keyEventManager.IsPressed(eventType);
        }

        public T AddResource<T>(T resource) where T : AResource
        {
            this.resources.Add(resource);
            return resource;
        }

        public T RemoveResource<T>(T resource) where T : AResource
        {
            this.resources.Remove(resource);
            return resource;
        }

        public T AddObject<T>(T resource) where T : ARenderObject
        {
            this.renderObjects.Add(resource);
            return resource;
        }

        public T RemoveObject<T>(T resource) where T : ARenderObject
        {
            this.renderObjects.Remove(resource);
            return resource;
        }

        /// <summary>
        /// Should be called in the render loop.
        /// </summary>
        public void RenderOneFrame(IRenderContext context)
        {
            var span = DateTime.UtcNow - this.start;
            var timestampMs = (long) span.TotalMilliseconds;
            var elapsedMsSinceLastLoop = timestampMs - this.lastTimestamp;
            this.lastTimestamp = timestampMs;

            bool isNowPause = false;
            if (this.isGameOver)
            {
                if (this.IsEscapePressed(timestampMs))
                {
                    this.ResetGameOver();
                    this.Init(context);
                    return;
                }
            }
            else
            {
                isNowPause = this.CalcPause(timestampMs);
            }

            if (!isNowPause && !this.isGameOver)
            {
                this.virtualTimestamp += elapsedMsSinceLastLoop;

                // Let the subclass do some updates:
                this.UpdateOneFrame(context, this.virtualTimestamp, elapsedMsSinceLastLoop);

                // Update the objects:
                this.renderObjects.Update(this.virtualTimestamp);
            }

            // Render the objects:
            context.BeginRender();

            // So we can dynamically add resources as well.
            if (!this.resources.IsPrepared)
            {
                this.resources.Prepare(context);
            }

            // Render the objects.
            this.renderObjects.Render(context);

            if (this.isGameOver)
            {
                this.DrawDarkBackground(context);
                this.DrawGameOver(context);
            }
            else if (isNowPause)
            {
                this.DrawDarkBackground(context);
                this.DrawPause(context);
            }

            context.EndRender();
        }

        private void DrawDarkBackground(IRenderContext context)
        {
            var rectangle = new Rectangle2D();
            rectangle.Color = Color.Black;
            var bounds = context.GetScreenBounds();
            rectangle.Size = bounds.Size;
            rectangle.Position = bounds.Location;
            rectangle.Opacity = 0.8f;

            rectangle.Render(context);
        }

        private void DrawPause(IRenderContext context)
        {
            var overlayImage = new Image2D(this.pauseImageResource);
            overlayImage.Render(context);
        }

        private void DrawGameOver(IRenderContext context)
        {
            var font = Text2D.TextFont.Helvetica_18;

            float width, height;
            context.CalcTextSize(font, this.gameOverText, out width, out height);

            var text = new Text2D();
            text.Text = this.gameOverText;
            text.Font = font;
            text.Color = Color.White;
            text.Position = new PointF(-width / 2f, height / 2f);
            text.Render(context);

            var overlayImage = new Image2D(this.gameOverImageResource);
            var size = overlayImage.Image.Data.Size;
            overlayImage.Position = new PointF(0, -size.Height / 2f);
            overlayImage.Render(context);
        }

        protected virtual void UpdateOneFrame(IRenderContext context, long timestampMs, long elapsedMsSinceLastLoop)
        {
        }

        private bool CalcPause(long timestampMs)
        {
            // switch pause with escape:
            bool escapePressed = this.IsEscapePressed(timestampMs);
            if (escapePressed)
            {
                this.isPauseByEscape = !this.isPauseByEscape;
            }

            // also set to pause if the game loses focus
            this.isPauseByFocus = !this.keyEventManager.HasFocus();

            return this.isPauseByEscape || this.isPauseByFocus;
        }

        private bool IsEscapePressed(long timestampMs)
        {
            bool escapePressed = this.IsPressed(EventType.Escape);
            if (escapePressed)
            {
                if (timestampMs >= this.nextAcceptTime_Escape)
                {
                    this.nextAcceptTime_Escape = timestampMs + ASceneGraph.EventResetTimeEscape;
                    return true;
                }
            }
            else
            {
                this.nextAcceptTime_Escape = 0;
            }

            return false;
        }

        protected void GameOver(int level, int points, Dictionary<string, int> additionalStatistics)
        {
            this.isGameOver = true;

            var sb = new StringBuilder();
            sb.Append("Press ESCAPE to restart").AppendLine().AppendLine();

            var statistics = new Dictionary<string, int>();
            statistics.Add("Reached Level", level);
            statistics.Add("Earned Points", points);

            if (additionalStatistics != null)
            {
                foreach (var stat in additionalStatistics)
                {
                    statistics.Add(stat.Key, stat.Value);
                }
            }

            int maxKeyLength = statistics.Select(x => x.Key.Length).Max();

            foreach (var stat in statistics)
            {
                string fill = new string(' ', maxKeyLength - stat.Key.Length);
                sb.Append(stat.Key).Append(": ").Append(fill).Append(stat.Value).AppendLine();
            }

            this.gameOverText = sb.ToString();
        }

        private void ResetGameOver()
        {
            this.isGameOver = false;
            this.gameOverText = null;
        }
    }
}