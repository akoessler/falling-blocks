using System.Drawing;
using System.Windows.Forms;

namespace FallingBlocks.Engine.Windows.Gdi
{
    /// <summary>
    /// Form which is used to draw the game bitmap
    /// </summary>
    public partial class GameForm : Form
    {
        public GameForm()
        {
            this.InitializeComponent();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        public void SetSize(Size screenSize)
        {
            // Ensure that the ClientRectangle has the expected size, not the form (which includes the border and title bar).
            this.Size = screenSize;
            Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
            int missingWidth = screenSize.Width - screenRectangle.Width;
            int missingHeight = screenSize.Height - screenRectangle.Height;
            this.Size = new Size
            (
                screenSize.Width + missingWidth,
                screenSize.Height + missingHeight
            );
        }
    }

    /// <summary>
    /// PictureBox to render the game bitmap
    /// </summary>
    public class GamePictureBox : PictureBox
    {
        public GamePictureBox()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
                true);
        }
    }
}