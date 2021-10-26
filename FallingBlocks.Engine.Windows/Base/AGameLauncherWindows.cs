using System.Windows.Forms;
using FallingBlocks.Engine.Core.Audio;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Core.Events;
using FallingBlocks.Engine.Windows.Audio;

namespace FallingBlocks.Engine.Windows.Base
{
    public abstract class AGameLauncherWindows : IGameLauncher, IKeyEventManager
    {
        public void Start(AGame sceneGraph)
        {
            MediaPlayer.Factory = new MediaPlayerFactoryWindows();

            StartInternal(sceneGraph);
        }

        public virtual void Stop()
        {
            MediaPlayer.Shutdown();
            Application.Exit();
        }

        protected abstract void StartInternal(AGame sceneGraph);

        public abstract bool HasFocus();

        public bool IsPressed(EventType eventType)
        {
            var key = this.GetKey(eventType);
            if (key != Keys.None)
            {
                var keyState = Win32.GetAsyncKeyState(key);
                if ((keyState & 0x8000) == 0x8000) // only return true if the MSB is set! (return value is of type short, so check for 0x8000)
                {
                    return true;
                }
            }

            return false;
        }

        private Keys GetKey(EventType key)
        {
            switch (key)
            {
                case EventType.Left: return Keys.Left;
                case EventType.Right: return Keys.Right;
                case EventType.Up: return Keys.Up;
                case EventType.Down: return Keys.Down;
                case EventType.Fire: return Keys.Space;
                case EventType.Escape: return Keys.Escape;
                default:
                    if (key >= EventType.A)
                    {
                        return (Keys)(int)key;
                    }

                    return Keys.None;
            }
        }
    }
}
