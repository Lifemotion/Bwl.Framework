using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Interactivity;
using System.Linq; // Add this for Enumerable extension methods
using System.ComponentModel; // Add this for INotifyPropertyChanged if needed later
using Avalonia.Controls.Templates; // Add this for IDataTemplate
using Avalonia.Input;
using System.Runtime.CompilerServices; // Add this for KeyEventArgs

namespace Bwl.Framework.Avalonia
{
    public enum ComboBoxStyle
    {
        Simple,
        DropDown,
        DropDownList
    }

    public partial class ComboBoxEx : UserControl, INotifyPropertyChanged
    {
        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Helper for properties that depend on other properties
        protected bool SetAndRaise<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // --- End INotifyPropertyChanged ---

        // Style property

        public static readonly StyledProperty<ComboBoxStyle> DropDownStyleProperty =
            AvaloniaProperty.Register<ComboBoxEx, ComboBoxStyle>(
                nameof(DropDownStyle), ComboBoxStyle.DropDown);

        // No explicit setter logic needed here IF we use the Changed handler
        public ComboBoxStyle DropDownStyle
        {
            get => GetValue(DropDownStyleProperty);
            set => SetValue(DropDownStyleProperty, value); // Standard SetValue
        }


        // Items property (No change needed here for notification, ItemsSource binding handles it)
        public static readonly StyledProperty<IEnumerable> ItemsProperty =
            AvaloniaProperty.Register<ComboBoxEx, IEnumerable>(nameof(Items));
        public IEnumerable Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        // Text property (No change needed here for notification, TwoWay binding handles it)
        public static readonly StyledProperty<string> TextProperty =
           AvaloniaProperty.Register<ComboBoxEx, string>(
               nameof(Text), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // SelectedIndex property (No change needed here for notification, sync logic handles it)
        public static readonly StyledProperty<int> SelectedIndexProperty =
           AvaloniaProperty.Register<ComboBoxEx, int>(nameof(SelectedIndex), -1);
        public int SelectedIndex
        {
            get => GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }


        // SelectedItem property (No change needed here for notification, sync logic handles it)
        public static readonly StyledProperty<object> SelectedItemProperty =
           AvaloniaProperty.Register<ComboBoxEx, object>(nameof(SelectedItem));
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool IsStyleSimple
        {
            get
            {
                return DropDownStyle == ComboBoxStyle.Simple;
            }
        }


        public bool IsStyleDropDown
        {
            get
            {
                return DropDownStyle == ComboBoxStyle.DropDown;
            }
        }

        public bool IsStyleDropDownList
        {
            get
            {
                return DropDownStyle == ComboBoxStyle.DropDownList;
            }
        }

        // --- Property Change Callbacks ---

        static ComboBoxEx()
        {
            // Property changed handlers remain the same
            DropDownStyleProperty.Changed.AddClassHandler<ComboBoxEx>((x, e) => x.OnDropDownStyleChanged(e));
            ItemsProperty.Changed.AddClassHandler<ComboBoxEx>((x, e) => x.OnItemsChanged(e));
            TextProperty.Changed.AddClassHandler<ComboBoxEx>((x, e) => x.OnTextChanged(e));
            SelectedIndexProperty.Changed.AddClassHandler<ComboBoxEx>((x, e) => x.OnSelectedIndexChanged(e));
            SelectedItemProperty.Changed.AddClassHandler<ComboBoxEx>((x, e) => x.OnSelectedItemChanged(e));
        }

        // --- Event Handlers ---

        // References to internal controls (get them in OnApplyTemplate or constructor after InitializeComponent)
        private TextBox? _simpleInput;
        private ListBox? _simpleItems;
        private TextBox? _dropDownInput;
        private ComboBox? _dropDownList;
        private MenuFlyout? _dropDownFlyout;
        private Border? _dropDownPanel; // Reference needed for flyout logic 

        public ComboBoxEx()
        {
            InitializeComponent();
            DataContext = this; // Setting DataContext=this can sometimes cause issues.
                                    // If bindings work without it, consider removing it.
                                    // If internal bindings break, use RelativeSource TemplatedParent in XAML instead.
                                    // For now, let's assume it's okay based on previous steps.
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // --- Detach existing handlers if template is reapplied ---
            if (_simpleInput != null) _simpleInput.TextChanged -= SimpleOrDropDownInput_TextChanged;
            if (_dropDownInput != null) _dropDownInput.TextChanged -= SimpleOrDropDownInput_TextChanged;
            if (_simpleItems != null) _simpleItems.SelectionChanged -= SimpleItems_SelectionChanged;
            if (_dropDownList != null) _dropDownList.SelectionChanged -= DropDownList_SelectionChanged;
            // Detach flyout item handlers if previously attached (more complex, see below)

            // --- Get control references ---
            _simpleInput = e.NameScope.Find<TextBox>("SimpleInput");
            _simpleItems = e.NameScope.Find<ListBox>("SimpleItems");
            _dropDownInput = e.NameScope.Find<TextBox>("DropDownInput");
            _dropDownList = e.NameScope.Find<ComboBox>("DropDownList");
            _dropDownPanel = e.NameScope.Find<Border>("DropDownPanel"); // Get the panel for flyout

            if (_dropDownPanel != null && FlyoutBase.GetAttachedFlyout(_dropDownPanel) is MenuFlyout flyout)
            {
                _dropDownFlyout = flyout;
                // We need to handle item clicks within the flyout dynamically when Items changes.
            }


            // --- Attach new handlers ---
            if (_simpleInput != null) _simpleInput.TextChanged += SimpleOrDropDownInput_TextChanged;
            if (_dropDownInput != null) _dropDownInput.TextChanged += SimpleOrDropDownInput_TextChanged;
            if (_simpleItems != null) _simpleItems.SelectionChanged += SimpleItems_SelectionChanged;
            if (_dropDownList != null) _dropDownList.SelectionChanged += DropDownList_SelectionChanged;

            // Initial synchronization based on current style
            UpdateControlBindingsAndVisibility(DropDownStyle);
            SynchronizeInternalControls(); // Sync internal controls with public properties
        }


        // --- Property Changed Handlers ---

        private void OnDropDownStyleChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // This is the crucial part: Notify that dependent properties have changed
            OnPropertyChanged(nameof(IsStyleSimple));
            OnPropertyChanged(nameof(IsStyleDropDown));
            OnPropertyChanged(nameof(IsStyleDropDownList));

            // Existing logic to update layout/sync based on the new style
            if (e.NewValue is ComboBoxStyle newStyle)
            {
                UpdateControlBindingsAndVisibility(newStyle);
                SynchronizeInternalControls(); // Re-sync when style changes
            }
        }

