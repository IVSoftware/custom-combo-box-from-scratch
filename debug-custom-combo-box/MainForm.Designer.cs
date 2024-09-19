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
            CustomDropDownListFromScratch.Item item4 = new CustomDropDownListFromScratch.Item();
            CustomDropDownListFromScratch.Item item5 = new CustomDropDownListFromScratch.Item();
            CustomDropDownListFromScratch.Item item6 = new CustomDropDownListFromScratch.Item();
            customDropDown = new CustomDropDownListFromScratch();
            textBoxNewItem = new TextBox();
            buttonAdd = new Label();
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
            customDropDown.DropDownText = "Select";
            customDropDown.Font = new Font("Segoe UI", 15F);
            item4.ControlStyle = CustomDropDownListFromScratch.ControlStyle.Button;
            item4.ForeColor = Color.FromArgb(192, 0, 0);
            item4.Text = "Red Button";
            item5.ControlStyle = CustomDropDownListFromScratch.ControlStyle.Checkbox;
            item5.ForeColor = Color.Green;
            item5.Text = "Green Checkbox";
            item6.ControlStyle = CustomDropDownListFromScratch.ControlStyle.Button;
            item6.ForeColor = Color.FromArgb(0, 0, 192);
            item6.Text = "Blue Button";
            customDropDown.Items.Add(item4);
            customDropDown.Items.Add(item5);
            customDropDown.Items.Add(item6);
            customDropDown.Location = new Point(34, 114);
            customDropDown.Margin = new Padding(0);
            customDropDown.Name = "customDropDown";
            customDropDown.RowCount = 1;
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.Size = new Size(394, 67);
            customDropDown.TabIndex = 0;
            // 
            // textBoxNewItem
            // 
            textBoxNewItem.Font = new Font("Segoe UI", 12F);
            textBoxNewItem.Location = new Point(117, 40);
            textBoxNewItem.Name = "textBoxNewItem";
            textBoxNewItem.PlaceholderText = "New Button Item";
            textBoxNewItem.Size = new Size(311, 39);
            textBoxNewItem.TabIndex = 1;
            // 
            // buttonAdd
            // 
            buttonAdd.Enabled = false;
            buttonAdd.Location = new Point(34, 44);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(77, 34);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "ADD";
            buttonAdd.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 244);
            Controls.Add(buttonAdd);
            Controls.Add(textBoxNewItem);
            Controls.Add(customDropDown);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main Form";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CustomDropDownListFromScratch customDropDown;
        private TextBox textBoxNewItem;
        private Label buttonAdd;
    }
}
