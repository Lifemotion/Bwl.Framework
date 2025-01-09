using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Styling;
using Avalonia;

namespace Bwl.Framework.Avalonia
{
    public partial class DatagridLogWriter : UserControl, ILogWriter
    {
        const int messagesLimit = 1024 * 8;
        private DispatcherTimer _tWrite;
        private DispatcherTimer _tFilterApply;
        private List<LogItem> newMessages = new List<LogItem>();
        private List<LogItem> oldMessages = new List<LogItem>();
        private bool working = true;
        public string rootName = "Root";

        private readonly object _newMessagesListLock = new object();

        private ObservableCollection<LogItem> messages = new ObservableCollection<LogItem>();

        private class LogItem
        {
            public string message { get; set; }
            public string additional { get; set; }
            public DateTime dateTime { get; set; }
            public LogEventType type { get; set; }
            public string[] path { get; set; }
            public string pathCombined { get; set; }
            public string className { get; set; }

            public string typeAsString
            {
                get => GetTypeName(type);
            }

            private string GetTypeName(LogEventType type)
            {
                if (type == LogEventType.information)
                    return "ÈÍÔ";
                if (type == LogEventType.errors)
                    return "ÎØÁ";
                if (type == LogEventType.message)
                    return "ÑÁÙ";
                if (type == LogEventType.warning)
                    return "ÏÐÄ";
                if (type == LogEventType.debug)
                    return "ÎÒË";
                return "";
            }

            public string color
            {
                get => GetRowColor(type);
            }

            private string GetRowColor(LogEventType type)
            {
                var isDarkTheme = Application.Current.ActualThemeVariant == ThemeVariant.Dark;
                if (type == LogEventType.debug)
                    return isDarkTheme ? "#3b3b3b" : "#C8C8C8";
                if (type == LogEventType.errors)
                    return isDarkTheme ? "#691515" : "#FFB4B4";
                if (type == LogEventType.message)
                    return isDarkTheme ? "#3c3c52" : "#F0F0FF";
                if (type == LogEventType.warning)
                    return isDarkTheme ? "#454536" : "#FFFFDC";
                return isDarkTheme ? "#000000" : "#FFFFFF";
            }
        }

        private string GetTypeName(LogEventType type)
        {
            if (type == LogEventType.information)
                return "ÈÍÔ";
            if (type == LogEventType.errors)
                return "ÎØÁ";
            if (type == LogEventType.message)
                return "ÑÁÙ";
            if (type == LogEventType.warning)
                return "ÏÐÄ";
            if (type == LogEventType.debug)
                return "ÎÒË";
            return "";
        }

        public DatagridLogWriter()
        {
            InitializeComponent();

            // Initialize timers
            _tWrite = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _tWrite.Tick += tWrite_Tick;
            _tWrite.Start();

            _tFilterApply = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _tFilterApply.Tick += tFilterApply_Tick;

            grid.ItemsSource = messages;

            cbExtended_CheckedChanged(null, null);
        }

        private void tWrite_Tick(object sender, EventArgs e)
        {
            _tWrite.Stop();
            lock (_newMessagesListLock)
            {
                if (newMessages.Count > 0)
                {
                    lock (_gridSync)
                    {
                        while (newMessages.Count > 0)
                        {
                            var message = newMessages[0];
                            oldMessages.Add(message);
                            if (Filter(message))
                            {
                                ShowMessage(message);
                            }
                            newMessages.RemoveAt(0);
                        }

                        while (oldMessages.Count > messagesLimit)
                        {
                            oldMessages.RemoveAt(0);
                        }

                        while (messages.Count > messagesLimit)
                        {
                            messages.RemoveAt(0);
                        }

                        if (cbAutoScroll.IsChecked.GetValueOrDefault())
                        {
                            // Scroll to last item
                            if (messages.Count > 0) grid.ScrollIntoView(messages[messages.Count - 1], null);
                        }
                    }
                }
            }
            _tWrite.Start();
        }

        public void CategoryListChanged()
        {
        }
        public void ConnectedToLogger(Logger logger)
        {
        }
        public bool LogEnabled
        {
            get
            {
                return working;
            }
            set
            {
                working = value;
            }
        }

        private string GetClassName(string additional)
        {
            var parts = additional.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var subparts = part.Split('=');
                if (subparts.Length == 2)
                {
                    if (subparts[0] == "ClassName")
                        return subparts[1];
                }
            }
            return "";
        }

