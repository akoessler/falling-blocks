using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Windows.Base;

namespace FallingBlocks.Engine.Windows.Gdi
{
    public class GameLauncherGdi : AGameLauncherWindows
    {
        private AGame game;
        private ASceneGraph scene;
        private GameForm window;
        private GamePictureBox pictureBox;
        private Bitmap windowBitmapBuffer;
        private GdiGameLoop gameLoop;

        private static readonly Size WindowSize = new Size(1024, 768);

        public bool IsRunning { get; set; }

        protected override void StartInternal(AGame game)
        {
            this.IsRunning = true;

            this.game = game;
            this.scene = this.game.GetScene();

            this.window = new GameForm();
            this.window.KeyPreview = true;
            this.window.FormBorderStyle = FormBorderStyle.Sizable;
            this.window.Text = this.game.GetName();
            this.window.Icon = this.game.GetIcon();
            this.window.Owner = Application.OpenForms.OfType<Form>().FirstOrDefault();
            this.window.SetSize(game.GetWindowSize());

            this.windowBitmapBuffer = new Bitmap(GameLauncherGdi.WindowSize.Width, GameLauncherGdi.WindowSize.Height);
            using (var graphics = Graphics.FromImage(this.windowBitmapBuffer))
            {
                graphics.Clear(Color.White);
            }

            this.pictureBox = new GamePictureBox();
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.Image = this.windowBitmapBuffer;
            this.pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            this.window.Controls.Add(this.pictureBox);

            this.scene.SetKeyEventManager(this);
            this.gameLoop = new GdiGameLoop(this.window, this.pictureBox, this, this.scene, this.windowBitmapBuffer);

            this.window.ShowDialog();
        }

        public override bool HasFocus()
        {
            return this.window.Focused;
        }

        public override void Stop()
        {
            this.IsRunning = false;
            base.Stop();
        }
    }
}