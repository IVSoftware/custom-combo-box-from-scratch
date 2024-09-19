using System.ComponentModel;
using System.Diagnostics;

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
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        var item = Items[e.NewIndex];
                        _flowLayoutPanel.Controls.Add(new Button
                        {
                            Text = item.Text,
                            Height = 80,
                            BackColor = Color.White, 
                            ForeColor = Color.Black,
                        });
                        break;
                    case ListChangedType.ItemDeleted:
                        break;
                    case ListChangedType.ItemMoved:
                        break;
                    case ListChangedType.ItemChanged:
                        break;
                    default:
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
                        control.MouseDown -= Any_ControlClick;
                        control.MouseDown += Any_ControlClick;
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
            if ((sender is Button button))
            {
                _labelDropDown.Text = button.Text;
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
        public bool PreFilterMessage(ref Message m)
        {
            var hWnd = m.HWnd;
            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    if(_dropDownContainer.Visible)
                    {
                        if(FromHandle(hWnd) is Control control)
                        {
                            BeginInvoke(()=> _dropDownContainer.Close());
                            if (ReferenceEquals(control, _buttonDropDown))
                            {
                                return true;
                            }
                        }                        
                    };
                    break;
            }
            return false;
        }
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

            [Editor(typeof(System.Drawing.Design.ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public Color ForeColor { get; set; } = Color.Black;
            public override string ToString() => Text;
        }
    }
}
