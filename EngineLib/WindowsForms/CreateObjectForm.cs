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
    public partial class CreateObjectForm : Form
    {
        MainForm parent;        
        public static string LoadObjectAdress = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AntFlow\test\KirchhoffLows\kross.nas");
        public static string LoadCurrentsAdress = Path.Combine(Environment.CurrentDirectory, "currentsOut.txt");
        public static bool UseCurrents = false;
        public static double WireRadius = 0.5;
        public static double Dim = 0.001;   //мм
        public CreateObjectForm(MainForm Parent)
        {
            parent = Parent;
            InitializeComponent();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (MatchTitle(textBoxObjectAdress.Text))
            {
                textBoxObjectAdress.BackColor = Color.Red;
            }
            else
            {
                CreateObjectForm.LoadObjectAdress = textBoxObjectAdress.Text;
                CreateObjectForm.LoadCurrentsAdress = textBoxUseCurrents.Text;
                CreateObjectForm.UseCurrents = checkBoxUseCurrents.Checked;
                CreateObjectForm.WireRadius = Convert.ToDouble(textBoxWireRadius.Text);

                parent.useCurrents = CreateObjectForm.UseCurrents;
                parent.loadCurrentsAdress = CreateObjectForm.LoadCurrentsAdress;
                parent.wire = CreateObjectForm.WireRadius * CreateObjectForm.Dim;

                WireMesh wiregeo = FekoWireLoad(textBoxObjectAdress.Text);
                if (wiregeo.BaseFunctionsCount != 0)
                {
                    parent.WireGeometry.Add(wiregeo);

                    parent.renderControl1.Add(wiregeo);

                    parent.AddObjectTreeView(textBoxObjectAdress.Text);

                    Close();
                }
            }
        }

        private bool MatchTitle(string title)
        {
            bool match = false;
            foreach (TreeNode node in parent.treeViewConfiguration.Nodes[2].Nodes)
            {
                if (node.Name == title)
                {
                    match = true;
                }
            }
            return match;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseCurrents.Checked)
            {
                textBoxUseCurrents.Enabled = true;
                buttonBrowseCurrents.Enabled = true;
            }
            else
            {
                textBoxUseCurrents.Enabled = false;
                buttonBrowseCurrents.Enabled = false;
            }
        }


        private WireMesh FekoWireLoad(string fileName)
        {
            if (File.Exists(fileName))
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

                    while (gr == "CBAR")
                    {
                        string ind1 = line.Substring(24, 8);
                        string ind2 = line.Substring(32, 8);
                        i1.Add(Convert.ToInt32(ind1));
                        i2.Add(Convert.ToInt32(ind2));
                        line = sr.ReadLine();
                        gr = line.Remove(4);
                    }
                }
                sr.Close();

                return new WireMesh(px, py, pz, i1, i2, fileName);
            }
            else
            {
                MessageBox.Show("Выбранного файла не существует");
                return new WireMesh(new List<double>(), new List<double>(), new List<double>(), new List<int>(), new List<int>(), fileName = "Empty");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "NASTRAN mesh Files (*.nas)|*.nas|Project Files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(LoadObjectAdress);//@"C:/Users/Denis/Documents/Документы 2018/Расчет рассеянного поля по ИУ/Feko"
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {                 
                textBoxObjectAdress.Text = openFileDialog1.FileName;
            }
        }

        private void CreateObjectForm_Load(object sender, EventArgs e)
        {
            checkBoxUseCurrents.Checked = CreateObjectForm.UseCurrents;
            textBoxObjectAdress.Text = CreateObjectForm.LoadObjectAdress;
            textBoxUseCurrents.Text = CreateObjectForm.LoadCurrentsAdress;
            textBoxWireRadius.Text = Convert.ToString(CreateObjectForm.WireRadius);            
        }
    }
}
