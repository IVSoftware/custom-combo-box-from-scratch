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
            CustomDropDownListFromScratch.Item item1 = new CustomDropDownListFromScratch.Item();
            CustomDropDownListFromScratch.Item item2 = new CustomDropDownListFromScratch.Item();
            CustomDropDownListFromScratch.Item item3 = new CustomDropDownListFromScratch.Item();
            CustomDropDownListFromScratch.Item item4 = new CustomDropDownListFromScratch.Item();
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
            item1.Style = CustomDropDownListFromScratch.ItemStyle.Button;
            item1.Text = "Item A";
            item2.Style = CustomDropDownListFromScratch.ItemStyle.Button;
            item2.Text = "Item B";
            item3.Style = CustomDropDownListFromScratch.ItemStyle.Button;
            item3.Text = "Item C";
            item4.Style = CustomDropDownListFromScratch.ItemStyle.Button;
            item4.Text = "Item D";
            customDropDown.Items.Add(item1);
            customDropDown.Items.Add(item2);
            customDropDown.Items.Add(item3);
            customDropDown.Items.Add(item4);
            customDropDown.Location = new Point(34, 133);
            customDropDown.Margin = new Padding(0);
            customDropDown.Name = "customDropDown";
            customDropDown.RowCount = 1;
            customDropDown.RowStyles.Add(new RowStyle());
            customDropDown.Size = new Size(394, 67);
            customDropDown.TabIndex = 0;
            // 
            // textBoxNewItem
            // 
            textBoxNewItem.Font = new Font("Segoe UI", 12F);
            textBoxNewItem.Location = new Point(117, 54);
            textBoxNewItem.Name = "textBoxNewItem";
            textBoxNewItem.PlaceholderText = "New Item";
            textBoxNewItem.Size = new Size(311, 39);
            textBoxNewItem.TabIndex = 1;
            // 
            // buttonAdd
            // 
            buttonAdd.Enabled = false;
            buttonAdd.Location = new Point(34, 63);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(77, 34);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "ADD";
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
