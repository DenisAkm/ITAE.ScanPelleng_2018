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
    public partial class  SetFrequencyForm : Form
    {
        MainForm parent;
        public SetFrequencyForm(MainForm form)
        {
            InitializeComponent();
            parent = form;
            Show();
        }

        private void buttonAddFrequency_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add("");
        }

        private void buttonRemoveFrequency_Click(object sender, EventArgs e)
        {            
            foreach (DataGridViewCell item in dataGridView1.SelectedCells)
            {
                if (dataGridView1.Rows.Count != item.RowIndex + 1)
                {
                    DataGridViewRow row = dataGridView1.Rows[item.RowIndex];
                    dataGridView1.Rows.Remove(row);
                }                          
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parent.Frequencies.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
			{
                if (Convert.ToDouble(dataGridView1[0, i].Value) != 0)
                {
                    parent.Frequencies.Add(Convert.ToDouble(dataGridView1[0, i].Value));
                }                
			}
            string name = "Частота [";
            for (int i = 0; i < parent.Frequencies.Count; i++)
            {
                name += parent.Frequencies[i];
                name += " ГГц";
                if (i != parent.Frequencies.Count - 1)
                {
                    name += ", ";
                }
            }
            name += "]";

            for (int i = 0; i < parent.treeViewConfiguration.Nodes.Count; i++)
            {
                if (parent.treeViewConfiguration.Nodes[i].Name == "Frequency")
                {
                    parent.treeViewConfiguration.Nodes[0].Text = name;
                }
                
            }
            
            Close();
        }

        private void SetFrequencyForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < parent.Frequencies.Count; i++)
            {
                dataGridView1.Rows.Add(parent.Frequencies[i]);
            }
        }
    }
}
