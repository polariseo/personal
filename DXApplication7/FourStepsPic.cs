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
    public partial class FourStepsPic : Form
    {
        public FourStepsPic()
        {
            InitializeComponent();
        }
        
        private void FourStepsPic_Load(object sender, EventArgs e)
        {
            Step1.Image = 实验软件.images[0];
            Step2.Image = 实验软件.images[1];
            Step3.Image = 实验软件.images[2];
            Step4.Image = 实验软件.images[3];
            Step5.Image = 实验软件.images[4];
        }
     
        private void Step1_Paint(object sender, PaintEventArgs e)
        {
            int width = Step1.Width;
            int height = Step1.Height;
            int w = width / 10;
            int h = height / 10;
            DrawGrid(width, height, w, h, e);
        }

        private void Step2_Paint(object sender, PaintEventArgs e)
        {
            int width = Step1.Width;
            int height = Step1.Height;
            int w = width / 10;
            int h = height / 10;
            DrawGrid(width, height, w, h, e);
        }

        private void Step3_Paint(object sender, PaintEventArgs e)
        {
            int width = Step1.Width;
            int height = Step1.Height;
            int w = width / 10;
            int h = height / 10;
            DrawGrid(width, height, w, h, e);
        }

        private void Step4_Paint(object sender, PaintEventArgs e)
        {
            int width = Step1.Width;
            int height = Step1.Height;
            int w = width / 10;
            int h = height / 10;
            DrawGrid(width, height, w, h, e);
        }

        private void Step5_Paint(object sender, PaintEventArgs e)
        {
            int width = Step1.Width;
            int height = Step1.Height;
            int w = width / 10;
            int h = height / 10;
            DrawGrid(width, height, w, h, e);
        }
        //
        private void DrawGrid(int width, int height, int w, int h, PaintEventArgs e)
        {
            Point p1 = new Point();
            Point p2 = new Point();

            p1.X = 0; p2.X = width;
            for (int y = 0; y <= height; y = y + h)
            {
                p1.Y = y; p2.Y = y;
                DrawLine(p1, p2, e);
            }
            p1.Y = 0; p2.Y = height;
            for (int x = 0; x <= width; x = x + w)
            {
                p1.X = x; p2.X = x;
                DrawLine(p1, p2, e);
            }
        }
        Pen bluePen = new Pen(Color.White);
        private void DrawLine(Point p1, Point p2, PaintEventArgs e)
        {
            e.Graphics.DrawLine(bluePen, p1, p2);
        }
    }
}
