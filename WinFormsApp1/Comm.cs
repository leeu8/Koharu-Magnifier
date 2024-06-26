using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WinFormsApp1
{
    public class Comm
    {
        public Point mousePos;
        public bool isMouseDown;

        public static Bitmap ZoomOut(PictureBox pictureBox, Bitmap image)
        {
            int width = pictureBox.Width;
            int height = pictureBox.Height;
            float ratio = (float)width / (float)image.Width;
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            Bitmap bitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
            graphic.Dispose();
            return bitmap;
        }

        public static Region BmpRgn(Bitmap Picture, Color TransparentColor)
        {
            int nWidth = Picture.Width;
            int nHeight = Picture.Height;
            Region rgn = new Region();
            rgn.MakeEmpty();
            bool isTransRgn;//前一个点是否在透明区
            Color curColor;//当前点的颜色
            Rectangle curRect = new Rectangle();
            curRect.Height = 1;
            int x = 0, y = 0;
            //逐像素扫描这个图片，找出非透明色部分区域并合并起来。
            for (y = 0; y < nHeight; ++y)
            {
                isTransRgn = true;
                for (x = 0; x < nWidth; ++x)
                {
                    curColor = Picture.GetPixel(x, y);
                    if (curColor == TransparentColor || x == nWidth - 1)//如果遇到透明色或行尾
                    {
                        if (isTransRgn == false)//退出有效区
                        {
                            curRect.Width = x - curRect.X;
                            rgn.Union(curRect);
                        }
                    }
                    else//非透明色
                    {
                        if (isTransRgn == true)//进入有效区
                        {
                            curRect.X = x;
                            curRect.Y = y;
                        }
                    }//if curColor
                    isTransRgn = curColor == TransparentColor;
                }//for x
            }//for y
            return rgn;
        }

        public static void SetBitmap(Bitmap bitmap, byte opacity, Control control)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("位图必须是32位包含alpha 通道");

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));   // 创建GDI位图句柄，效率较低
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point pointSource = new Win32.Point(0, 0);
                Win32.Point topPos = new Win32.Point(control.Left, control.Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(control.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);

                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }
    }
}