        public void WriteEvent(DateTime datetime, string[] path, LogEventType type, string text, params object[] @params)
        {
            lock (_newMessagesListLock)
            {
                if (working)
                {
                    LogItem item = new LogItem();
                    item.dateTime = datetime;
                    item.path = path;
                    item.message = text;
                    item.additional = "";
                    if (@params != null && @params.Length > 0)
                        item.additional = @params[0].ToString();
                    item.type = type;
                    var pathString = "";
                    if (item.path.GetUpperBound(0) >= 0)
                    {
                        pathString = item.path[0];
                        for (int i = 1; i <= item.path.Length - 1; i++)
                            pathString = item.path[i] + "." + pathString;
                    }
                    else
                        pathString = rootName;


                    item.pathCombined = pathString;
                    newMessages.Add(item);
                }
            }
        }

        private bool Filter(LogItem message)
        {
            var withBlock = message;
            if (cbCats.Items.Count > 0)
            {
                var pass = false;
                foreach (var catItem in cbCats.CheckedItems)
                {
                    string cat = catItem.Text;
                    if (cat.StartsWith("#"))
                    {
                        cat = cat.Substring(1);
                        if (cat == withBlock.pathCombined)
                        {
                            pass = true; break;
                        }
                    }
                    if (cat.StartsWith("*"))
                    {
                        cat = cat.Substring(1);
                        var className = GetClassName(withBlock.additional);
                        if (className == cat)
                        {
                            pass = true; break;
                        }
                    }
                }

                if (pass == false)
                    return false;
            }

            if (!cbDebug.IsChecked.Value && withBlock.type == LogEventType.debug)
                return false;
            if (!cbInformation.IsChecked.Value && withBlock.type == LogEventType.information)
                return false;
            if (!cbErrors.IsChecked.Value && withBlock.type == LogEventType.errors)
                return false;
            if (!cbWarnings.IsChecked.Value && withBlock.type == LogEventType.warning)
                return false;
            if (!cbMessages.IsChecked.Value && withBlock.type == LogEventType.message)
                return false;

            bool textFilter = false;

            if (tbFilter.Text?.Length == 0 | !cbFilter.IsChecked.Value)
                textFilter = true;
            else
            {
                if (!String.IsNullOrEmpty(tbFilter.Text))
                {
                    textFilter = true;
                    var parts = tbFilter.Text?.Split(",");
                    foreach (var part in parts)
                    {
                        bool found = false;
                        var trimmedPart = part.Trim();
                        if (trimmedPart == "")
                            found = true;
                        if (withBlock.message.ToLower().Contains(trimmedPart.ToLower()))
                            found = true;
                        if (withBlock.pathCombined.ToLower().Contains(trimmedPart.ToLower()))
                            found = true;
                        if (withBlock.type.ToString().ToLower().Contains(trimmedPart.ToLower()))
                            found = true;
                        if (!found)
                            textFilter = false;
                    }
                }
            }

            return textFilter;
        }


        private void ShowMessage(LogItem message)
        {
            {
                var withBlock = message;
                string pathString = withBlock.pathCombined;
                string dateString = withBlock.dateTime.ToShortDateString();
                string timeString = withBlock.dateTime.ToLongTimeString();
                string typeString = GetTypeName(withBlock.type);

                var itemToAdd = new LogItem
                {
                    dateTime = withBlock.dateTime,
                    message = withBlock.message,
                    additional = withBlock.additional,
                    pathCombined = pathString,
                    type = withBlock.type
                };

                messages.Add(itemToAdd);
            }
        }

        public void Clear()
        {
            lock (_gridSync)
            {
                oldMessages.Clear();
                messages.Clear();
            }
        }

        private object _gridSync = new object();

        private void RedrawItems()
        {
            lock (_gridSync)
            {
                messages.Clear();
                foreach (var msg in oldMessages)
                {
                    if (Filter(msg))
                        ShowMessage(msg);
                }

                // If cbAutoScroll.Checked Then
                if (messages.Count > 0) grid.ScrollIntoView(messages[messages.Count - 1], null);
            }
        }

        private void ViewSettingsChanged(object sender, RoutedEventArgs e)
        {
            RedrawItems();
        }

