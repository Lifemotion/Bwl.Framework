using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class DatagridLogWriter : ILogWriter
    {
        private const int messagesLimit = 1024 * 8;

        private class ListItem
        {
            public string message;
            public string additional;
            public DateTime dateTime;
            public LogEventType @type;
            public string[] path;
            public string pathCombined;
            public string className;
        }

        private List<ListItem> newMessages = new List<ListItem>();
        private List<ListItem> oldMessages = new List<ListItem>();
        private bool working = true;
        public string rootName = "Root";
        public DatagridLogWriter()
        {
            InitializeComponent();
        }

        private string GetTypeName(LogEventType @type)
        {
            if (type == LogEventType.information)
                return "ИНФ";
            if (type == LogEventType.errors)
                return "ОШБ";
            if (type == LogEventType.message)
                return "СБЩ";
            if (type == LogEventType.warning)
                return "ПРД";
            if (type == LogEventType.debug)
                return "ОТЛ";
            return "";
        }

        private void tWrite_Tick(object sender, EventArgs e)
        {
            tWrite.Stop();
            lock (_newMessagesListLock)
            {
                if (newMessages.Count > 0)
                {

                    lock (_gridSync)
                    {
                        while (newMessages.Count > 0)
                        {
                            if (newMessages[0] is not null)
                            {
                                oldMessages.Add(newMessages[0]);
                                if (Filter(newMessages[0]))
                                    ShowMessage(newMessages[0]);
                            }
                            newMessages.RemoveAt(0);
                        }

                        while (oldMessages.Count > messagesLimit)
                            oldMessages.RemoveAt(0);

                        while (grid.Rows.Count > messagesLimit)
                            grid.Rows.RemoveAt(0);
                        if (cbAutoScroll.Checked)
                        {
                            if (grid.Rows.Count > 0)
                            {
                                grid.Rows.SharedRow(grid.Rows.Count - 1).Cells[0].Selected = true;
                                grid.Rows.SharedRow(grid.Rows.Count - 1).Cells[0].Selected = false;
                            }
                        }
                    }
                }
            }
            tWrite.Start();
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
            string[] parts = additional.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                string[] subparts = part.Split('=');
                if (subparts.Length == 2)
                {
                    if (subparts[0] == "ClassName")
                        return subparts[1];
                }
            }
            return "";
        }

        private object _newMessagesListLock = new object();
        public void WriteEvent(DateTime datetime, string[] path, LogEventType @type, string text, params object[] @params)
        {
            lock (_newMessagesListLock)
            {
                if (working)
                {
                    var item = new ListItem();
                    item.dateTime = datetime;
                    item.path = path;
                    item.message = text;
                    item.additional = "";
                    if (@params is not null && @params.Length > 0)
                        item.additional = @params[0].ToString();
                    item.type = type;
                    string pathString = "";
                    if (item.path.GetUpperBound(0) >= 0)
                    {
                        pathString = item.path[0];
                        for (int i = 1, loopTo = item.path.Length - 1; i <= loopTo; i++)
                            pathString = item.path[i] + "." + pathString;
                    }
                    else
                    {
                        pathString = rootName;
                    }


                    item.pathCombined = pathString;
                    newMessages.Add(item);
                }
            }
        }

        private System.Drawing.Color _GetRowColor_lastColor = default;
        private System.Drawing.Color GetRowColor(LogEventType @type)
        {
            var newColor = System.Drawing.Color.White;
            if (type == LogEventType.debug)
                newColor = System.Drawing.Color.FromArgb(200, 200, 200);
            if (type == LogEventType.errors)
                newColor = System.Drawing.Color.FromArgb(255, 180, 180);
            if (type == LogEventType.message)
                newColor = System.Drawing.Color.FromArgb(240, 240, 255);
            if (type == LogEventType.warning)
                newColor = System.Drawing.Color.FromArgb(255, 255, 220);
            if (_GetRowColor_lastColor == newColor)
            {
                _GetRowColor_lastColor = System.Drawing.Color.Black;
                newColor = System.Drawing.Color.FromArgb((int)Math.Round(newColor.R * 0.9d), (int)Math.Round(newColor.G * 0.9d), (int)Math.Round(newColor.B * 0.9d));
            }
            _GetRowColor_lastColor = newColor;
            return newColor;
        }
        private bool Filter(ListItem message)
        {
            if (cbCats.Items.Count > 0)
            {
                bool pass = false;
                foreach (string catOrig in cbCats.CheckedItems)
                {
                    if (catOrig.StartsWith("#"))
                    {
                        var cat = catOrig.Substring(1);
                        if ((cat ?? "") == (message.pathCombined ?? ""))
                        {
                            pass = true;
                            break;
                        }
                    }
                    if (catOrig.StartsWith("*"))
                    {
                        var cat = catOrig.Substring(1);
                        string className = GetClassName(message.additional);
                        if ((className ?? "") == (cat ?? ""))
                        {
                            pass = true;
                            break;
                        }
                    }
                }

                if (pass == false)
                    return false;
            }

            if (!cbDebug.Checked && message.type == LogEventType.debug)
                return false;
            if (!cbInformation.Checked && message.type == LogEventType.information)
                return false;
            if (!cbErrors.Checked && message.type == LogEventType.errors)
                return false;
            if (!cbWarnings.Checked && message.type == LogEventType.warning)
                return false;
            if (!cbMessages.Checked && message.type == LogEventType.message)
                return false;

            bool textFilter;

            if (tbFilter.Text.Length == 0 | cbFilter.Checked == false)
            {
                textFilter = true;
            }
            else
            {
                textFilter = true;
                string[] parts = tbFilter.Text.Split(',');
                foreach (var partOrig in parts)
                {
                    bool found = false;
                    var part = partOrig.Trim();
                    if (string.IsNullOrEmpty(part))
                        found = true;
                    if (message.message.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                        found = true;
                    if (message.pathCombined.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                        found = true;
                    if (message.type.ToString().IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                        found = true;
                    if (!found)
                        textFilter = false;
                }
            }

            return textFilter;
        }
        private void ShowMessage(ListItem message)
        {
            string pathString = message.pathCombined;
            string dateString = message.dateTime.ToShortDateString();
            string timeString = message.dateTime.ToLongTimeString();
            string typeString = GetTypeName(message.type);

            grid.RowTemplate.DefaultCellStyle.BackColor = GetRowColor(message.type);
            // grid.Rows.SharedRow (grid.Rows.Count-1).
            grid.Rows.Add(dateString, timeString, typeString, pathString, message.message, message.additional);
        }

        public void Clear()
        {
            lock (_gridSync)
            {
                oldMessages.Clear();
                grid.Rows.Clear();
            }
        }

        private void bClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clear();
        }

        private object _gridSync = new object();

        private void RedrawItems()
        {
            lock (_gridSync)
            {
                grid.Rows.Clear();
                foreach (var msg in oldMessages)
                {
                    if (Filter(msg))
                        ShowMessage(msg);
                }
                // If cbAutoScroll.Checked Then
                if (grid.Rows.Count > 0)
                {
                    grid.Rows.SharedRow(grid.Rows.Count - 1).Cells[0].Selected = true;
                    grid.Rows.SharedRow(grid.Rows.Count - 1).Cells[0].Selected = false;
                }
                // End If
            }
        }

        private void ViewSettingsChanged(object sender, EventArgs e)
        {
            RedrawItems();
        }

        public bool ShowDebug
        {
            get
            {
                return cbDebug.Checked;
            }
            set
            {
                cbDebug.Checked = value;
            }
        }
        public bool ShowInformation
        {
            get
            {
                return cbInformation.Checked;
            }
            set
            {
                cbInformation.Checked = value;
            }
        }
        public bool ShowMessages
        {
            get
            {
                return cbMessages.Checked;
            }
            set
            {
                cbMessages.Checked = value;
            }
        }
        public bool ShowErrors
        {
            get
            {
                return cbErrors.Checked;
            }
            set
            {
                cbErrors.Checked = value;
            }
        }
        public bool ShowWarnings
        {
            get
            {
                return cbWarnings.Checked;
            }
            set
            {
                cbWarnings.Checked = value;
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
                if (string.IsNullOrEmpty(tbFilter.Text))
                {
                    tbFilter.Visible = false;
                    cbFilter.Checked = false;
                }
                else
                {
                    tbFilter.Visible = true;
                    cbFilter.Checked = true;
                }
            }
        }
        private void cbFilter_CheckedChanged(object sender, EventArgs e)
        {
            tbFilter.Visible = cbFilter.Checked;
            // If tbFilter.Visible = False Then
            // tbFilter.Text = ""
            // End If
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            tFilterApply.Stop();
            tFilterApply.Start();
        }

        private void tFilterApply_Tick(object sender, EventArgs e)
        {
            tFilterApply.Stop();
            RedrawItems();
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // вызов окна
            if (e.RowIndex >= 0 && e.RowIndex <= grid.RowCount)
            {
                var LogInfo = new FormLogInfo();
                var infoList = new List<string>();
                for (int i = 0, loopTo = grid.ColumnCount - 1; i <= loopTo; i++)
                    infoList.Add(grid[i, e.RowIndex].Value?.ToString());
                LogInfo.LogInfoText = infoList;
                LogInfo.Show();
                // LogInfo.Dispose()
            }
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void cbExtended_CheckedChanged(object sender, EventArgs e)
        {
            grid.Columns[2].Visible = cbExtended.Checked;
            grid.Columns[3].Visible = cbExtended.Checked;
            grid.Columns[5].Visible = cbExtended.Checked;
        }

        public bool ExtendedView
        {
            get
            {
                return cbExtended.Checked;
            }
            set
            {
                cbExtended.Checked = value;
            }
        }

        private void bRefreshPlaces_Click(object sender, EventArgs e)
        {
            var places = new List<string>();
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
            string[] arr = places.ToArray();
            Array.Sort(arr);
            cbCats.Items.Clear();
            foreach (var place in arr)
                cbCats.Items.Add("#" + place.ToString());
            RedrawItems();
        }

        private void cbCats_SelectedIndexChanged()
        {
            RedrawItems();
        }

        private void bRefreshNone_Click(object sender, EventArgs e)
        {
            cbCats.Items.Clear();
            RedrawItems();
        }

        private void bRefreshClasses_Click(object sender, EventArgs e)
        {
            var classes = new List<string>();
            lock (_newMessagesListLock)
            {
                foreach (var msg in newMessages.ToArray())
                {
                    string className = GetClassName(msg.additional);
                    if (classes.Contains(className) == false)
                        classes.Add(className);
                }
                foreach (var msg in oldMessages.ToArray())
                {
                    string className = GetClassName(msg.additional);
                    if (classes.Contains(className) == false)
                        classes.Add(className);
                }
            }
            string[] arr = classes.ToArray();
            Array.Sort(arr);
            cbCats.Items.Clear();
            foreach (var place in arr)
                cbCats.Items.Add("*" + place.ToString());
            RedrawItems();
        }

        private void cbCats_SelectedIndexChanged(object sender, MouseEventArgs e) => cbCats_SelectedIndexChanged();
    }
}