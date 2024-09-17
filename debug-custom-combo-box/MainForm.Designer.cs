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
            customDropDown = new DropdownCheckBox();
            SuspendLayout();
            // 
            // customDropDown
            // 
            customDropDown.Appearance = Appearance.Button;
            customDropDown.BackColor = SystemColors.Window;
            customDropDown.DropDownText = null;
            customDropDown.FlatStyle = FlatStyle.Flat;
            customDropDown.Font = new Font("Segoe UI", 15F);
            customDropDown.Location = new Point(61, 60);
            customDropDown.Margin = new Padding(0);
            customDropDown.Name = "customDropDown";
            customDropDown.Size = new Size(236, 67);
            customDropDown.TabIndex = 0;
            customDropDown.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 244);
            Controls.Add(customDropDown);
            Name = "MainForm";
            Text = "Main Form";
            ResumeLayout(false);
        }

        #endregion

        private DropdownCheckBox customDropDown;
    }
}