        public bool ShowDebug
        {
            get
            {
                return cbDebug.IsChecked.Value;
            }
            set
            {
                cbDebug.IsChecked = value;
            }
        }
        public bool ShowInformation
        {
            get
            {
                return cbInformation.IsChecked.Value;
            }
            set
            {
                cbInformation.IsChecked = value;
            }
        }
        public bool ShowMessages
        {
            get
            {
                return cbMessages.IsChecked.Value;
            }
            set
            {
                cbMessages.IsChecked = value;
            }
        }
        public bool ShowErrors
        {
            get
            {
                return cbErrors.IsChecked.Value;
            }
            set
            {
                cbErrors.IsChecked = value;
            }
        }
        public bool ShowWarnings
        {
            get
            {
                return cbWarnings.IsChecked.Value;
            }
            set
            {
                cbWarnings.IsChecked = value;
            }
        }
        public string FilterText
        {
            get
            {
                return tbFilter.Text;
            }
            set
            {
                tbFilter.Text = value;
                if (tbFilter.Text == "")
                {
                    tbFilter.IsVisible = false;
                    cbFilter.IsChecked = false;
                }
                else
                {
                    tbFilter.IsVisible = true;
                    cbFilter.IsChecked = true;
                }
            }
        }
        private void cbFilter_CheckedChanged(object sender, RoutedEventArgs e)
        {
            tbFilter.IsVisible = cbFilter.IsChecked.Value;
        }

        private void tbFilter_TextChanged(object sender, RoutedEventArgs e)
        {
            _tFilterApply.Stop();
            _tFilterApply.Start();
        }

        private void tFilterApply_Tick(object sender, EventArgs e)
        {
            _tFilterApply.Stop();
            RedrawItems();
        }

        private void grid_CellDoubleClick(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItem is LogItem selectedItem)
            {
                // Implement logic to show details
                var logInfo = new FormLogInfo(); // You need to implement this window
                logInfo.LogInfoText = new List<string>
                {
                    selectedItem.dateTime.ToShortDateString(),
                    selectedItem.dateTime.ToLongTimeString(),
                    GetTypeName(selectedItem.type),
                    selectedItem.pathCombined,
                    selectedItem.message,
                    selectedItem.additional
                };
                logInfo.Show();
            }
        }

        private void bClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void cbExtended_CheckedChanged(object sender, RoutedEventArgs e)
        {
            grid.Columns[1].IsVisible = cbExtended.IsChecked.Value;
            grid.Columns[2].IsVisible = cbExtended.IsChecked.Value;
            grid.Columns[4].IsVisible = cbExtended.IsChecked.Value;
        }

        public bool ExtendedView
        {
            get
            {
                return cbExtended.IsChecked.Value;
            }
            set
            {
                cbExtended.IsChecked = value;
            }
        }

        private void bRefreshPlaces_Click(object sender, RoutedEventArgs e)
        {
            List<string> places = new List<string>();
            lock (_newMessagesListLock)
            {
                foreach (var msg in newMessages.ToArray())
                {
                    if (places.Contains(msg.pathCombined) == false)
                        places.Add(msg.pathCombined);
                }
                foreach (var msg in oldMessages.ToArray())
                {
                    if (places.Contains(msg.pathCombined) == false)
                        places.Add(msg.pathCombined);
                }
            }
            var arr = places.ToArray();
            Array.Sort(arr);
            cbCats.Items.Clear();
            foreach (var place in arr)
                cbCats.Items.Add(new CheckedListBoxItem() { Text = "#" + place.ToString() });
            RedrawItems();
        }

        private void cbCats_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            RedrawItems();
        }

        private void bRefreshNone_Click(object sender, RoutedEventArgs e)
        {
            cbCats.Items.Clear();
            RedrawItems();
        }

        private void bRefreshClasses_Click(object sender, RoutedEventArgs e)
        {
            List<string> classes = new List<string>();
            lock (_newMessagesListLock)
            {
                foreach (var msg in newMessages.ToArray())
                {
                    var className = GetClassName(msg.additional);
                    if (classes.Contains(className) == false)
                        classes.Add(className);
                }
                foreach (var msg in oldMessages.ToArray())
                {
                    var className = GetClassName(msg.additional);
                    if (classes.Contains(className) == false)
                        classes.Add(className);
                }
            }
            var arr = classes.ToArray();
            Array.Sort(arr);
            cbCats.Items.Clear();
            foreach (var place in arr)
                cbCats.Items.Add(new CheckedListBoxItem() { Text = "*" + place.ToString() });
            RedrawItems();
        }
    }
}
