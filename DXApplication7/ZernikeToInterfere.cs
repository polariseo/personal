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
    public partial class ZernikeToInterfere : Form
    {
        public ZernikeToInterfere()
        {
            InitializeComponent();
        }

        private void ZernikeToInterfere_Load(object sender, EventArgs e)
        {
            string filepath = "F:\\zernike像差干涉图\\" + 实验软件.zeridx.ToString() + ".jpg";
            Bitmap zerpic = new Bitmap(filepath);
            Interfere.Image = zerpic;
        }
    }
}
