namespace debug_custom_combo_box
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
            customDropDown = new CustomDropDownListFromScratch();
            SuspendLayout();
            // 
            // customDropDown
            // 
            customDropDown.AutoSize = true;
            customDropDown.BackColor = Color.Azure;
            customDropDown.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            customDropDown.ColumnCount = 2;
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            customDropDown.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            customDropDown.DropDownText = "Select";
            customDropDown.Font = new Font("Segoe UI", 15F);
            customDropDown.Location = new Point(34, 60);
            customDropDown.Margin = new Padding(0);
            customDropDown.Name = "customDropDown";
            customDropDown.RowCount = 1;
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.Size = new Size(394, 67);
            customDropDown.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 244);
            Controls.Add(customDropDown);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main Form";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CustomDropDownListFromScratch customDropDown;
    }
}
