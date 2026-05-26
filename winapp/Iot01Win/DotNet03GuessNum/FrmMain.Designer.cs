namespace DotNet03GuessNum
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            LblDisplay = new Label();
            TxtGuess = new TextBox();
            BtnCheck = new Button();
            BtnStart = new Button();
            SuspendLayout();
            // 
            // LblDisplay
            // 
            LblDisplay.Dock = DockStyle.Top;
            LblDisplay.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LblDisplay.Location = new Point(0, 0);
            LblDisplay.Name = "LblDisplay";
            LblDisplay.Size = new Size(316, 67);
            LblDisplay.TabIndex = 0;
            LblDisplay.Text = "게임을 시작합니다";
            LblDisplay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TxtGuess
            // 
            TxtGuess.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TxtGuess.Location = new Point(42, 70);
            TxtGuess.Name = "TxtGuess";
            TxtGuess.Size = new Size(113, 29);
            TxtGuess.TabIndex = 1;
            // 
            // BtnCheck
            // 
            BtnCheck.Location = new Point(161, 70);
            BtnCheck.Name = "BtnCheck";
            BtnCheck.Size = new Size(119, 29);
            BtnCheck.TabIndex = 2;
            BtnCheck.Text = "맞히기";
            BtnCheck.UseVisualStyleBackColor = true;
            // 
            // BtnStart
            // 
            BtnStart.Dock = DockStyle.Bottom;
            BtnStart.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BtnStart.Location = new Point(0, 121);
            BtnStart.Name = "BtnStart";
            BtnStart.Size = new Size(316, 44);
            BtnStart.TabIndex = 3;
            BtnStart.Text = "게임 시작";
            BtnStart.UseVisualStyleBackColor = true;
            BtnStart.Click += BtnStart_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(316, 165);
            Controls.Add(BtnStart);
            Controls.Add(BtnCheck);
            Controls.Add(TxtGuess);
            Controls.Add(LblDisplay);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "숫자맞히기";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LblDisplay;
        private TextBox TxtGuess;
        private Button BtnCheck;
        private Button BtnStart;
    }
}
