using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace DXApplication7
{
    public partial class CurvePic : Form
    {
        public CurvePic()
        {
            InitializeComponent();
            Drawcurve(this.CurveBox.Handle, 实验软件.rUnwrap, 实验软件.wwidth, 实验软件.wheight, (int)实验软件.scalev * (实验软件.points[0].X - 实验软件.picwgap / 2),
                (int)实验软件.scalev * (实验软件.points[0].Y - 实验软件.pichgap / 2),
                  (int)实验软件.scalev * (实验软件.points[1].X - 实验软件.picwgap / 2), (int)实验软件.scalev * (实验软件.points[1].Y - 实验软件.pichgap / 2), 实验软件.wsteps);
            
        }
        [DllImport("3D_MATDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drawcurve(IntPtr handle, IntPtr image, int width, int height, int x1, int y1, int x2, int y2, int steps);
        private void CurveBox_Paint(object sender, PaintEventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.rUnwrap, 实验软件.wwidth, 实验软件.wheight, (int)实验软件.scalev * (实验软件.points[0].X - 实验软件.picwgap / 2),
          (int)实验软件.scalev * (实验软件.points[0].Y - 实验软件.pichgap / 2),
            (int)实验软件.scalev * (实验软件.points[1].X - 实验软件.picwgap / 2), (int)实验软件.scalev * (实验软件.points[1].Y - 实验软件.pichgap / 2), 实验软件.wsteps);
        }

        private void CurvePic_Load(object sender, EventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.rUnwrap, 实验软件.wwidth, 实验软件.wheight, (int)实验软件.scalev * (实验软件.points[0].X - 实验软件.picwgap / 2),
         (int)实验软件.scalev * (实验软件.points[0].Y - 实验软件.pichgap / 2),
           (int)实验软件.scalev * (实验软件.points[1].X - 实验软件.picwgap / 2), (int)实验软件.scalev * (实验软件.points[1].Y - 实验软件.pichgap / 2), 实验软件.wsteps);
        }

        private void CurvePic_Shown(object sender, EventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.rUnwrap, 实验软件.wwidth, 实验软件.wheight, (int)实验软件.scalev * (实验软件.points[0].X - 实验软件.picwgap / 2),
          (int)实验软件.scalev * (实验软件.points[0].Y - 实验软件.pichgap / 2),
            (int)实验软件.scalev * (实验软件.points[1].X - 实验软件.picwgap / 2), (int)实验软件.scalev * (实验软件.points[1].Y - 实验软件.pichgap / 2), 实验软件.wsteps);
        }
    }
}
