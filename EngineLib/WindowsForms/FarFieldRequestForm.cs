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
    public partial class FarFieldRequestForm : Form
    {
        public static double ThetaStart = 88;        
        public static double ThetaFinish = 92;
        public static double PhiStart = 0;
        public static double PhiFinish = 0;
        public static double Delta = 0.001;
        public static int SystemOfCoordinates = 1;
        public static string initialTitle = "Поле дальней зоны ";
        //public static int FarFieldCount = 1;
        public bool Reducting = false;

        FarFieldRequestTemplate parentTemplate;        
        MainForm parent;

        public FarFieldRequestForm(MainForm Parent)
        {
            InitializeComponent();
            parent = Parent;
            LoadDefaultValues();
            Show();
        }
        public FarFieldRequestForm(MainForm Parent, FarFieldRequestTemplate template)
        {
            InitializeComponent();
            parent = Parent;
            ApplyFormValues(template);
            RefreshFormValues();
            Show();
            parentTemplate = template;
            Reducting = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            RefreshFormValues();
            if (Reducting)
            {
                string formerName = parentTemplate.Title;
                if (MatchTitle(textBoxTitle.Text) && formerName != textBoxTitle.Text)
                {
                    textBoxTitle.BackColor = Color.Red;
                }
                else
                {

                    parentTemplate.ThetaFinish = Convert.ToDouble(textBoxThetaFinish.Text.Replace(".", ","));
                    parentTemplate.ThetaStart = Convert.ToDouble(textBoxThetaStart.Text.Replace(".", ","));
                    parentTemplate.PhiFinish = Convert.ToDouble(textBoxPhiFinish.Text.Replace(".", ","));
                    parentTemplate.PhiStart = Convert.ToDouble(textBoxPhiStart.Text.Replace(".", ","));
                    parentTemplate.Delta = Convert.ToDouble(textBoxStep.Text.Replace(".", ","));
                    parentTemplate.SystemOfCoordinates = comboBoxFarFieldSystem.SelectedIndex;
                    
                    parentTemplate.Title = textBoxTitle.Text;

                    parent.ChangeFarFieldTreeViewName(formerName, parentTemplate.Title);
                    parent.renderControl1.ReDrawFarFieldRequest(formerName, textBoxTitle.Text, ThetaStart, ThetaFinish, PhiStart, PhiFinish, Delta, FarFieldRequestForm.SystemOfCoordinates, Color.FromArgb(225, 250, 0).ToArgb());
                    Close();
                }               
            }
            else
            {
                if (MatchTitle(textBoxTitle.Text))
                {
                    textBoxTitle.BackColor = Color.Red;
                }
                else
                {
                    var template = new FarFieldRequestTemplate(this);
                    parent.RequestsF.Add(template);
                    parent.AddFarFieldTreeView(template);

                    parent.renderControl1.DrawFarFieldRequest(template.Title, ThetaStart, ThetaFinish, PhiStart, PhiFinish, Delta, FarFieldRequestForm.SystemOfCoordinates, Color.FromArgb(225, 250, 0).ToArgb());
                    Close();
                }
            }           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadDefaultValues()
        {
            textBoxThetaFinish.Text = FarFieldRequestForm.ThetaFinish.ToString();
            textBoxThetaStart.Text = FarFieldRequestForm.ThetaStart.ToString();
            textBoxPhiFinish.Text = FarFieldRequestForm.PhiFinish.ToString();
            textBoxPhiStart.Text = FarFieldRequestForm.PhiStart.ToString();
            textBoxStep.Text = FarFieldRequestForm.Delta.ToString();
            comboBoxFarFieldSystem.SelectedIndex = FarFieldRequestForm.SystemOfCoordinates;

            int k = 0;
            do
            {
                k++;
                textBoxTitle.Text = String.Concat(FarFieldRequestForm.initialTitle, k);                
            } while (MatchTitle(textBoxTitle.Text));
            
        }

        private void RefreshFormValues()
        {
            FarFieldRequestForm.ThetaFinish = Convert.ToDouble(textBoxThetaFinish.Text.Replace(".", ","));
            FarFieldRequestForm.ThetaStart = Convert.ToDouble(textBoxThetaStart.Text.Replace(".", ","));
            FarFieldRequestForm.PhiFinish = Convert.ToDouble(textBoxPhiFinish.Text.Replace(".", ","));
            FarFieldRequestForm.PhiStart = Convert.ToDouble(textBoxPhiStart.Text.Replace(".", ","));
            FarFieldRequestForm.Delta = Convert.ToDouble(textBoxStep.Text.Replace(".", ","));
            FarFieldRequestForm.SystemOfCoordinates = comboBoxFarFieldSystem.SelectedIndex;           
        }

        private void ApplyFormValues(FarFieldRequestTemplate template)
        {
            textBoxThetaFinish.Text = Convert.ToString(template.ThetaFinish).Replace(",", ".");
            textBoxThetaStart.Text = Convert.ToString(template.ThetaStart).Replace(",", ".");
            textBoxPhiFinish.Text = Convert.ToString(template.PhiFinish).Replace(",", ".");
            textBoxPhiStart.Text = Convert.ToString(template.PhiStart).Replace(",", ".");
            textBoxStep.Text = Convert.ToString(template.Delta).Replace(",", ".");
            comboBoxFarFieldSystem.SelectedIndex = template.SystemOfCoordinates;
            textBoxTitle.Text = template.Title;
        }
        public bool MatchTitle(string title)
        {
            bool match = false;
            foreach (TreeNode node in parent.treeViewConfiguration.Nodes[3].Nodes)
            {
                if (node.Name == title)
                {
                    match = true;
                }
            }
            return match;
        }
    }    
}
