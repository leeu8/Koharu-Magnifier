namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Point mousePos;
        private bool isMouseDown;
        private Form2 form2;
        private Bitmap bitmap1;
        private Bitmap bitmap2;
        private bool wheelFlag;

        public Form1()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            bitmap1 = Comm.ZoomOut(pictureBox1, new Bitmap(@"pic.png"));
            bitmap2 = Comm.ZoomOut(pictureBox1, new Bitmap(@"pic2.png"));
            Comm.SetBitmap(bitmap1, 255, this);
            Region = Comm.BmpRgn(bitmap1, Color.FromArgb(0, 0, 0, 0));

            pictureBox1.MouseDown += OnMouseDown;
            pictureBox1.MouseUp += OnMouseUp;
            pictureBox1.MouseMove += OnMouseMove;

            ContextMenuStrip ms = new ContextMenuStrip();
            ms.Items.Add("春赛克");
            ms.Items.Add("放大镜");
            ms.Items.Add("退出");
            ms.ItemClicked += new ToolStripItemClickedEventHandler(ItemClicked);
            pictureBox1.ContextMenuStrip = ms;
            pictureBox1.MouseWheel += MouseWheel;
        }

        private void MouseWheel(object sender, MouseEventArgs e)
        {
            if (wheelFlag)
            {
                float m = 0.01f;
                form2.UpdateMagnification((float)e.Delta * m);
            }
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

        private void ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (((ContextMenuStrip)sender).Items[0] == e.ClickedItem)
            {
                form2.CoverMagnifier();
                Comm.SetBitmap(bitmap1, 255, this);
                wheelFlag = false;
            }

            if (((ContextMenuStrip)sender).Items[1] == e.ClickedItem)
            {
                form2.UncoverMagnifier();
                Comm.SetBitmap(bitmap2, 255, this);
                wheelFlag |= true;
            }

            if (((ContextMenuStrip)sender).Items[2] == e.ClickedItem)
            {
                Environment.Exit(0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size size = Screen.PrimaryScreen.WorkingArea.Size;
            Left = (size.Width - Width) / 2;
            Top = (size.Height - Height) / 2;
            WindowState = FormWindowState.Normal;
            TopMost = true;
            form2 = new Form2(this);
            form2.Show();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            mousePos = Cursor.Position;
            isMouseDown = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            Focus();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point tempPos = Cursor.Position;
                Location = new Point(Location.X + (tempPos.X - mousePos.X), Location.Y + (tempPos.Y - mousePos.Y));
                form2.Location = Location;
                mousePos = Cursor.Position;
            }
        }
    }
}
