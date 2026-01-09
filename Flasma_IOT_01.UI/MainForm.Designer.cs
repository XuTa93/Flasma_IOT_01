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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panel1 = new Panel();
            BtnDisConnect = new Button();
            btnConnect = new Button();
            lblCurrentAmpe = new Label();
            lblCurentVolt = new Label();
            btnTestData = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(41, 65);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(131, 40);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(41, 111);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(131, 40);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 238);
            label1.Name = "label1";
            label1.Size = new Size(114, 30);
            label1.TabIndex = 2;
            label1.Text = "Voltage (V)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 350);
            label2.Name = "label2";
            label2.Size = new Size(114, 30);
            label2.TabIndex = 3;
            label2.Text = "Current (A)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(41, 467);
            label3.Name = "label3";
            label3.Size = new Size(107, 30);
            label3.TabIndex = 3;
            label3.Text = "Power (W)";
            // 
            // panel1
            // 
            panel1.Controls.Add(BtnDisConnect);
            panel1.Controls.Add(btnConnect);
            panel1.Controls.Add(btnStart);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnTestData);
            panel1.Controls.Add(btnStop);
            panel1.Controls.Add(lblCurrentAmpe);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(lblCurentVolt);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(970, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(306, 816);
            panel1.TabIndex = 4;
            // 
            // BtnDisConnect
            // 
            BtnDisConnect.Location = new Point(24, 684);
            BtnDisConnect.Name = "BtnDisConnect";
            BtnDisConnect.Size = new Size(131, 40);
            BtnDisConnect.TabIndex = 0;
            BtnDisConnect.Text = "DisConnect";
            BtnDisConnect.UseVisualStyleBackColor = true;
            BtnDisConnect.Click += BtnDisConnect_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(24, 638);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(131, 40);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblCurrentAmpe
            // 
            lblCurrentAmpe.AutoSize = true;
            lblCurrentAmpe.Location = new Point(41, 380);
            lblCurrentAmpe.Name = "lblCurrentAmpe";
            lblCurrentAmpe.Size = new Size(61, 30);
            lblCurrentAmpe.TabIndex = 3;
            lblCurrentAmpe.Text = "------";
            // 
            // lblCurentVolt
            // 
            lblCurentVolt.AutoSize = true;
            lblCurentVolt.Location = new Point(41, 268);
            lblCurentVolt.Name = "lblCurentVolt";
            lblCurentVolt.Size = new Size(61, 30);
            lblCurentVolt.TabIndex = 2;
            lblCurentVolt.Text = "------";
            // 
            // btnTestData
            // 
            btnTestData.Location = new Point(41, 170);
            btnTestData.Name = "btnTestData";
            btnTestData.Size = new Size(131, 40);
            btnTestData.TabIndex = 1;
            btnTestData.Text = "Get Data";
            btnTestData.UseVisualStyleBackColor = true;
            btnTestData.Click += btnTestData_Click;
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
            ResumeLayout(false);
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
        private Button btnConnect;
        private Label lblCurrentAmpe;
        private Label lblCurentVolt;
        private Button BtnDisConnect;
        private Button btnTestData;
    }
}
