using System;
using System.Collections;
using System.Collections.Generic;


namespace Integral
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Частота[]");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Источник");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Объект");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Цель");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenuFrequency = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripFrequency = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AppertureFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuObject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.WireGeometryLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SurfaceGeometryLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRequest = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FarFieldRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ErrorsRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBoxSolution = new System.Windows.Forms.CheckedListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.treeViewConfiguration = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonPelleng = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuWireGeometry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renderControl1 = new Integral.RenderControl();
            this.contextMenuFrequency.SuspendLayout();
            this.contextMenuSource.SuspendLayout();
            this.contextMenuObject.SuspendLayout();
            this.contextMenuRequest.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuFrequency
            // 
            this.contextMenuFrequency.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFrequency});
            this.contextMenuFrequency.Name = "contextMenuFrequency";
            this.contextMenuFrequency.Size = new System.Drawing.Size(167, 26);
            // 
            // toolStripFrequency
            // 
            this.toolStripFrequency.Name = "toolStripFrequency";
            this.toolStripFrequency.Size = new System.Drawing.Size(166, 22);
            this.toolStripFrequency.Text = "Выбрать частоту";
            this.toolStripFrequency.Click += new System.EventHandler(this.toolStripFrequency_Click);
            // 
            // contextMenuSource
            // 
            this.contextMenuSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AppertureFieldToolStripMenuItem});
            this.contextMenuSource.Name = "contextMenuSource";
            this.contextMenuSource.Size = new System.Drawing.Size(217, 26);
            // 
            // AppertureFieldToolStripMenuItem
            // 
            this.AppertureFieldToolStripMenuItem.Name = "AppertureFieldToolStripMenuItem";
            this.AppertureFieldToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.AppertureFieldToolStripMenuItem.Text = "Поле плоской аппертуры";
            this.AppertureFieldToolStripMenuItem.Click += new System.EventHandler(this.toolStripCreateAppertureForm_Click);
            // 
            // contextMenuObject
            // 
            this.contextMenuObject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WireGeometryLoadToolStripMenuItem,
            this.SurfaceGeometryLoadToolStripMenuItem});
            this.contextMenuObject.Name = "contextMenuObject";
            this.contextMenuObject.Size = new System.Drawing.Size(234, 48);
            // 
            // WireGeometryLoadToolStripMenuItem
            // 
            this.WireGeometryLoadToolStripMenuItem.Name = "WireGeometryLoadToolStripMenuItem";
            this.WireGeometryLoadToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.WireGeometryLoadToolStripMenuItem.Text = "Проволочная геометрия";
            this.WireGeometryLoadToolStripMenuItem.Click += new System.EventHandler(this.проволочнаяГеометрияToolStripMenuItem_Click);
            // 
            // SurfaceGeometryLoadToolStripMenuItem
            // 
            this.SurfaceGeometryLoadToolStripMenuItem.Enabled = false;
            this.SurfaceGeometryLoadToolStripMenuItem.Name = "SurfaceGeometryLoadToolStripMenuItem";
            this.SurfaceGeometryLoadToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.SurfaceGeometryLoadToolStripMenuItem.Text = "Криволинейная поверхность";
            // 
            // contextMenuRequest
            // 
            this.contextMenuRequest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FarFieldRequestToolStripMenuItem,
            this.ErrorsRequestToolStripMenuItem});
            this.contextMenuRequest.Name = "contextMenuRequest";
            this.contextMenuRequest.Size = new System.Drawing.Size(189, 48);
            // 
            // FarFieldRequestToolStripMenuItem
            // 
            this.FarFieldRequestToolStripMenuItem.Name = "FarFieldRequestToolStripMenuItem";
            this.FarFieldRequestToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.FarFieldRequestToolStripMenuItem.Text = "Поле в дальней зоне";
            this.FarFieldRequestToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemFarFieldRequest_Click);
            // 
            // ErrorsRequestToolStripMenuItem
            // 
            this.ErrorsRequestToolStripMenuItem.Enabled = false;
            this.ErrorsRequestToolStripMenuItem.Name = "ErrorsRequestToolStripMenuItem";
            this.ErrorsRequestToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.ErrorsRequestToolStripMenuItem.Text = "Ошибки пеленга";
            this.ErrorsRequestToolStripMenuItem.Click += new System.EventHandler(this.ErrorsRequestToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.13386F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.86614F));
            this.tableLayoutPanel1.Controls.Add(this.renderControl1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.31468F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.68532F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1270, 858);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.61604F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.38396F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 188F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 198F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(237, 852);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.checkedListBoxSolution);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 182);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Решение";
            // 
            // checkedListBoxSolution
            // 
            this.checkedListBoxSolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxSolution.FormattingEnabled = true;
            this.checkedListBoxSolution.Location = new System.Drawing.Point(3, 16);
            this.checkedListBoxSolution.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.checkedListBoxSolution.Name = "checkedListBoxSolution";
            this.checkedListBoxSolution.Size = new System.Drawing.Size(225, 163);
            this.checkedListBoxSolution.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.treeViewConfiguration);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 191);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(231, 460);
            this.groupBox4.TabIndex = 40;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Конфигурация";
            // 
            // treeViewConfiguration
            // 
            this.treeViewConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeViewConfiguration.Location = new System.Drawing.Point(3, 16);
            this.treeViewConfiguration.Name = "treeViewConfiguration";
            treeNode1.ContextMenuStrip = this.contextMenuFrequency;
            treeNode1.Name = "Frequency";
            treeNode1.Text = "Частота[]";
            treeNode2.ContextMenuStrip = this.contextMenuSource;
            treeNode2.Name = "Source";
            treeNode2.Text = "Источник";
            treeNode3.ContextMenuStrip = this.contextMenuObject;
            treeNode3.Name = "Object";
            treeNode3.Text = "Объект";
            treeNode4.ContextMenuStrip = this.contextMenuRequest;
            treeNode4.Name = "Request";
            treeNode4.Text = "Цель";
            this.treeViewConfiguration.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.treeViewConfiguration.Size = new System.Drawing.Size(225, 441);
            this.treeViewConfiguration.TabIndex = 0;
            this.treeViewConfiguration.DoubleClick += new System.EventHandler(this.treeViewConfiguration_DoubleClick);
            // 
            // groupBox2
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 657);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 192);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.Controls.Add(this.buttonPelleng, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(225, 173);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // buttonPelleng
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.buttonPelleng, 3);
            this.buttonPelleng.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPelleng.Location = new System.Drawing.Point(3, 117);
            this.buttonPelleng.Name = "buttonPelleng";
            this.buttonPelleng.Size = new System.Drawing.Size(219, 53);
            this.buttonPelleng.TabIndex = 3;
            this.buttonPelleng.Text = "Пуск";
            this.buttonPelleng.UseVisualStyleBackColor = true;
            this.buttonPelleng.Click += new System.EventHandler(this.buttonGoButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxInfo);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(246, 734);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1021, 121);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Сообщение";
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInfo.Location = new System.Drawing.Point(3, 16);
            this.textBoxInfo.Multiline = true;
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.Size = new System.Drawing.Size(1015, 102);
            this.textBoxInfo.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1270, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // contextMenuWireGeometry
            // 
            this.contextMenuWireGeometry.Name = "contextMenuWireGeometry";
            this.contextMenuWireGeometry.Size = new System.Drawing.Size(61, 4);
            // 
            // renderControl1
            // 
            this.renderControl1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.renderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl1.Location = new System.Drawing.Point(246, 3);
            this.renderControl1.Name = "renderControl1";
            this.renderControl1.Size = new System.Drawing.Size(1021, 725);
            this.renderControl1.TabIndex = 0;
            this.renderControl1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1270, 882);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Pelleng";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuFrequency.ResumeLayout(false);
            this.contextMenuSource.ResumeLayout(false);
            this.contextMenuObject.ResumeLayout(false);
            this.contextMenuRequest.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public Integral.RenderControl renderControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonPelleng;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListBoxSolution;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ContextMenuStrip contextMenuSource;
        private System.Windows.Forms.ToolStripMenuItem AppertureFieldToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuFrequency;
        private System.Windows.Forms.ToolStripMenuItem toolStripFrequency;
        private System.Windows.Forms.ContextMenuStrip contextMenuObject;
        private System.Windows.Forms.ToolStripMenuItem WireGeometryLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SurfaceGeometryLoadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuRequest;
        private System.Windows.Forms.ToolStripMenuItem FarFieldRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ErrorsRequestToolStripMenuItem;
        public System.Windows.Forms.TreeView treeViewConfiguration;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuWireGeometry;

    }
}

