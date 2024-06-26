using Karna.Magnification;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        Form form1 = new Form();
        private Bitmap bitmap;
        private Magnifier magnifier;

        public Form2(Form form)
        {
            InitializeComponent();
            form1 = form;
            FormBorderStyle = form1.FormBorderStyle;
            TopMost = form1.TopMost;
            StartPosition = FormStartPosition.Manual;
            Size = form1.Size;
            Location = form1.Location;
            ShowInTaskbar = false;
            bitmap = Comm.ZoomOut(pictureBox1, new Bitmap(@"pic3.png"));
            Region = Comm.BmpRgn(bitmap, Color.FromArgb(0, 0, 0, 0));
            CoverMagnifier();
            magnifier = new Magnifier(this);
        }

        public void UpdateMagnification(float m)
        {
            float f = magnifier.Magnification + m;
            if (f <= 2.0f)
            {
                magnifier.Magnification = 2.0f;
            }
            else
            {
                magnifier.Magnification = f;
            }

        }

        public void UncoverMagnifier()
        {
            Comm.SetBitmap(bitmap, 255, this);
        }

        public void CoverMagnifier()
        {
            Comm.SetBitmap(bitmap, 0, this);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;  //  WS_EX_LAYERED 扩展样式
                return cp;
            }
        }
    }
}