        // OnItemsChanged, OnTextChanged, etc., remain the same as previous suggestion
        private void OnItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // Optional: Explicitly notify if needed, though ItemsSource bindings usually update automatically.
            // OnPropertyChanged(nameof(Items));

            // Existing logic:
            PopulateDropDownFlyout();
            SynchronizeFromText(Text);
        }

        private void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // Avoid feedback loops
            if (_isUpdatingInternally) return;

            var newValue = e.GetNewValue<string>() ?? string.Empty;

            // Update internal controls based on style
            if (DropDownStyle == ComboBoxStyle.Simple && _simpleInput != null && _simpleInput.Text != newValue)
            {
                _simpleInput.Text = newValue;
            }
            else if (DropDownStyle == ComboBoxStyle.DropDown && _dropDownInput != null && _dropDownInput.Text != newValue)
            {
                _dropDownInput.Text = newValue;
            }
            // DropDownList text is handled via SelectedItem

            // Try to find the item matching the text
            SynchronizeFromText(newValue);
        }


        private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (_isUpdatingInternally) return;

            var newIndex = e.GetNewValue<int>();
            SynchronizeFromIndex(newIndex);
        }

        private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (_isUpdatingInternally) return;

            var newItem = e.GetNewValue<object>();
            SynchronizeFromItem(newItem);
        }


        // --- Internal Control Event Handlers ---

        private void SimpleOrDropDownInput_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SetPublicProperty(TextProperty, textBox.Text);
            }
        }

        private void SimpleItems_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_simpleItems != null)
            {
                SetPublicProperty(SelectedIndexProperty, _simpleItems.SelectedIndex);
                SetPublicProperty(SelectedItemProperty, _simpleItems.SelectedItem);
                if (_simpleItems.SelectedItem != null)
                {
                    SetPublicProperty(TextProperty, GetItemText(_simpleItems.SelectedItem));
                }
            }
        }

        private void DropDownList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_dropDownList != null)
            {
                SetPublicProperty(SelectedIndexProperty, _dropDownList.SelectedIndex);
                SetPublicProperty(SelectedItemProperty, _dropDownList.SelectedItem);
                // Text is implicitly handled by SelectedItem synchronization for DropDownList
            }
        }

        private void DropDownFlyoutItem_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext != null)
            {
                var selectedItem = menuItem.DataContext;
                SetPublicProperty(SelectedItemProperty, selectedItem);
                // SelectedItem change will trigger index and text updates
                _dropDownFlyout?.Hide(); // Close the flyout
            }
        }

        // --- Synchronization Logic ---

        private bool _isUpdatingInternally = false;

        // Helper to set properties and avoid feedback loops
        private void SetPublicProperty<T>(StyledProperty<T> property, T value)
        {
            if (_isUpdatingInternally) return;

            _isUpdatingInternally = true;
            try
            {
                SetValue(property, value);
            }
            finally
            {
                _isUpdatingInternally = false;
            }
        }

        // Sync Index/Item based on Text
        private void SynchronizeFromText(string? text)
        {
            if (Items == null)
            {
                SetPublicProperty(SelectedIndexProperty, -1);
                SetPublicProperty(SelectedItemProperty, null);
                return;
            }

            int index = -1;
            object? foundItem = null;
            int currentIndex = 0;
            foreach (var item in Items)
            {
                if (item != null && GetItemText(item).Equals(text, StringComparison.Ordinal)) // Adjust comparison if needed
                {
                    index = currentIndex;
                    foundItem = item;
                    break;
                }
                currentIndex++;
            }

            SetPublicProperty(SelectedIndexProperty, index);
            SetPublicProperty(SelectedItemProperty, foundItem);

            // Also update internal controls if needed (e.g., DropDownList selection)
            SynchronizeInternalControls();
        }

        // Sync Text/Item based on Index
        private void SynchronizeFromIndex(int index)
        {
            if (Items == null || index < 0)
            {
                SetPublicProperty(SelectedItemProperty, null);
                // Don't clear Text if index is -1, user might be typing
                if (index < 0 && DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    // Keep Text as is for Simple/DropDown if index is -1
                }
                else
                {
                    SetPublicProperty(TextProperty, string.Empty);
                }
                SynchronizeInternalControls();
                return;
            }

            object? item = Items.Cast<object>().ElementAtOrDefault(index);
            SetPublicProperty(SelectedItemProperty, item);
            SetPublicProperty(TextProperty, item != null ? GetItemText(item) : string.Empty);

            SynchronizeInternalControls();
        }

        // Sync Text/Index based on Item
        private void SynchronizeFromItem(object? item)
        {
            if (item == null)
            {
                SetPublicProperty(SelectedIndexProperty, -1);
                // Don't clear Text if item is null, user might be typing (unless DropDownList)
                if (DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    SetPublicProperty(TextProperty, string.Empty);
                }
            }
            else
            {
                int index = -1;
                int currentIndex = 0;
                if (Items != null)
                {
                    foreach (var current in Items)
                    {
                        if (Equals(current, item)) // Use Equals for object comparison
                        {
                            index = currentIndex;
                            break;
                        }
                        currentIndex++;
                    }
                }
                SetPublicProperty(SelectedIndexProperty, index);
                SetPublicProperty(TextProperty, GetItemText(item));
            }
            SynchronizeInternalControls();
        }


        // Syncs the internal controls (TextBoxes, ListBox, ComboBox) based on the public properties
        private void SynchronizeInternalControls()
        {
            if (_isUpdatingInternally) return; // Prevent loops during initial sync or style change
            _isUpdatingInternally = true;
            try
            {
                string currentText = Text ?? string.Empty;
                int currentIndex = SelectedIndex;
                object? currentItem = SelectedItem;

                // Simple Style
                if (_simpleInput != null && _simpleInput.Text != currentText) _simpleInput.Text = currentText;
                if (_simpleItems != null)
                {
                    if (_simpleItems.SelectedIndex != currentIndex) _simpleItems.SelectedIndex = currentIndex;
                    if (_simpleItems.SelectedItem != currentItem) _simpleItems.SelectedItem = currentItem;
                }

                // DropDown Style
                if (_dropDownInput != null && _dropDownInput.Text != currentText) _dropDownInput.Text = currentText;
                // DropDown selection is handled by flyout clicks -> SelectedItem

                // DropDownList Style
                if (_dropDownList != null)
                {
                    if (_dropDownList.SelectedIndex != currentIndex) _dropDownList.SelectedIndex = currentIndex;
                    if (_dropDownList.SelectedItem != currentItem) _dropDownList.SelectedItem = currentItem;
                    // Text for DropDownList is implicitly set by its SelectedItem
                }
            }
            finally
            {
                _isUpdatingInternally = false;
            }
        }

        // --- Helper Methods ---

        private void UpdateControlBindingsAndVisibility(ComboBoxStyle style)
        {
            // No need to change bindings dynamically here if they are already set up
            // Just ensure the correct controls are visible
            // The visibility is already handled by bindings in XAML

            // Re-populate flyout items if switching to DropDown
            if (style == ComboBoxStyle.DropDown)
            {
                PopulateDropDownFlyout();
            }
        }

        private void PopulateDropDownFlyout()
        {
            if (_dropDownFlyout == null || DropDownStyle != ComboBoxStyle.DropDown) return;

            var menuItemsSource = new System.Collections.Generic.List<MenuItem>(); // Or appropriate collection

            if (Items != null)
            {
                foreach (var item in Items)
                {
                    var menuItem = new MenuItem
                    {
                        Header = GetItemText(item), // Display text
                        DataContext = item         // Store the actual item
                    };
                    menuItem.Click += DropDownFlyoutItem_Click; // Attach handler
                    menuItemsSource.Add(menuItem);
                }
            }
            // Set the items for the flyout
            // Note: MenuFlyout doesn't have ItemsSource in all versions.
            // If ItemsSource isn't available directly, you might need to clear/add items manually
            _dropDownFlyout.Items.Clear();
            foreach (var mi in menuItemsSource) { _dropDownFlyout.Items.Add(mi); }
        }


        // Gets the string representation of an item (customize if needed)
        private string GetItemText(object? item)
        {
            if (item == null) return string.Empty;
            // TODO: Add support for DisplayMemberPath if needed
            return item.ToString() ?? string.Empty;
        }

        // Original DropDownButton_Click
        private void DropDownButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_dropDownPanel != null)
            {
                PopulateDropDownFlyout(); // Ensure flyout is populated before showing
                FlyoutBase.ShowAttachedFlyout(_dropDownPanel);
            }
        }
    }
}