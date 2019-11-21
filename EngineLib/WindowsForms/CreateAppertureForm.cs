using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    public partial class CreateAppertureForm : Form
    {
        public static DVector I = new DVector(1, 0, 0); //-0.1598934596, 0, 0.3663091297
        public static DVector M = new DVector(0, 1, 0); //0, 0.4912473491, 0
        public static double Magnitude = Constant.Z_0;
        public static double Phase = 0;
        public static bool Difference = true;
        public static string Axis = "Плоскость XZ";
        public static string LoadAdress = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AntFlow\wireTest\wireTestApp.nas");//@"C:\Users\Denis\Documents\Documents 2018\Feko Models\AntFlow\wireTest\wireTestApp.nas";
        public static string DistributionType = "Косинус на пьедестале";//"Постоянное поле";
        public static int CreationType = 2;
        public static bool Scanning = true;
        public static int SysOfCoord = 0;
        

        public static double scanPhiStart = 0;
        public static double scanPhiFinish = 0;
        public static double scanThetaStart = 0;
        public static double scanThetaFinish = 0;
        public static double scanDelta = 15;

        MainForm parent;
        public CreateAppertureForm(MainForm ParentForm)
        {
            InitializeComponent();
            parent = ParentForm;
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateAppertureForm.I = new DVector(Convert.ToDouble(textBoxAppertureIx.Text.Replace(".", ",")), Convert.ToDouble(textBoxAppertureIy.Text.Replace(".", ",")), Convert.ToDouble(textBoxAppertureIz.Text.Replace(".", ",")));
            CreateAppertureForm.M = new DVector(Convert.ToDouble(textBoxAppertureMx.Text.Replace(".", ",")), Convert.ToDouble(textBoxAppertureMy.Text.Replace(".", ",")), Convert.ToDouble(textBoxAppertureMz.Text.Replace(".", ",")));

            CreateAppertureForm.Magnitude = Convert.ToDouble(textBoxAppertureAmplitude.Text);
            CreateAppertureForm.Phase = Convert.ToDouble(textBoxApperturePhase.Text);
            CreateAppertureForm.Difference = radioButtonChannel2.Checked;
            CreateAppertureForm.LoadAdress = textBoxAppertureFileName.Text;
            CreateAppertureForm.CreationType = comboBoxCreationType.SelectedIndex;
            CreateAppertureForm.Scanning = checkBoxScanning.Checked;

            CreateAppertureForm.scanPhiStart = Convert.ToDouble(textBoxPhiStart.Text);
            CreateAppertureForm.scanPhiFinish = Convert.ToDouble(textBoxPhiFinish.Text);
            CreateAppertureForm.scanThetaStart = Convert.ToDouble(textBoxThetaStart.Text);
            CreateAppertureForm.scanThetaFinish = Convert.ToDouble(textBoxThetaFinish.Text);
            CreateAppertureForm.scanDelta = Convert.ToDouble(textBoxStep.Text);
            CreateAppertureForm.SysOfCoord = comboBoxSysOfCoord.SelectedIndex;

            parent.apperture = FekoAppertureLoad(textBoxAppertureFileName.Text);
            parent.renderControl1.Add(parent.apperture);

            if (radioButtonChannel2.Checked)
            {
                CreateAppertureForm.Axis = comboBoxAxis.Items[comboBoxAxis.SelectedIndex].ToString();    
            }            
            CreateAppertureForm.DistributionType = comboBoxDistribution.Items[comboBoxDistribution.SelectedIndex].ToString();

            
            if (comboBoxDistribution.SelectedIndex == 0)
            {
                parent.apperture.GenerateConstantCurrentApperture(I, M, Magnitude, Phase);
            }
            else if (comboBoxDistribution.SelectedIndex == 1)
            {
                parent.apperture.GenerateCosOnStepApperture(I, M, 0.5, Magnitude, Phase);
            }

            if (radioButtonChannel2.Checked)
            {
                parent.apperture.DifferenceRadiationPattern(comboBoxAxis.SelectedIndex + 1);
            }

            

            for (int k = 0; k < parent.treeViewConfiguration.Nodes.Count; k++)
            {
                if (parent.treeViewConfiguration.Nodes[k].Name == "Source")
                {
                    TreeNode newNode = new TreeNode(textBoxAppertureFileName.Text);
                    newNode.Name = "CreateApperture";
                    if (parent.treeViewConfiguration.Nodes[k].Nodes.Count == 0)
                    {
                        parent.treeViewConfiguration.Nodes[k].Nodes.Add(newNode);
                    }
                    else
                    {
                        parent.treeViewConfiguration.Nodes[k].Nodes[0].Text = LoadAdress;
                    }
                }
            }
            if (Scanning)
            {
                parent.renderControl1.DrawScanRegion(CreateAppertureForm.scanThetaStart, CreateAppertureForm.scanThetaFinish, CreateAppertureForm.scanPhiStart, CreateAppertureForm.scanPhiFinish, CreateAppertureForm.scanDelta, CreateAppertureForm.SysOfCoord);    
            }
            else
            {
                parent.renderControl1.DrawScanRegion(0, 0, 0, 0, 1, 0);    
            }
            Close();
            
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "NASTRAN mesh Files (*.nas)|*.nas|Project Files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(LoadAdress);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {   
                textBoxAppertureFileName.Text = openFileDialog1.FileName;
            }            
        }
        private Apperture FekoAppertureLoad(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string s = line.Remove(1);

            while (s == "$")
            {
                line = sr.ReadLine();
                if (line.Length > 1)
                {
                    s = line.Remove(1);
                }
            }
            List<double> px = new List<double>();
            List<double> py = new List<double>();
            List<double> pz = new List<double>();

            List<int> i1 = new List<int>();
            List<int> i2 = new List<int>();
            List<int> i3 = new List<int>();

            string gr = line.Remove(4);
            if (line.Length > 4)
            {
                gr = line.Remove(4);
                while (gr == "GRID")
                {
                    string x = line.Substring(40, 16);
                    string y = line.Substring(56, 16);
                    line = sr.ReadLine();
                    string z = line.Substring(8, 16);

                    px.Add(Convert.ToDouble(x.Replace(".", ",")));
                    py.Add(Convert.ToDouble(y.Replace(".", ",")));
                    pz.Add(Convert.ToDouble(z.Replace(".", ",")));
                    line = sr.ReadLine();
                    gr = line.Remove(4);
                }
            }
            if (line.Length > 4)
            {

                while (gr == "CTRI")
                {
                    string ind1 = line.Substring(26, 6);
                    string ind2 = line.Substring(34, 6);
                    string ind3 = line.Substring(42, 6);
                    i1.Add(Convert.ToInt32(ind1));
                    i2.Add(Convert.ToInt32(ind2));
                    i3.Add(Convert.ToInt32(ind3));
                    line = sr.ReadLine();
                    gr = line.Remove(4);
                }
            }
            sr.Close();

            return new Apperture(px, py, pz, i1, i2, i3);
        }

        private void radioButtonChannel2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChannel2.Checked)
            {
                comboBoxAxis.Enabled = true;
            }
            else
            {
                comboBoxAxis.Enabled = false;
            }
        }

        private void CreateAppertureForm_Load(object sender, EventArgs e)
        {

            textBoxAppertureAmplitude.Text = Convert.ToString(CreateAppertureForm.Magnitude);
            textBoxApperturePhase.Text = Convert.ToString(CreateAppertureForm.Phase);
            textBoxAppertureIx.Text = Convert.ToString(CreateAppertureForm.I.X);
            textBoxAppertureIy.Text = Convert.ToString(CreateAppertureForm.I.Y);
            textBoxAppertureIz.Text = Convert.ToString(CreateAppertureForm.I.Z);

            textBoxAppertureMx.Text = Convert.ToString(CreateAppertureForm.M.X);
            textBoxAppertureMy.Text = Convert.ToString(CreateAppertureForm.M.Y);
            textBoxAppertureMz.Text = Convert.ToString(CreateAppertureForm.M.Z);

            textBoxStep.Text = Convert.ToString(scanDelta);
            textBoxPhiStart.Text = Convert.ToString(scanPhiStart);
            textBoxPhiFinish.Text = Convert.ToString(scanPhiFinish);
            textBoxThetaStart.Text = Convert.ToString(scanThetaStart);
            textBoxThetaFinish.Text = Convert.ToString(scanThetaFinish);

            radioButtonChannel2.Checked = CreateAppertureForm.Difference;
            textBoxAppertureFileName.Text = CreateAppertureForm.LoadAdress;
            comboBoxCreationType.SelectedIndex = CreateAppertureForm.CreationType;
            checkBoxScanning.Checked = CreateAppertureForm.Scanning;
            comboBoxSysOfCoord.SelectedIndex = CreateAppertureForm.SysOfCoord;

            for (int i = 0; i < comboBoxDistribution.Items.Count; i++)
            {
                if (comboBoxDistribution.Items[i].ToString() == CreateAppertureForm.DistributionType)
                {
                    comboBoxDistribution.SelectedIndex = i;
                }
            }

            for (int i = 0; i < comboBoxAxis.Items.Count; i++)
            {
                if (comboBoxAxis.Items[i].ToString() == CreateAppertureForm.Axis)
                {
                    comboBoxAxis.SelectedIndex = i;
                }
            }
            
                
          
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxScanning.Checked)
            {
                textBoxThetaStart.Enabled = true;
                textBoxThetaFinish.Enabled = true;
                textBoxPhiStart.Enabled = true;
                textBoxPhiFinish.Enabled = true;
                textBoxStep.Enabled = true;
                this.checkBoxScanning.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                textBoxThetaStart.Enabled = false;
                textBoxThetaFinish.Enabled = false;
                textBoxPhiStart.Enabled = false;
                textBoxPhiFinish.Enabled = false;
                textBoxStep.Enabled = false;
                checkBoxScanning.ForeColor = System.Drawing.SystemColors.ControlDark;
            }
        }

        private void checkBoxScanning_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxScanning.Checked)
            {
                groupBox6.Enabled = true;
                groupBox7.Enabled = true;
            }
            else
            {
                groupBox6.Enabled = false;
                groupBox7.Enabled = false;
            }
        }

     
    }
}
