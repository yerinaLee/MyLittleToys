using System.Drawing;
using System.Windows.Forms;

namespace MyLittleToys
{
    public partial class OverlayForm : Form
    {
        private Point startPoint;
        private Rectangle selectionRect;
        private bool isDragging = false;
        private Bitmap screenSnapshot;

        public Bitmap CapturedImage { get; private set; }

        public OverlayForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Cursor = Cursors.Cross;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;

            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            this.Bounds = virtualScreen;

            screenSnapshot = new Bitmap(virtualScreen.Width, virtualScreen.Height);
            using (Graphics g = Graphics.FromImage(screenSnapshot))
            {
                g.CopyFromScreen(virtualScreen.Location, Point.Empty, virtualScreen.Size);
            }
            this.BackgroundImage = screenSnapshot;

            this.KeyDown += OverlayForm_KeyDown;
        }

        private void OverlayForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                startPoint = e.Location;
                selectionRect = new Rectangle(e.Location, new Size(0, 0));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                selectionRect = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(startPoint.X - e.X),
                    Math.Abs(startPoint.Y - e.Y));

                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                if (selectionRect.Width > 0 && selectionRect.Height > 0)
                {
                    CapturedImage = screenSnapshot.Clone(selectionRect, screenSnapshot.PixelFormat);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                this.Close();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, Color.Black)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            if (selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                e.Graphics.DrawImage(screenSnapshot, selectionRect, selectionRect, GraphicsUnit.Pixel);
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {

        }
    }
}