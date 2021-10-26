using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Render.Gdi;

namespace FallingBlocks.Engine.Windows.Gdi
{
    internal class GdiGameLoop
    {
        private const int ExpectedFps = 60;
        private const int ThreadSleepMs = 1; // too low: more cpu usage; too high: FPS get unstable as thread.sleep is not precise enough.

        private readonly GameLauncherGdi gameLauncher;
        private readonly GameForm window;
        private readonly GamePictureBox pictureBox;
        private readonly ASceneGraph scene;
        private readonly Bitmap windowBitmapBuffer;

        private double currentGameLoopIntervalMs;
        private double gameLoopSleepThresholdMs;
        private Stopwatch gameLoopStopwatch;

        internal GdiGameLoop(GameForm window, GamePictureBox pictureBox, GameLauncherGdi gameLauncher, ASceneGraph scene, Bitmap windowBitmapBuffer)
        {
            this.window = window;
            this.pictureBox = pictureBox;
            this.gameLauncher = gameLauncher;
            this.scene = scene;
            this.windowBitmapBuffer = windowBitmapBuffer;

            this.window.Shown += this.Window_Shown;
            this.window.Closing += this.Window_Closing;
        }

        private void Window_Shown(object sender, EventArgs e)
        {
            try
            {
                this.StartGameLoop();
            }
            catch (Exception)
            {
                this.gameLauncher.Stop();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.gameLauncher.Stop();
        }

        /// <summary>
        /// Starts the game loop, is started after the form is shown.
        /// </summary>
        private void StartGameLoop()
        {
            try
            {
                this.currentGameLoopIntervalMs = 1000.0 / GdiGameLoop.ExpectedFps;
                this.gameLoopSleepThresholdMs = this.currentGameLoopIntervalMs - GdiGameLoop.ThreadSleepMs * 2;

                // Initialize graphics
                this.graphics = Graphics.FromImage(this.windowBitmapBuffer);
                this.graphics.Clear(Color.White);
                this.renderContext = new GdiRenderContext(this.graphics);

                // Let the game do its initialization
                this.scene.Init(this.renderContext);

                // Let the show begin ...
                this.window.BeginInvoke((Action)this.GameLoop);
            }
            catch (Exception)
            {
                this.gameLauncher.Stop();
            }
        }

        /// <summary>
        /// The main game loop method.
        /// Takes over the gui thread to ensure a constant frame rate.
        /// </summary>
        private void GameLoop()
        {
            try
            {
                this.gameLoopStopwatch = Stopwatch.StartNew();

                while (this.gameLauncher.IsRunning)
                {
                    // As this method takes over the control for this thread, we need to process the window messages to support window or key events.
                    Win32.NativeMessage msg;
                    if (Win32.PeekMessage(out msg, IntPtr.Zero, 0, 0, (uint)Win32.PM.REMOVE))
                    {
                        // WindowMessage is pending -> process
                        if (msg.message == (uint)Win32.WindowsMessage.WM_QUIT || msg.message == (uint)Win32.WindowsMessage.WM_CLOSE)
                        {
                            this.gameLauncher.Stop();
                            break;
                        }
                        else
                        {
                            Win32.TranslateMessage(ref msg);
                            Win32.DispatchMessage(ref msg);
                        }
                    }
                    else
                    {
                        // No WindowMessage is pending -> do a real gameloop iteration
                        this.GameLoopDoWork();
                    }
                }
            }
            catch (Exception)
            {
                this.gameLauncher.Stop();
            }
        }

        private Graphics graphics;
        private GdiRenderContext renderContext;

        /// <summary>
        /// Does the actual game loop logic.
        /// </summary>
        private void GameLoopDoWork()
        {
            // We must wait until the currentGameLoopIntervalMs is reached to ensure a constant gameloop interval.
            // If not reached wait a while and try again.
            double elapsedSinceLastLoopMs = this.gameLoopStopwatch.Elapsed.TotalMilliseconds;
            if (elapsedSinceLastLoopMs < this.gameLoopSleepThresholdMs)
            {
                // If the expected interval is far away, we can use Thread.Sleep to wait a little bit.
                Thread.Sleep(GdiGameLoop.ThreadSleepMs);
                return;
            }
            if (elapsedSinceLastLoopMs < this.currentGameLoopIntervalMs)
            {
                // We cannot use Thread.Sleep in the last milliseconds of the interval,
                // because Thread.Sleep is not precise and it is very likely to oversleep.
                Thread.SpinWait(50);
                return;
            }

            // Expected interval is reached, lets go!
            this.gameLoopStopwatch.Restart();

            // Render
            this.scene.RenderOneFrame(this.renderContext);
            this.pictureBox.Refresh();
        }
    }
}
