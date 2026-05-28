namespace DotNet05AsyncApp
{
    partial class Frmmain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frmmain));
            groupBox1 = new GroupBox();
            PrgProcess = new ProgressBar();
            BtnASyncCopy = new Button();
            BtnSyncCopy = new Button();
            BtnTarget = new Button();
            BtnSource = new Button();
            TxtTarget = new TextBox();
            TxtSource = new TextBox();
            label2 = new Label();
            label1 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(PrgProcess);
            groupBox1.Controls.Add(BtnASyncCopy);
            groupBox1.Controls.Add(BtnSyncCopy);
            groupBox1.Controls.Add(BtnTarget);
            groupBox1.Controls.Add(BtnSource);
            groupBox1.Controls.Add(TxtTarget);
            groupBox1.Controls.Add(TxtSource);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(437, 179);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "비동기 처리";
            // 
            // PrgProcess
            // 
            PrgProcess.Location = new Point(36, 143);
            PrgProcess.Name = "PrgProcess";
            PrgProcess.Size = new Size(365, 23);
            PrgProcess.TabIndex = 9;
            // 
            // BtnASyncCopy
            // 
            BtnASyncCopy.Location = new Point(220, 112);
            BtnASyncCopy.Name = "BtnASyncCopy";
            BtnASyncCopy.Size = new Size(103, 23);
            BtnASyncCopy.TabIndex = 7;
            BtnASyncCopy.Text = "비동기화 복사";
            BtnASyncCopy.UseVisualStyleBackColor = true;
            BtnASyncCopy.Click += BtnASyncCopy_Click;
            // 
            // BtnSyncCopy
            // 
            BtnSyncCopy.Location = new Point(113, 112);
            BtnSyncCopy.Name = "BtnSyncCopy";
            BtnSyncCopy.Size = new Size(103, 23);
            BtnSyncCopy.TabIndex = 6;
            BtnSyncCopy.Text = "동기화 복사";
            BtnSyncCopy.UseVisualStyleBackColor = true;
            BtnSyncCopy.Click += BtnSyncCopy_Click;
            // 
            // BtnTarget
            // 
            BtnTarget.Location = new Point(368, 81);
            BtnTarget.Name = "BtnTarget";
            BtnTarget.Size = new Size(33, 23);
            BtnTarget.TabIndex = 5;
            BtnTarget.Text = "...";
            BtnTarget.UseVisualStyleBackColor = true;
            BtnTarget.Click += BtnTarget_Click;
            // 
            // BtnSource
            // 
            BtnSource.Location = new Point(368, 40);
            BtnSource.Name = "BtnSource";
            BtnSource.Size = new Size(33, 23);
            BtnSource.TabIndex = 4;
            BtnSource.Text = "...";
            BtnSource.UseVisualStyleBackColor = true;
            BtnSource.Click += BtnSource_Click;
            // 
            // TxtTarget
            // 
            TxtTarget.Location = new Point(89, 82);
            TxtTarget.Name = "TxtTarget";
            TxtTarget.ReadOnly = true;
            TxtTarget.Size = new Size(256, 23);
            TxtTarget.TabIndex = 3;
            // 
            // TxtSource
            // 
            TxtSource.Location = new Point(89, 41);
            TxtSource.Name = "TxtSource";
            TxtSource.ReadOnly = true;
            TxtSource.Size = new Size(256, 23);
            TxtSource.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(36, 85);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 1;
            label2.Text = "타겟";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(36, 44);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 0;
            label1.Text = "소스";
            // 
            // Frmmain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 203);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Frmmain";
            Text = "비동기 파일복사";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ProgressBar PrgProcess;
        private Button BtnASyncCopy;
        private Button BtnSyncCopy;
        private Button BtnTarget;
        private Button BtnSource;
        private TextBox TxtTarget;
        private TextBox TxtSource;
        private Label label2;
        private Label label1;
    }
}
