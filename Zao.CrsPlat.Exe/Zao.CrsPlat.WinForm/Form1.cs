using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zao.CrsPlat.WinForm
{
    public partial class Form1 : Form
    {
        SplashScreenManager manager;

        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            manager = new SplashScreenManager(this, typeof(global::Zao.CrsPlat.UserControl.StartingScreen), true, true);
            manager.Properties.ClosingDelay = 5000;
        }
    }
}
 