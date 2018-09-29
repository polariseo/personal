using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication7
{
    public delegate void SendEventHandler(Bitmap bmpContrast);
    public partial class Form2 : Form
    {
        public event SendEventHandler SendEvent;
        public Form2()
        {
            InitializeComponent();
            this.trackBar1.Minimum = 0;
            this.trackBar1.Maximum = 10;
            this.trackBar1.Value = 5;
        }

        public void ShowPicture(Bitmap bmpToForm2, int PictureNum)
        {
            //判断掩膜板
            Bitmap temp_bmp = bmpToForm2;
            if (PictureNum == 0)
            {
                BitmapInfo bmpt = 实验软件.GetImagePixel(temp_bmp);
                int step = bmpt.Step;
                int gstep;
                IntPtr OutputHist = 实验软件.histShow(bmpt.Result, 实验软件.width, 实验软件.height, step, out gstep);
                Bitmap img = new Bitmap(256, 256, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputHist);
                img.Palette = CvToolbox.GrayscalePalette;
                pictureBox1.Image = img;
            }
            else
            {
                BitmapInfo bmpt = 实验软件.GetImagePixel(temp_bmp);
                int step = bmpt.Step;
                int gstep;
                IntPtr OutputHist = 实验软件.histShow_Mask(bmpt.Result, 实验软件.width, 实验软件.height, step, 实验软件.Mask, out gstep);
                Bitmap img = new Bitmap(256, 256, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputHist);
                img.Palette = CvToolbox.GrayscalePalette;
                pictureBox1.Image = img;
            }

        }

        public static bool EventFlag = false;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float tempValue = trackBar1.Value;
            float thresh = tempValue / 5 + 0.1f;

            Bitmap tempContrast = 实验软件.bmpToForm2;
            BitmapInfo bmpt = 实验软件.GetImagePixel(tempContrast);

            int step = bmpt.Step;
            int gstep;

            IntPtr OutputImage = 实验软件.contractProc(bmpt.Result, 实验软件.width, 实验软件.height,
                step, thresh, out gstep);
            Bitmap img = new Bitmap(实验软件.width, 实验软件.height, gstep,
                 System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
            img.Palette = CvToolbox.GrayscalePalette;

            //生成委托
           if (this.SendEvent != null)
            {
                this.SendEvent(img);
            }
            
            //直方图显示
            BitmapInfo bmpt_hist = 实验软件.GetImagePixel(img);
            int step_hist = bmpt_hist.Step;
            int gstep_hist;
            if (实验软件.PictureNum == 1)
            {
                IntPtr OutputHist = 实验软件.histShow_Mask(bmpt_hist.Result, 实验软件.width, 实验软件.height,
                    step, 实验软件.Mask, out gstep_hist);
                Bitmap imgHist = new Bitmap(256, 256, gstep_hist,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputHist);
                imgHist.Palette = CvToolbox.GrayscalePalette;
                pictureBox1.Image = imgHist;
            }
            else
            {
                IntPtr OutputHist = 实验软件.histShow(bmpt_hist.Result, 实验软件.width, 实验软件.height,
                    step, out gstep_hist);
                Bitmap imgHist = new Bitmap(256, 256, gstep_hist,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputHist);
                imgHist.Palette = CvToolbox.GrayscalePalette;
                pictureBox1.Image = imgHist;
            }
        }
    }
}
