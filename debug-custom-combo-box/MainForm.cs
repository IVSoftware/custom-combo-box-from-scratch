using IVSoftware.Portable;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace debug_custom_combo_box
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            textBoxNewItem.TextChanged += (sender, e) =>
                buttonAdd.Enabled = !string.IsNullOrWhiteSpace(textBoxNewItem.Text);
            textBoxNewItem.KeyDown += (sender, e) =>
            {
                switch (e.KeyData)
                {
                    case Keys.Enter:
                        e.SuppressKeyPress = true;
                        customDropDown.Items.Add(textBoxNewItem.Text);
                        textBoxNewItem.Clear();
                        break;
                }
            };
        }
    }

    public class CustomDropDownListFromScratch : TableLayoutPanel, IMessageFilter
    {
        public CustomDropDownListFromScratch()
        {
            AutoSize = true;
            Items.ListChanged += (sender, e) =>
            {
                CheckBox checkBox;
                Item item;
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        item = Items[e.NewIndex];
                        checkBox = new CheckBox
                        {
                            Text = item.Text,
                            Height = 80,
                            BackColor = item.BackColor,
                            ForeColor = item.ForeColor,
                            Appearance =
                                item.ControlStyle == ControlStyle.Checkbox ?
                                Appearance.Normal :
                                Appearance.Button,
                        };
                        checkBox.MouseDown += Any_ControlClick;
                        _flowLayoutPanel.Controls.Add(checkBox);
                        item.Control = checkBox;
                        break;
                    case ListChangedType.ItemDeleted:
                        item = Items[e.OldIndex];
                        if(item.Control is Control control)
                        {
                            control.MouseDown -= Any_ControlClick;
                        }
                        break;
                }
            };
            _dropDownContainer.Controls.Add(_flowLayoutPanel);
            _dropDownContainer.VisibleChanged += (sender, e) =>
            {
                if (_dropDownContainer.Visible)
                {
                    _dropDownContainer.Width = Width;
                    foreach (var control in _flowLayoutPanel.Controls.OfType<Control>())
                    {
                        control.Width = Width;
                    }
                }
            };
            _dropDownContainer.FormClosing += (sender, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    _dropDownContainer.Hide();
                }
            };
            _buttonDropDown.Click += (sender, e) =>
            {
                _dropDownContainer.Location = PointToScreen(new Point(0, this.Height));
                if(!_dropDownContainer.Visible) _dropDownContainer.Show(this);
            };
            Application.AddMessageFilter(this);
            Disposed += (sender, e) =>Application.RemoveMessageFilter(this);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; 
                return cp;
            }
        }
        // Try to innoculate against handle recreation at design time.
        bool _initialized = false;
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!_initialized)
            {
                _initialized = true;
                if (ColumnCount == 0)
                {
                    ColumnCount = 2;
                    ColumnStyles.Clear();
                    ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
                }

                if (RowCount == 0)
                {
                    RowCount = 1;
                    RowStyles.Clear();
                    RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }

                if (Controls.Count == 0)
                {
                    Controls.Add(_labelDropDown, 0, 0);
                    Controls.Add(_buttonDropDown, 1, 0);
                }
            }
            // Although the intention is to allow design time
            // mods of rows and columns. for now let us know:
            Debug.Assert(ColumnStyles.Count == 2);
            Debug.Assert(ColumnCount == 2);
            Debug.Assert(RowStyles.Count == 1);
            Debug.Assert(RowCount == 1);
        }

        private void Any_ControlClick(object? sender, EventArgs e)
        {
            if ((sender is Control control))
            {
                _labelDropDown.Text = control.Text;
            }
        }
        private readonly Label _labelDropDown = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        private readonly Button _buttonDropDown = new Button
        {
            Dock = DockStyle.Fill,
            Text = "V",
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = SystemColors.ControlDark,
        };
        private Form _dropDownContainer = new Form
        {
            StartPosition = FormStartPosition.Manual,
            TopLevel = true,
            MinimumSize = new Size(0, 80),
            FormBorderStyle = FormBorderStyle.None,
            BackColor= Color.White,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        FlowLayoutPanel _flowLayoutPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
        };
        public string DropDownText
        {
            get => _labelDropDown.Text;
            set => _labelDropDown.Text = value;
        }
        const int WM_LBUTTONDOWN = 0x201;
        const int WM_KEYDOWN = 0x100;
        const int VK_ESCAPE = 0x1B;
        public bool PreFilterMessage(ref Message m)
        {
            if (_dropDownContainer.Visible)
            {
                var hWnd = m.HWnd;
                switch (m.Msg)
                {
                    case WM_LBUTTONDOWN:
                        if (FromHandle(hWnd) is Control target)
                        {
                            Point client = new Point(
                                    m.LParam.ToInt32() & 0xFFFF,
                                    m.LParam.ToInt32() >> 16);
                            if
                                (target is CheckBox checkBox &&
                                _flowLayoutPanel.Controls.Contains(checkBox))
                            {
                                if (ModifierKeys == Keys.Control)
                                {
                                    // Toggle appearance
                                    switch (checkBox.Appearance)
                                    {
                                        case Appearance.Normal:
                                            _debounce.StartOrRestart(() =>
                                            {
                                                checkBox.Appearance = Appearance.Button;
                                                checkBox.Text = checkBox.Text.Replace(nameof(CheckBox), nameof(Button), StringComparison.OrdinalIgnoreCase);
                                            });
                                            break;
                                        case Appearance.Button:
                                            _debounce.StartOrRestart(() =>
                                            {
                                                checkBox.Appearance = Appearance.Normal;
                                                checkBox.Text = checkBox.Text.Replace(nameof(Button), nameof(CheckBox), StringComparison.OrdinalIgnoreCase);
                                            });
                                            break;
                                    }
                                    return true;
                                }
                                else
                                {
                                    Size boxSize = SystemInformation.MenuCheckSize;
                                    Rectangle boxRect = new Rectangle(new Point(0, (checkBox.Height - boxSize.Height) / 2), boxSize);

                                    if (boxRect.Contains(client))
                                    {
                                        _debounce.StartOrRestart(() => checkBox.Checked = !checkBox.Checked);
                                        return true;
                                    }
                                }
                            }
                        }
                        BeginInvoke(() => _dropDownContainer.Close());
                        break;
                    case WM_KEYDOWN:
                        if (m.WParam.ToInt32() == VK_ESCAPE)
                        {
                            BeginInvoke(() => _dropDownContainer.Close());
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        // <PackageReference Include="IVSoftware.Portable.WatchdogTimer" Version="1.2.1" />
        WatchdogTimer _debounce = new WatchdogTimer{Interval = TimeSpan.FromSeconds(0.1)};
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            _dropDownContainer.Location = PointToScreen(new Point(0, this.Height));
        }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (TopLevelControl is Control valid)
            {
                valid.Move -= localOnMove;
                valid.Move += localOnMove;
                void localOnMove(object? sender, EventArgs e)
                {
                    if (_dropDownContainer.Visible)
                    {
                        Point screenPoint = this.PointToScreen(new Point(0, this.Height));
                        _dropDownContainer.Location = new Point(screenPoint.X, screenPoint.Y);
                    }
                }
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<Item> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        private BindingList<Item> _items = new BindingList<Item>();

        public enum ControlStyle
        {
            Button,
            Checkbox,
        }
        public class Item
        {
            public static implicit operator Item(string text) =>
                new Item { Text = text };
            public string Text { get; set; } = "Item";
            public ControlStyle ControlStyle { get; set; } = ControlStyle.Button;
            public Color BackColor { get; set; } = Color.White;

            [Editor(typeof(System.Drawing.Design.ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public Color ForeColor { get; set; } = Color.Black;

            [Browsable(false)]
            internal CheckBox? Control { get; set; }

            public override string ToString() => Text;
        }
    }
}
