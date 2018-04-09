using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ScreenShotDemo;
using LockBitsTest;

namespace ThermalColorAnalyser
{
    public partial class frmPrincipal : Form
    {
        #region Dll Imports

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();
        ScreenCapture sc = new ScreenCapture();

        #endregion

        #region CaptureWindow Variables

        IntPtr activeWindowHandle, oldWindowHandle;
        Image image;
        Bitmap bitmap;
        LockBitmap lockBitmap;

        #endregion

        #region CalculateHUE Method Variables

        Color currentColor;
        double totalHUE = 0f;
        double mediaHUE = 0f;
        int totalPixels = 1;
        
        #endregion

        #region Form Load and Initialize Methods

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            //Nothing to do here...
        }

        #endregion

        #region Main Methods

        private void CaptureWindow()
        {
            int top    = (int)numTop.Value;
            int left   = (int)numLeft.Value;
            int right  = (int)numRight.Value;
            int bottom = (int)numBottom.Value;

            activeWindowHandle = GetForegroundWindow();

            image = sc.CaptureWindow(activeWindowHandle, top, left, right, bottom);

            pbPreview.Image = image;
            pbPreview.SizeMode = PictureBoxSizeMode.StretchImage;

            bitmap = new Bitmap(image);
            lockBitmap = new LockBitmap(bitmap);
        }

        private void CalculateHUE(LockBitmap lb)
        {
            totalHUE = 0f;

            lb.LockBits();

            totalPixels = (lb.Width * lb.Height);

            for (int y = 0; y < lb.Height; y++)
            {
                for (int x = 0; x < lb.Width; x++)
                {
                    currentColor = lb.GetPixel(x, y);
                    totalHUE += currentColor.GetHue();
                }
            }

            mediaHUE = totalHUE / totalPixels;

            lb.UnlockBits();

            tckTemperature.Value = Math.Abs((int)mediaHUE - 180);
        }

        #endregion

        #region Main Events

        private void btnCapture_Click(object sender, EventArgs e)
        {
            oldWindowHandle = GetForegroundWindow();
            
            btnCapture.Text = (!MainLoop.Enabled) ? "Parar" : "Iniciar";

            MainLoop.Enabled = !MainLoop.Enabled;
        }

        private void MainLoop_Tick(object sender, EventArgs e)
        {
            
            CaptureWindow();
            CalculateHUE(lockBitmap);
        }

        #endregion

    }
}
