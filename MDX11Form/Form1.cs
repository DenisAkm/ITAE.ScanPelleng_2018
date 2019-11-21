using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apparat;
using Apparat.Renderables;
using System.IO;
using System.Collections;
using Ewk.Math.Numerics;
using ZedGraph;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace MDX11Form
{
    public partial class Form1 : Form
    {       
        public Form1()
        {
            InitializeComponent();            
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderControl1.shutDown();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            renderControl1.init();
        }


        private void ChangeCameraView(object sender, EventArgs e)
        {
            var obj = (sender as RadioButton);
            if (obj.Name == "radioButtonFreeRotate" && obj.Checked)
            {
                renderControl1.ChangeCamera(0);
            }
            else if (obj.Name == "radioButtonEgoX" && obj.Checked)
            {
                renderControl1.ChangeCamera(1);
            }
            else if (obj.Name == "radioButtonEgoY" && obj.Checked)
            {
                renderControl1.ChangeCamera(2);
            }
            else if (obj.Name == "radioButtonEgoZ" && obj.Checked)
            {
                renderControl1.ChangeCamera(3);
            }
        }   
    }
}
