

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Security.Cryptography;
using System.Windows.Forms.Design;

namespace debug_custom_combo_box
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var items = new ComboBox().Items;
        }
    }

    public class CustomDropDownListFromScratch : TableLayoutPanel, IMessageFilter
    {
        public CustomDropDownListFromScratch()
        {
            ColumnCount = 2;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            RowCount = 1;
            RowStyles.Add(new RowStyle(SizeType.AutoSize));
            AutoSize = true;
            Controls.Add(_labelDropDown, 0, 0);
            Controls.Add(_buttonDropDown, 1, 0);

            for (int i = 1; i <= 3; i++)
            {
                _flowLayoutPanel.Controls.Add(new Button
                {
                    Text = $"Item {i}",
                    Height = 80,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                });
            }

            _dropDownContainer.Controls.Add(_flowLayoutPanel);
            _dropDownContainer.VisibleChanged += (sender, e) =>
            {
                if (_dropDownContainer.Visible)
                {
                    _dropDownContainer.Width = Width;
                    foreach (var btn in _flowLayoutPanel.Controls.OfType<Button>())
                    {
                        btn.Width = Width;
                        btn.MouseDown -= Any_ButtonClick;
                        btn.MouseDown += Any_ButtonClick;
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
        private void Any_ButtonClick(object? sender, EventArgs e)
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

        #region Init Text (from Designer), Init Items (From Designer), Add and Remove Items (Runtime)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        public BindingList<string> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        private BindingList<string> _items = new BindingList<string>();
        #endregion Init Text (from Designer), Init Items (From Designer), Add and Remove Items (Runtime)
    }
    /// <summary>
    /// Mutable item class that defaults to 2 columns (Auto, 80) where
    /// the Label is on the left and a button with the legend "-" is
    /// in column 1 (this will eventually allow removal of the item)
    /// </summary>
    public class CustomTableLayoutPanelItem : TableLayoutPanel
    {
        public CustomTableLayoutPanelItem()
        {
            ColumnCount = 2;
            RowCount = 1;
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));

            _label = new Label
            {
                Text = "Item",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            Controls.Add(_label, 0, 0);
            _button = new Button
            {
                Text = "-",
                Dock = DockStyle.Fill,
            };
            Controls.Add(_button, 1, 0);
        }
        [Browsable(true)]
        public new string Text
        {
            get => _label.Text;
            set => _label.Text = value;
        }
        private Label _label;
        private Button _button;
        public event EventHandler? ItemClicked;
        public override string ToString() => Text;
    }
    public class StringCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            throw new NotImplementedException();
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value)
        {
            throw new NotImplementedException();
            if (
                provider is IWindowsFormsEditorService validProvider &&
                provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
            {
                if (editorService != null && value is IList<string> strings)
                {
                    using (Form form = new Form
                    {
                        FormBorderStyle = FormBorderStyle.None,
                        StartPosition = FormStartPosition.CenterScreen,
                        Width = 300,
                        Height = 400,
                    })
                    {
                        TextBox textBox = new TextBox
                        {
                            Multiline = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill,
                            Text = string.Join(Environment.NewLine, strings)
                        };

                        Button okButton = new Button
                        {
                            Text = "OK",
                            Dock = DockStyle.Bottom,
                            DialogResult = DialogResult.OK
                        };

                        Button cancelButton = new Button
                        {
                            Text = "Cancel",
                            Dock = DockStyle.Bottom,
                            DialogResult = DialogResult.Cancel
                        };

                        form.Controls.Add(textBox);
                        form.Controls.Add(okButton);
                        form.Controls.Add(cancelButton);
                        form.AcceptButton = okButton;
                        form.CancelButton = cancelButton;

                        if (editorService.ShowDialog(form) == DialogResult.OK)
                        {
                            value = textBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                }
            }
            return value ?? new object();
        }
    }
}
