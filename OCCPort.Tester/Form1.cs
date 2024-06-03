using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCCPort.Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            V3d_View view = new V3d_View();
            view.StartRotation(0, 0);
            view.Rotation(0, 0);
            view.Pan(0, 0,1,false);

        }
    }
}
