using System;
using System.Drawing;
using FallingBlocks.Engine.Core.Core.Resource;

namespace FallingBlocks.Engine.Core.Core.Primitive
{
    /// <summary>
    /// For simple explosion or so effects it is handy to have a particle object.
    /// </summary>
    public class Particles : ARenderObject
    {
        private float slowdown = 1.0f; // Slow Down Particles
        private Particle[] particles;
        private Image2D imageRenderObject;
        private bool finished;
        private Random random = new Random();

        public Particles(ImageResource texture, int count)
        {
            this.imageRenderObject = new Image2D(texture);
            this.particles = new Particle[count];
            this.Reset();
        }

        public Particles(ASceneGraph sceneGraph, int count)
            : this(sceneGraph.AddResource(new ImageResource(Resources.Particle)), count)
        {
        }

        public Particles(ASceneGraph sceneGraph)
            : this(sceneGraph, 99)
        {
        }


        public void Reset()
        {
            this.finished = false;
            for (int loop = 0; loop < particles.Length; loop++)
            {
                particles[loop] = new Particle();
                particles[loop].active = true; // Make All The Particles Active
                particles[loop].life = 1.0f; // Give All The Particles Full Life
                particles[loop].fade = (float) (random.Next(100)) / 300.0f + 0.03f; // Random Fade Speed

                particles[loop].r =
                    colors[loop * (int) ((float) 12 / (float) particles.Length), 0]; // Select Red Rainbow Color
                particles[loop].g =
                    colors[loop * (int) ((float) 12 / (float) particles.Length), 1]; // Select Red Rainbow Color
                particles[loop].b =
                    colors[loop * (int) ((float) 12 / (float) particles.Length), 2]; // Select Red Rainbow Color

                particles[loop].xi = (float) ((random.Next(50) - 26.0f) * 100.0f); // Random Speed On X Axis
                particles[loop].yi = (float) ((random.Next(50) - 25.0f) * 100.0f); // Random Speed On Y Axis
                particles[loop].zi = (float) ((random.Next(50) - 25.0f) * 100.0f); // Random Speed On Z Axis

                /*
                particles[loop].xg = 0.0f;                        // Set Horizontal Pull To Zero
                particles[loop].yg = 0.0f;                    // Set Vertical Pull Downward
                particles[loop].zg = 0.0f;                        // Set Pull On Z Axis To Zero
                 * */
            }
        }

        protected override void RenderInternal(IRenderContext context)
        {
            if (this.finished)
            {
                return;
            }

            var curPos = this.Position;
            int activeCount = 0;

            for (int loop = 0; loop < particles.Length; loop++) // Loop Through All The Particles
            {
                if (particles[loop].active) // If The Particle Is Active
                {
                    ++activeCount;
                    float x = curPos.X + particles[loop].x; // Grab Our Particle X Position
                    float y = curPos.Y + particles[loop].y; // Grab Our Particle Y Position

                    // Draw The Particle Using Our RGB Values, Fade The Particle Based On It's Life
                    //gl.Color(particles[loop].r, particles[loop].g, particles[loop].b, particles[loop].life);

                    this.imageRenderObject.Position = new PointF(x, y);
                    //this.imageRenderObject.Scale = 0.5f;
                    this.imageRenderObject.Opacity = particles[loop].life;
                    this.imageRenderObject.Render(context);

                    particles[loop].x += particles[loop].xi / (slowdown * 1000); // Move On The X Axis By X Speed
                    particles[loop].y += particles[loop].yi / (slowdown * 1000); // Move On The Y Axis By Y Speed

                    /*
                    particles[loop].xi += particles[loop].xg;            // Take Pull On X Axis Into Account
                    particles[loop].yi += particles[loop].yg;            // Take Pull On Y Axis Into Account
                    particles[loop].zi += particles[loop].zg;            // Take Pull On Z Axis Into Account
                    */
                    particles[loop].life -= particles[loop].fade; // Reduce Particles Life By 'Fade'

                    // Re alive the particel, we are not doing this right now.
                    if (particles[loop].life < 0.0f) // If Particle Is Burned Out
                    {
                        particles[loop].active = false;
                        /*
                        particles[loop].life = 1.0f;                // Give It New Life
                        particles[loop].fade = (float)(random.Next(100)) / 1000.0f + 0.003f;        // Random Fade Speed

                        particles[loop].x = 0.0f;                    // Center On X Axis
                        particles[loop].y = 0.0f;                    // Center On Y Axis
                        particles[loop].z = 0.0f;                    // Center On Z Axis

                        particles[loop].xi = (float)((random.Next(50) - 26.0f) * 10.0f);        // Random Speed On X Axis
                        particles[loop].yi = (float)((random.Next(50) - 25.0f) * 10.0f);        // Random Speed On Y Axis
                        particles[loop].zi = (float)((random.Next(50) - 25.0f) * 10.0f);        // Random Speed On Y Axis

                        int colour = random.Next(12);
                        particles[loop].r = colors[colour, 0];            // Select Red From Color Table
                        particles[loop].g = colors[colour, 1];            // Select Green From Color Table
                        particles[loop].b = colors[colour, 2];            // Select Blue From Color Table
                         * */
                    }
                }
            }

            if (activeCount == 0)
            {
                this.finished = true;
                Reset();
            }
        }

        private class Particle
        {
            public bool active; // Active (Yes/No)
            public float life; // Particle Life
            public float fade; // Fade Speed

            public float r; // Red Value
            public float g; // Green Value
            public float b; // Blue Value

            public float x; // X Position
            public float y; // Y Position

            public float xi; // X Direction
            public float yi; // Y Direction
            public float zi; // Z Direction

            /*
            public float xg;                    // X Gravity
            public float yg;                    // Y Gravity
            public float zg;                    // Z Gravity
             * */
        }


        readonly float[,] colors = new float[,]
        {
            {1.0f, 0.5f, 0.5f}, {1.0f, 0.75f, 0.5f}, {1.0f, 1.0f, 0.5f}, {0.75f, 1.0f, 0.5f},
            {0.5f, 1.0f, 0.5f}, {0.5f, 1.0f, 0.75f}, {0.5f, 1.0f, 1.0f}, {0.5f, 0.75f, 1.0f},
            {0.5f, 0.5f, 1.0f}, {0.75f, 0.5f, 1.0f}, {1.0f, 0.5f, 1.0f}, {1.0f, 0.5f, 0.75f}
        };
    }
}