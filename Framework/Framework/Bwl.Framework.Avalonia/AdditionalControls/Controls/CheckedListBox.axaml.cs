using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System;

namespace Bwl.Framework.Avalonia
{
    public partial class CheckedListBox : UserControl, IDisposable
    {
        public static readonly StyledProperty<ObservableCollection<CheckedListBoxItem>> ItemsProperty =
            AvaloniaProperty.Register<CheckedListBox, ObservableCollection<CheckedListBoxItem>>(nameof(Items));

        public ObservableCollection<CheckedListBoxItem> Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public event EventHandler<RoutedEventArgs>? SelectedValueChanged;
        private bool _disposed = false;

        /// <summary>
        /// Implementation of CheckedListBox from WinForms, but for Avalonia
        /// </summary>
        public CheckedListBox()
        {
            InitializeComponent();

            Items = new ObservableCollection<CheckedListBoxItem>();
            Items.CollectionChanged += ItemsCollectionChanged;

            foreach (var item in Items)
                item.PropertyChanged += ItemPropertyChanged;

            CheckedListBoxControl.ItemsSource = Items;
        }

        /// <summary>
        /// Items were changed, so we need to subscribe to PropertyChanged event of new items and unsubscribe from old items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CheckedListBoxItem newItem in e.NewItems)
                    newItem.PropertyChanged += ItemPropertyChanged;

            if (e.OldItems != null)
                foreach (CheckedListBoxItem oldItem in e.OldItems)
                    oldItem.PropertyChanged -= ItemPropertyChanged;
        }

        /// <summary>
        /// Item's property was changed, so we need to raise SelectedValueChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckedListBoxItem.IsChecked))
                SelectedValueChanged?.Invoke(sender, new RoutedEventArgs());
        }

        public CheckedListBoxItem[] CheckedItems => Items.Where(x => x.IsChecked).ToArray();

        public void Dispose()
        {
            if (_disposed) return;
            if (Items != null)
            {
                Items.CollectionChanged -= ItemsCollectionChanged;
                foreach (var item in Items)
                    item.PropertyChanged -= ItemPropertyChanged;
                Items.Clear();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~CheckedListBox()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Item for CheckedListBox
    /// </summary>
    public class CheckedListBoxItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Text { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
