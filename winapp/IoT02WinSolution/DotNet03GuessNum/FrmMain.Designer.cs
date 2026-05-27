namespace DotNet03GuessNum
{
    partial class TxtNum
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TxtNum));
            LblDisplay = new Label();
            textBox1 = new TextBox();
            BtnCheck = new Button();
            BtnStart = new Button();
            SuspendLayout();
            // 
            // LblDisplay
            // 
            LblDisplay.Dock = DockStyle.Top;
            LblDisplay.Font = new Font("맑은 고딕", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LblDisplay.Location = new Point(0, 0);
            LblDisplay.Name = "LblDisplay";
            LblDisplay.Size = new Size(307, 35);
            LblDisplay.TabIndex = 0;
            LblDisplay.Text = "게임을 시작합니다.";
            LblDisplay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(53, 47);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(95, 23);
            textBox1.TabIndex = 1;
            // 
            // BtnCheck
            // 
            BtnCheck.Location = new Point(154, 47);
            BtnCheck.Name = "BtnCheck";
            BtnCheck.Size = new Size(104, 23);
            BtnCheck.TabIndex = 2;
            BtnCheck.Text = "확인";
            BtnCheck.UseVisualStyleBackColor = true;
            BtnCheck.Click += BtnCheck_Click;
            // 
            // BtnStart
            // 
            BtnStart.Dock = DockStyle.Bottom;
            BtnStart.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BtnStart.Location = new Point(0, 91);
            BtnStart.Name = "BtnStart";
            BtnStart.Size = new Size(307, 37);
            BtnStart.TabIndex = 3;
            BtnStart.Text = "게임 시작";
            BtnStart.UseVisualStyleBackColor = true;
            BtnStart.Click += BtnStart_Click;
            // 
            // TxtNum
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(307, 128);
            Controls.Add(BtnStart);
            Controls.Add(BtnCheck);
            Controls.Add(textBox1);
            Controls.Add(LblDisplay);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "TxtNum";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "숫자 맞히기 게임";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LblDisplay;
        private TextBox textBox1;
        private Button BtnCheck;
        private Button BtnStart;
    }
}
