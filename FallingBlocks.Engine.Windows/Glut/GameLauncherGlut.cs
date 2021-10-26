using System;
using System.Drawing;
using System.Windows.Forms;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Render.Ogl;
using FallingBlocks.Engine.Windows.Base;
using TaoGlut = Tao.FreeGlut.Glut;

namespace FallingBlocks.Engine.Windows.Glut
{
    public class GameLauncherGlut : AGameLauncherWindows
    {
        private ASceneGraph scene;
        private OglRenderContext renderContext;
        private IntPtr windowHandle;
        private Size windowSize;
        private string windowTitle;

        protected override void StartInternal(AGame game)
        {
            this.scene = game.GetScene();
            this.windowTitle = game.GetName();
            this.windowSize = game.GetWindowSize();

            TaoGlut.glutInit(); // Initialize glut
            TaoGlut.glutInitDisplayMode(TaoGlut.GLUT_DOUBLE | TaoGlut.GLUT_RGBA); // Setup display mode to double buffer and RGB color
            TaoGlut.glutInitWindowSize(this.windowSize.Width, this.windowSize.Height); // Set the screen size
            TaoGlut.glutInitWindowPosition((TaoGlut.glutGet(TaoGlut.GLUT_SCREEN_WIDTH) - this.windowSize.Width) / 2,
                (TaoGlut.glutGet(TaoGlut.GLUT_SCREEN_HEIGHT) - this.windowSize.Height) / 2);

            TaoGlut.glutCreateWindow(this.windowTitle);
            this.windowHandle = Win32.FindWindow(null, this.windowTitle);

            Icon icon = game.GetIcon();
            Win32.SendMessage(this.windowHandle, (int) Win32.WindowsMessage.WM_SETICON, (int) Win32.SetIconMode.ICON_SMALL, icon.Handle);
            Win32.SendMessage(this.windowHandle, (int) Win32.WindowsMessage.WM_SETICON, (int) Win32.SetIconMode.ICON_BIG, icon.Handle);

            this.renderContext = new OglRenderContext(new GlutOpenGl());

            //TaoGlut.glutReshapeFunc(reshape);
            this.scene.SetKeyEventManager(this);
            this.scene.Init(this.renderContext);

            TaoGlut.glutDisplayFunc(() =>
            {
                this.scene.RenderOneFrame(this.renderContext);
                TaoGlut.glutPostRedisplay(); // Redraw the scene
                TaoGlut.glutSwapBuffers();
            });

            TaoGlut.glutMainLoop();
            TaoGlut.glutWMCloseFunc(this.window_closed);
        }

        public override bool HasFocus()
        {
            return this.windowHandle == Win32.GetFocus();
        }

        private void window_closed()
        {
            this.Stop();
        }
    }
}