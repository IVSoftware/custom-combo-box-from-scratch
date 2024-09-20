Your comment says:

> ...based on your feedback Jimi and IV. Maybe you have some extra pointers.

___

If it were based on my feedback in particular, a basic implementation might look more like this (before implementing custom functionality you show in your answer like `initialDropDownButtonText` and `itemsList`). 

[![custom combo box][1]][1]

___

Sometimes we go to great lengths to do custom draws and such on `ComboBox`, trying to teach the proverbial pig to sing (which wastes your time and annoys the pig). In my own experience, it can be less painful to start from scratch, inheriting `TableLayoutPanel` instead of `ComboBox` (or any other control), and when the time comes to make the drop down visible, show a top level borderless form with a docked `FlowLayoutPanel` that can contain "anything under the sun", making sure it tracks any movement of the `TopLevelForm` while it's visible.

The first thing out new control is going to need is an `Items` collection.

```
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
public BindingList<Item> Items { get; } = new BindingList<Item>();
```

Since the stated goal is _adding some features_, make a custom item to support the intended features. In this example, one such feature is individual drop down items that can be styled with `ForeColor` and `BackColor`, and can display with either a `Button` appearance or a `CheckBox` appearance.

```
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
```

##### Adding Items at in the Visual Studio Designer

The ability to edit the `Items` collection in the Visual Studio Designer requires no effort on our part; it's there by default.



___

```
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
            // Don't allow the drop down window to dispose!
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
```