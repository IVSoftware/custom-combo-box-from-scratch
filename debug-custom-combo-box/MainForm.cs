

using System;
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
            buttonTest.Click += (sender, e) =>
            {
                var editor = new CustomStringCollectionEditor();
                var testList = new BindingList<string> { "Item 1", "Item 2", "Item 3" };
                ITypeDescriptorContext? context = null;
                IWindowsFormsEditorService editorService = new MockWindowsFormsEditorService();
                var serviceProvider = new SimpleServiceProvider();
                serviceProvider.AddService(typeof(IWindowsFormsEditorService), editorService);

                if (serviceProvider is IWindowsFormsEditorService validProvider)
                {
                    if (serviceProvider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService validService)
                    {
                        // Invoke the editor
                        var result = editor.EditValue(context, (IServiceProvider)validProvider, testList);
                        MessageBox.Show($"Result: {string.Join(", ", result as IList<string>)}");
                    }
                    else Debug.Fail("Expecting valid service");
                }
                else Debug.Fail("Expecting valid provider");
            };
        }

        private void customDropDown_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    public class SimpleServiceProvider : IWindowsFormsEditorService, IServiceProvider
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void AddService(Type serviceType, object serviceInstance)
        {
            services[serviceType] = serviceInstance;
        }

        public void CloseDropDown() => throw new NotImplementedException();

        public void DropDownControl(Control? control) => throw new NotImplementedException();

        public object GetService(Type serviceType)
        {
            services.TryGetValue(serviceType, out var service);
            return service;
        }

        public DialogResult ShowDialog(Form dialog)
        {
            return default;
        }
    }

    public class MockWindowsFormsEditorService : IWindowsFormsEditorService
    {
        public DialogResult ShowDialog(Form dialog) => dialog.ShowDialog();

        public void DropDownControl(Control control) =>
            throw new NotImplementedException("Not required for modal editors");

        public void CloseDropDown() =>
            throw new NotImplementedException("Not required for modal editors");
    }


    public class CustomDropDownListFromScratch : TableLayoutPanel, IMessageFilter
    {
        public CustomDropDownListFromScratch()
        {
            AutoSize = true;

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
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Init if InitializeComponent of the parent
            // has run and there are still 0 counts.
            if (ColumnCount == 0)
            {
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            }
            if(RowCount == 0)
            {
                RowCount = 1;
                RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            if (Controls.Count == 0)
            {
                Controls.Add(_labelDropDown, 0, 0);
                Controls.Add(_buttonDropDown, 1, 0);
            }
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
        // This custom editor IS NOT FOUND by Designer.
        [Editor(typeof(CustomStringCollectionEditor), typeof(UITypeEditor))]
        public ObservableCollection<StringItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        private ObservableCollection<StringItem> _items = new ObservableCollection<StringItem>();
        #endregion Init Text (from Designer), Init Items (From Designer), Add and Remove Items (Runtime)

        public class StringItem
        {
            public string Text { get; set; } = "Item";
            public string Value { get; set; } = string.Empty;

            public override string ToString() => Text;
        }
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
    public class CustomStringCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value)
        {
            if (
                provider is IWindowsFormsEditorService validProvider &&
                provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
            {
                if (editorService != null && value is IList<string> strings)
                {
                    using (Form form = new Form
                    {
                        FormBorderStyle = FormBorderStyle.None,
                        StartPosition = FormStartPosition.CenterParent,
                        Width = 300,
                        Height = 400,
                        Padding = new Padding(2),
                        BackColor = Color.Maroon,
                    })
                    {
                        TextBox textBox = new TextBox
                        {
                            Multiline = true,
                            Dock = DockStyle.Fill,
                            Text = string.Join(Environment.NewLine, strings)
                        };
                        Button okButton = new Button
                        {
                            Text = "OK",
                            Dock = DockStyle.Bottom,
                            DialogResult = DialogResult.OK,
                            Height = 50,
                            ForeColor = Color.White,
                        };

                        Button cancelButton = new Button
                        {
                            Text = "Cancel",
                            Dock = DockStyle.Bottom,
                            DialogResult = DialogResult.Cancel,
                            Height = 50,
                            ForeColor = Color.White,
                        };

                        form.Controls.Add(textBox);
                        form.Controls.Add(okButton);
                        form.Controls.Add(cancelButton);
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
