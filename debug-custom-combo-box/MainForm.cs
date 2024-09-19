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
                        checkBox
                        .AddInlineFunctionButton(new Button
                        {
                            Height = 25,
                            Width = 25,
                            BackColor = SystemColors.ControlDark,
                            ForeColor = Color.White,
                            Text = "X",
                            Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
                            FlatStyle = FlatStyle.Flat,
                        })
                        .MouseDown += Any_Remove;

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
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && !_initialized)
            {
                _initialized = true;
                if (ColumnCount < 2)
                {
                    ColumnCount = 2;
                }
                if (ColumnStyles.Count < 2)
                {
                    ColumnStyles.Clear();
                    ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
                }
                if (RowCount == 0)
                {
                    RowCount = 1;
                }
                if (RowStyles.Count == 0)
                {
                    RowStyles.Clear();
                    RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
                if (Controls.Count == 0)
                {
                    Controls.Add(_labelDropDown, 0, 0);
                    Controls.Add(_buttonDropDown, 1, 0);
                }
                DropDownText = PlaceholderText;
                _labelDropDown
                .AddInlineFunctionButton(new Button
                {
                    Height = 25,
                    Width = 25,
                    BackColor = SystemColors.ControlDark,
                    ForeColor = Color.White,
                    Text = "+",
                    Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    FlatStyle = FlatStyle.Flat,
                }, padRight: 5).Click += (sender, e) =>
                {
                    if (
                    _itemEditor.ShowDialog(this) == DialogResult.OK &&
                    _itemEditor.CurrentItem is not null)
                    {
                        Items.Add(_itemEditor.CurrentItem);
                    };
                };
            }
        }

        private void Any_ControlClick(object? sender, EventArgs e)
        {
            if ((sender is Control control))
            {
                _labelDropDown.Text = control.Text;
            }
        }
        private void Any_Remove(object? sender, MouseEventArgs e)
        {
            if(
                sender is Control control &&
                control.Parent is CheckBox checkbox &&
                checkbox.Parent is FlowLayoutPanel parent)
            {
                parent.Controls.Remove(checkbox);
                if(string.Equals(DropDownText, checkbox.Text))
                {
                    DropDownText = PlaceholderText;
                }
            };
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
        [Browsable(false)]
        internal string DropDownText
        {
            get => _labelDropDown.Text;
            set => _labelDropDown.Text = value;
        }
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                if (!Equals(_placeholderText, value))
                {
                    _labelDropDown.Text = value;
                    _placeholderText = value;
                }
            }
        }
        private string _placeholderText = "Select";
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
        private ItemEditorPropertyForm _itemEditor = new ItemEditorPropertyForm();
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
        class ItemEditorPropertyForm : Form
        {
            private PropertyGrid _propertyGrid = new PropertyGrid
            {
                HelpVisible = false,
            };
            private TableLayoutPanel _tableLayoutPanel = new TableLayoutPanel();
            public ItemEditorPropertyForm()
            {
                Text = "Edit Item Properties";
                ClientSize = new Size(400, 300);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MinimizeBox = false;
                MaximizeBox = false;
                StartPosition = FormStartPosition.CenterParent;

                _tableLayoutPanel.Dock = DockStyle.Fill;
                _tableLayoutPanel.RowCount = 2;
                _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
                _tableLayoutPanel.ColumnCount = 2;
                _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                Controls.Add(_tableLayoutPanel);

                _propertyGrid.Dock = DockStyle.Fill;
                _tableLayoutPanel.Controls.Add(_propertyGrid, 0, 0);
                _tableLayoutPanel.SetColumnSpan(_propertyGrid, 2);

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = SystemColors.ControlDark,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point),
                };
                _tableLayoutPanel.Controls.Add(okButton, 0, 1);
                AcceptButton = okButton;

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = SystemColors.ControlDark,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point),
                };
                _tableLayoutPanel.Controls.Add(cancelButton, 1, 1);
                CancelButton = cancelButton;
            }
            public new DialogResult ShowDialog(IWin32Window owner) =>
                ShowDialog(owner, new Item());
            public DialogResult ShowDialog(IWin32Window owner, Item item)
            {
                _propertyGrid.SelectedObject = item;
                return base.ShowDialog(owner);
            }
            public Item? CurrentItem => _propertyGrid.SelectedObject as Item;

            [Obsolete($"Use DialogResult ShowDialog(IWin32Window owner, Item item = null)")]
            public new DialogResult ShowDialog() => throw new NotImplementedException();
        }

    }
    static partial class Extensions
    {
        /// <summary>
        /// Adds a function button to a control and allows for immediate inline event assignment.
        /// This method positions the button on the right with a specified padding and centers it vertically.
        /// </summary>
        /// <param name="parent">The parent control to which the button is added.</param>
        /// <param name="button">The button to add to the control.</param>
        /// <param name="padRight">Right padding from the edge of the parent control.</param>
        /// <returns>The button added, allowing for inline event assignment, e.g., `.Click += ...`.</returns>
        /// <example>
        /// <code>
        /// parentControl
        /// .AddInlineFunctionButton(new Button { Text = "Click Me" }, 20)
        /// .Click += (sender, e) => { MessageBox.Show("Clicked!"); 
        /// </code>
        /// </example>
        public static Button AddInlineFunctionButton(this Control parent, Button button, int padRight = 20)
        {
            if (!parent.Controls.Contains(button))
            {
                parent.Controls.Add(button);
                localPositionButton();
            }
            parent.SizeChanged += (sender, e) => localPositionButton();
            return button;

            void localPositionButton()
            {
                button.Left = parent.Width - (button.Width + padRight);
                button.Top = (parent.Height - button.Height) / 2;
            }
        }
        /// <summary>
        /// Adds a control to a parent control's <see cref="Control.ControlCollection"/> and returns it, allowing for immediate inline modifications or event assignments.
        /// This method facilitates inline configurations and event handler attachments directly after the control is added.
        /// </summary>
        /// <typeparam name="T">The type of the control being added, which must derive from <see cref="Control"/>.</typeparam>
        /// <param name="parent">The parent control to which the control is added.</param>
        /// <param name="control">The control to be added to the parent's collection.</param>
        /// <returns>The control added, enabling inline modifications or event handling assignments such as setting properties or attaching events.</returns>
        /// <example>
        /// <code>
        /// parentControl
        /// .AddInlineControl(new Button { Text = "OK" })
        /// .Click += (sender, e) => { MessageBox.Show("OK Clicked!"); };
        /// </code>
        /// </example>

        public static T AddInlineControl<T>(this Control parent, T control) where T : Control
        {
            parent.Controls.Add(control);
            return control;
        }
    }
}
