namespace Flasma_IOT_01.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            btnStop = new Button();
            label3 = new Label();
            panel1 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgvHistory = new DataGridView();
            tabPage2 = new TabPage();
            Chart_Current = new ScottPlot.WinForms.FormsPlot();
            Chart_Volt = new ScottPlot.WinForms.FormsPlot();
            TxtBarcode = new TextBox();
            BtnDisConnect = new Button();
            btnConnect = new Button();
            label4 = new Label();
            btnTestData = new Button();
            lblCurrentAmpe = new Label();
            lblCurentVolt = new Label();
            chkDummyMode = new CheckBox();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(294, 764);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(131, 40);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(431, 764);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(131, 40);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(778, 716);
            label3.Name = "label3";
            label3.Size = new Size(107, 30);
            label3.TabIndex = 3;
            label3.Text = "Power (W)";
            // 
            // panel1
            // 
            panel1.Controls.Add(tabControl1);
            panel1.Controls.Add(TxtBarcode);
            panel1.Controls.Add(BtnDisConnect);
            panel1.Controls.Add(btnConnect);
            panel1.Controls.Add(btnStart);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnTestData);
            panel1.Controls.Add(btnStop);
            panel1.Controls.Add(lblCurrentAmpe);
            panel1.Controls.Add(lblCurentVolt);
            panel1.Controls.Add(chkDummyMode);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1276, 816);
            panel1.TabIndex = 4;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(3, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1273, 703);
            tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvHistory);
            tabPage1.Location = new Point(4, 39);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1265, 660);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "History";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvHistory
            // 
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.Location = new Point(3, 3);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.ReadOnly = true;
            dgvHistory.RowHeadersWidth = 72;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.Size = new Size(1259, 654);
            dgvHistory.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(Chart_Current);
            tabPage2.Controls.Add(Chart_Volt);
            tabPage2.Location = new Point(4, 39);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1265, 660);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Chart";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // Chart_Current
            // 
            Chart_Current.DisplayScale = 1.75F;
            Chart_Current.Location = new Point(6, 18);
            Chart_Current.Name = "Chart_Current";
            Chart_Current.Size = new Size(1251, 316);
            Chart_Current.TabIndex = 5;
            // 
            // Chart_Volt
            // 
            Chart_Volt.DisplayScale = 1.75F;
            Chart_Volt.Location = new Point(6, 353);
            Chart_Volt.Name = "Chart_Volt";
            Chart_Volt.Size = new Size(1251, 292);
            Chart_Volt.TabIndex = 6;
            // 
            // TxtBarcode
            // 
            TxtBarcode.Location = new Point(954, 766);
            TxtBarcode.Name = "TxtBarcode";
            TxtBarcode.Size = new Size(287, 35);
            TxtBarcode.TabIndex = 8;
            // 
            // BtnDisConnect
            // 
            BtnDisConnect.Location = new Point(149, 764);
            BtnDisConnect.Name = "BtnDisConnect";
            BtnDisConnect.Size = new Size(131, 40);
            BtnDisConnect.TabIndex = 0;
            BtnDisConnect.Text = "DisConnect";
            BtnDisConnect.UseVisualStyleBackColor = true;
            BtnDisConnect.Click += BtnDisConnect_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 764);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(131, 40);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(841, 769);
            label4.Name = "label4";
            label4.Size = new Size(88, 30);
            label4.TabIndex = 3;
            label4.Text = "Barcode";
            // 
            // btnTestData
            // 
            btnTestData.Location = new Point(578, 764);
            btnTestData.Name = "btnTestData";
            btnTestData.Size = new Size(131, 40);
            btnTestData.TabIndex = 1;
            btnTestData.Text = "Get Data";
            btnTestData.UseVisualStyleBackColor = true;
            btnTestData.Click += btnTestData_Click;
            // 
            // lblCurrentAmpe
            // 
            lblCurrentAmpe.AutoSize = true;
            lblCurrentAmpe.Location = new Point(271, 716);
            lblCurrentAmpe.Name = "lblCurrentAmpe";
            lblCurrentAmpe.Size = new Size(61, 30);
            lblCurrentAmpe.TabIndex = 3;
            lblCurrentAmpe.Text = "------";
            // 
            // lblCurentVolt
            // 
            lblCurentVolt.AutoSize = true;
            lblCurentVolt.Location = new Point(26, 716);
            lblCurentVolt.Name = "lblCurentVolt";
            lblCurentVolt.Size = new Size(61, 30);
            lblCurentVolt.TabIndex = 2;
            lblCurentVolt.Text = "------";
            // 
            // chkDummyMode
            // 
            chkDummyMode.AutoSize = true;
            chkDummyMode.Location = new Point(1015, 716);
            chkDummyMode.Name = "chkDummyMode";
            chkDummyMode.Size = new Size(226, 34);
            chkDummyMode.TabIndex = 7;
            chkDummyMode.Text = "Dummy Mode (Test)";
            chkDummyMode.UseVisualStyleBackColor = true;
            chkDummyMode.CheckedChanged += chkDummyMode_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1276, 816);
            Controls.Add(panel1);
            Name = "MainForm";
            Text = "Main Form";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Label label3;
        private Panel panel1;
        private Button btnConnect;
        private Label lblCurrentAmpe;
        private Label lblCurentVolt;
        private Button BtnDisConnect;
        private Button btnTestData;
        private ScottPlot.WinForms.FormsPlot Chart_Current;
        private ScottPlot.WinForms.FormsPlot Chart_Volt;
        private CheckBox chkDummyMode;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox TxtBarcode;
        private DataGridView dgvHistory;
        private Label label4;
    }
}
