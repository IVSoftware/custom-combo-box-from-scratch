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

Since the stated goal is _adding some features_, make a custom item that supports the functionality you intend. In this example, one such feature is individual drop down items that can be styled with `ForeColor` and `BackColor`, and can display with either a `Button` appearance or a `CheckBox` appearance. 

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

Having a collection of items provides a more flexible alternative to the constructor you show in your answer:

```
public CustomComboBox(string initialDropDownButtonText, string[] itemsList){ ... }
```

Now, by exposing both the `Items` collection and perhaps a `PlaceholderText` property for when a selection has not been made, there are multiple ways to edit the items:

- Add the styled drop down items in the Designer so that they're compiled in.
- Modify the `Items` collection programmatically at runtime in the code.
- Provide the user with an interface to add or remove items dynamically at runtime.

###### In the Designer

[Placeholder]

###### At runtime

[Placeholder]

___

##### _...Based on IV feedback..._

I had commented:
> Start by inheriting TableLayoutPanel instead of ComboBox (or any other control), and when the time comes to make the drop down visible, show a top level borderless form with a docked FlowLayoutPanel that can contain "anything under the sun", making sure it tracks any movement of the TopLevelForm while it's visible. 

Here's what a basic implementation of that comment looks like:

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
    .
    .
    .
}
```
___

##### Full Example

My comment below provides a link to my GitHub repo where you can [Browse or Clone]() how I added my extra features.

- Style drop down items with `ForeColor` and `BackColor`
- Display as either a `Button` appearance or a `CheckBox` appearance.
- Add or Remove styled items at runtime.
- Close the dropdown if [Escape] key is pressed, or if mouse is clicked "anywhere else".
- Toggle a checkbox without closing the dropdown.
- Toggle between `Button` appearance and `CheckBox` appearance at runtime using [Control] + Click.
- Reset to PlaceholderText if dynamic item removal makes it invalid.

