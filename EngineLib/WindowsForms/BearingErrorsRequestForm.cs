using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{   

    public partial class BearingErrorsRequestForm : Form
    {
        MainForm parent;
        public static bool ScanningDirection = true;        
        public static double AlthaFinish = 0;
        public static double AlthaStart = 10;        
        public static double Step = 0.1;        
        
        public BearingErrorsRequestForm(MainForm Parent)
        {
            InitializeComponent();
            parent = Parent;
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BearingErrorsRequestForm.ScanningDirection = radioButton1.Checked;
            BearingErrorsRequestForm.AlthaFinish = Convert.ToDouble(textBoxAlthaFinish.Text);
            BearingErrorsRequestForm.AlthaStart = Convert.ToDouble(textBoxAlthaStart.Text);
            BearingErrorsRequestForm.Step = Convert.ToDouble(textBoxStep.Text);            
            

            bool match = false;
            foreach (TreeNode node in parent.treeViewConfiguration.Nodes[3].Nodes)
            {   
                if (node.Name == "BearingErrors")
                {
                    match = true;   
                }
            }
            if (!match)
            {
                TreeNode newNode = new TreeNode("Ошибки пеленга");
                newNode.Name = "BearingErrors";
                parent.treeViewConfiguration.Nodes[3].Nodes.Add(newNode);
            }
            string workPlane = parent.apperture.workPlane;

            parent.renderControl1.DrawPelleng(CreateAppertureForm.scanThetaStart, CreateAppertureForm.scanThetaFinish, CreateAppertureForm.scanPhiStart, CreateAppertureForm.scanDelta, CreateAppertureForm.SysOfCoord, BearingErrorsRequestForm.AlthaStart, workPlane, CreateAppertureForm.Axis, true);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        

        private void BearingErrorsRequestForm_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = BearingErrorsRequestForm.ScanningDirection;
            textBoxAlthaFinish.Text = Convert.ToString(BearingErrorsRequestForm.AlthaFinish);
            textBoxAlthaStart.Text = Convert.ToString(BearingErrorsRequestForm.AlthaStart);
            textBoxStep.Text = Convert.ToString(BearingErrorsRequestForm.Step);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBoxAlthaFinish.Enabled = false;                
            }
            else
            {
                textBoxAlthaFinish.Enabled = true;
            }
        }
    }
}

