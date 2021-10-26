using System;
using System.Windows.Forms;
using FallingBlocks.Engine.Windows.Glut;
using FallingBlocks.Game.Scene;

namespace FallingBlocks
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var launcher = new GameLauncherGlut();
            //var launcher = new GameLauncherGdi();

            launcher.Start(new FallingBlocksGame());

            Application.Run();
        }
    }
}
