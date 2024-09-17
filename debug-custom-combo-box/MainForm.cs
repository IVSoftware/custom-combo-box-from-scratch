namespace debug_custom_combo_box
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
    }

    public class DropdownCheckBox : CheckBox
    {
        private int initialWidth;

        private string dropdownText;

       // private Icon dropdownIcon = Properties.Resources.DropDownArrow;

        public string DropDownText
        {
            get { return dropdownText; }
            set
            {
                dropdownText = value;
                this.Text = value;
            }
        }

        public DropdownCheckBox()
        {
            TextAlign = ContentAlignment.MiddleLeft;
            Margin = new Padding(0);
            Padding = new Padding(0);
            FlatAppearance.BorderSize = 1;
            FlatStyle = FlatStyle.Flat;
            BackColor = SystemColors.Window;
            Appearance = Appearance.Button;
            FlatAppearance.MouseOverBackColor = SystemColors.Window;
            FlatAppearance.MouseDownBackColor = SystemColors.Window;
            initialWidth = this.Width;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Text = dropdownText;

            Rectangle dropDownIconRectangle = new Rectangle(this.Width - this.Height + 2, 2, this.Height - 4, this.Height - 4);

            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Control), dropDownIconRectangle);

            //e.Graphics.DrawIcon(dropdownIcon, dropDownIconRectangle);
        }
    }
}
