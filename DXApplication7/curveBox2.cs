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
    public partial class curveBox2 : Form
    {
        public curveBox2()
        {
            InitializeComponent();
            Drawcurve(this.CurveBox.Handle, 实验软件.UnwrapRes1, 实验软件.newwidth1, 实验软件.newheight1,
       (int)实验软件.unwrap1scalev * (实验软件.up1points[0].X - 实验软件.unwrap1wgap / 2),
       (int)实验软件.unwrap1scalev * (实验软件.up1points[0].Y - 实验软件.unwrap1hgap / 2),
       (int)实验软件.unwrap1scalev * (实验软件.up1points[1].X - 实验软件.unwrap1wgap / 2),
       (int)实验软件.unwrap1scalev * (实验软件.up1points[1].Y - 实验软件.unwrap1hgap / 2), 实验软件.steps);
        }
        [DllImport("3D_MATDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drawcurve(IntPtr handle, IntPtr image, int width, int height, int x1, int y1, int x2, int y2, int steps);
        private void CurveBox_Paint(object sender, PaintEventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.UnwrapRes1, 实验软件.newwidth1, 实验软件.newheight1,
         (int)实验软件.unwrap1scalev * (实验软件.up1points[0].X - 实验软件.unwrap1wgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[0].Y - 实验软件.unwrap1hgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[1].X - 实验软件.unwrap1wgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[1].Y - 实验软件.unwrap1hgap / 2), 实验软件.steps);
        }

        private void curveBox2_Load(object sender, EventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.UnwrapRes1, 实验软件.newwidth1, 实验软件.newheight1,
         (int)实验软件.unwrap1scalev * (实验软件.up1points[0].X - 实验软件.unwrap1wgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[0].Y - 实验软件.unwrap1hgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[1].X - 实验软件.unwrap1wgap / 2),
         (int)实验软件.unwrap1scalev * (实验软件.up1points[1].Y - 实验软件.unwrap1hgap / 2), 实验软件.steps);
        }

        private void curveBox2_Paint(object sender, PaintEventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.UnwrapRes1, 实验软件.newwidth1, 实验软件.newheight1,
           (int)实验软件.unwrap1scalev * (实验软件.up1points[0].X - 实验软件.unwrap1wgap / 2),
           (int)实验软件.unwrap1scalev * (实验软件.up1points[0].Y - 实验软件.unwrap1hgap / 2),
           (int)实验软件.unwrap1scalev * (实验软件.up1points[1].X - 实验软件.unwrap1wgap / 2),
           (int)实验软件.unwrap1scalev * (实验软件.up1points[1].Y - 实验软件.unwrap1hgap / 2), 实验软件.steps);
        }

        private void curveBox2_Shown(object sender, EventArgs e)
        {
            Drawcurve(this.CurveBox.Handle, 实验软件.UnwrapRes1, 实验软件.newwidth1, 实验软件.newheight1,
          (int)实验软件.unwrap1scalev * (实验软件.up1points[0].X - 实验软件.unwrap1wgap / 2),
          (int)实验软件.unwrap1scalev * (实验软件.up1points[0].Y - 实验软件.unwrap1hgap / 2),
          (int)实验软件.unwrap1scalev * (实验软件.up1points[1].X - 实验软件.unwrap1wgap / 2),
          (int)实验软件.unwrap1scalev * (实验软件.up1points[1].Y - 实验软件.unwrap1hgap / 2), 实验软件.steps);
        }
    }
}
