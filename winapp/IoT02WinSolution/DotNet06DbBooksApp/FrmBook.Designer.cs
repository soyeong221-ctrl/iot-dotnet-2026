namespace DotNet06DbBooksApp
{
    partial class FrmBook
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBook));
            DgvBooks = new DataGridView();
            BtnLoad = new MaterialSkin.Controls.MaterialButton();
            BtnNew = new MaterialSkin.Controls.MaterialButton();
            BtnEdit = new MaterialSkin.Controls.MaterialButton();
            BtnDelete = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)DgvBooks).BeginInit();
            SuspendLayout();
            // 
            // DgvBooks
            // 
            DgvBooks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvBooks.Location = new Point(28, 86);
            DgvBooks.Name = "DgvBooks";
            DgvBooks.Size = new Size(711, 319);
            DgvBooks.TabIndex = 0;
            // 
            // BtnLoad
            // 
            BtnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnLoad.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnLoad.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            BtnLoad.Depth = 0;
            BtnLoad.HighEmphasis = true;
            BtnLoad.Icon = null;
            BtnLoad.Location = new Point(450, 414);
            BtnLoad.Margin = new Padding(4, 6, 4, 6);
            BtnLoad.MouseState = MaterialSkin.MouseState.HOVER;
            BtnLoad.Name = "BtnLoad";
            BtnLoad.NoAccentTextColor = Color.Empty;
            BtnLoad.Size = new Size(64, 36);
            BtnLoad.TabIndex = 1;
            BtnLoad.Text = "Load";
            BtnLoad.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            BtnLoad.UseAccentColor = false;
            BtnLoad.UseVisualStyleBackColor = true;
            BtnLoad.Click += BtnLoad_Click;
            // 
            // BtnNew
            // 
            BtnNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnNew.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnNew.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            BtnNew.Depth = 0;
            BtnNew.HighEmphasis = true;
            BtnNew.Icon = null;
            BtnNew.Location = new Point(522, 414);
            BtnNew.Margin = new Padding(4, 6, 4, 6);
            BtnNew.MouseState = MaterialSkin.MouseState.HOVER;
            BtnNew.Name = "BtnNew";
            BtnNew.NoAccentTextColor = Color.Empty;
            BtnNew.Size = new Size(64, 36);
            BtnNew.TabIndex = 2;
            BtnNew.Text = "NEW";
            BtnNew.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            BtnNew.UseAccentColor = false;
            BtnNew.UseVisualStyleBackColor = true;
            BtnNew.Click += BtnNew_Click;
            // 
            // BtnEdit
            // 
            BtnEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnEdit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnEdit.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            BtnEdit.Depth = 0;
            BtnEdit.HighEmphasis = true;
            BtnEdit.Icon = null;
            BtnEdit.Location = new Point(594, 414);
            BtnEdit.Margin = new Padding(4, 6, 4, 6);
            BtnEdit.MouseState = MaterialSkin.MouseState.HOVER;
            BtnEdit.Name = "BtnEdit";
            BtnEdit.NoAccentTextColor = Color.Empty;
            BtnEdit.Size = new Size(64, 36);
            BtnEdit.TabIndex = 3;
            BtnEdit.Text = "EDIT";
            BtnEdit.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            BtnEdit.UseAccentColor = false;
            BtnEdit.UseVisualStyleBackColor = true;
            BtnEdit.Click += BtnEdit_Click;
            // 
            // BtnDelete
            // 
            BtnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnDelete.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnDelete.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            BtnDelete.Depth = 0;
            BtnDelete.HighEmphasis = true;
            BtnDelete.Icon = null;
            BtnDelete.Location = new Point(666, 414);
            BtnDelete.Margin = new Padding(4, 6, 4, 6);
            BtnDelete.MouseState = MaterialSkin.MouseState.HOVER;
            BtnDelete.Name = "BtnDelete";
            BtnDelete.NoAccentTextColor = Color.Empty;
            BtnDelete.Size = new Size(73, 36);
            BtnDelete.TabIndex = 4;
            BtnDelete.Text = "DELETE";
            BtnDelete.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            BtnDelete.UseAccentColor = false;
            BtnDelete.UseVisualStyleBackColor = true;
            BtnDelete.Click += BtnDelete_Click;
            // 
            // FrmBook
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(766, 459);
            Controls.Add(BtnDelete);
            Controls.Add(BtnEdit);
            Controls.Add(BtnNew);
            Controls.Add(BtnLoad);
            Controls.Add(DgvBooks);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(490, 350);
            Name = "FrmBook";
            Text = "MySQL Books";
            Load += FrmBook_Load;
            ((System.ComponentModel.ISupportInitialize)DgvBooks).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView DgvBooks;
        private MaterialSkin.Controls.MaterialButton BtnLoad;
        private MaterialSkin.Controls.MaterialButton BtnNew;
        private MaterialSkin.Controls.MaterialButton BtnEdit;
        private MaterialSkin.Controls.MaterialButton BtnDelete;
    }
}
