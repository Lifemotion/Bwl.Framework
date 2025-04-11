using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class FormBase : Form
    {

        // Форма переопределяет dispose для очистки списка компонентов.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Является обязательной для конструктора форм Windows Forms
        private System.ComponentModel.IContainer components;

        // Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
        // Для ее изменения используйте конструктор форм Windows Form.  
        // Не изменяйте ее в редакторе исходного кода.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBase));
            _MenuStrip1 = new MenuStrip();
            _УправлениеToolStripMenuItem = new ToolStripMenuItem();
            _openAppDirMenuItem = new ToolStripMenuItem();
            _openAppDirMenuItem.Click += new EventHandler(openAppDirMenuItem_Click);
            _settingsMenuItem = new ToolStripMenuItem();
            _settingsMenuItem.Click += new EventHandler(settingsMenuItem_Click);
            _ЛогToolStripMenuItem = new ToolStripMenuItem();
            _ЛогToolStripMenuItem.Click += new EventHandler(ЛогToolStripMenuItem_Click);
            _exitMenuItem = new ToolStripMenuItem();
            _exitMenuItem.Click += new EventHandler(exitMenuItem_Click);
            _logWriter = new DatagridLogWriter();
            _MenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStrip1
            // 
            resources.ApplyResources(_MenuStrip1, "MenuStrip1");
            _MenuStrip1.Items.AddRange(new ToolStripItem[] { _УправлениеToolStripMenuItem });
            _MenuStrip1.Name = "_MenuStrip1";
            // 
            // УправлениеToolStripMenuItem
            // 
            resources.ApplyResources(_УправлениеToolStripMenuItem, "УправлениеToolStripMenuItem");
            _УправлениеToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _openAppDirMenuItem, _settingsMenuItem, _ЛогToolStripMenuItem, _exitMenuItem });
            _УправлениеToolStripMenuItem.Name = "_УправлениеToolStripMenuItem";
            // 
            // openAppDirMenuItem
            // 
            resources.ApplyResources(_openAppDirMenuItem, "openAppDirMenuItem");
            _openAppDirMenuItem.Name = "_openAppDirMenuItem";
            // 
            // settingsMenuItem
            // 
            resources.ApplyResources(_settingsMenuItem, "settingsMenuItem");
            _settingsMenuItem.Name = "_settingsMenuItem";
            // 
            // ЛогToolStripMenuItem
            // 
            resources.ApplyResources(_ЛогToolStripMenuItem, "ЛогToolStripMenuItem");
            _ЛогToolStripMenuItem.Name = "_ЛогToolStripMenuItem";
            // 
            // exitMenuItem
            // 
            resources.ApplyResources(_exitMenuItem, "exitMenuItem");
            _exitMenuItem.Name = "_exitMenuItem";
            // 
            // logWriter
            // 
            resources.ApplyResources(_logWriter, "logWriter");
            _logWriter.ExtendedView = true;
            _logWriter.FilterText = "";
            _logWriter.LogEnabled = true;
            _logWriter.Name = "_logWriter";
            _logWriter.ShowDebug = false;
            _logWriter.ShowErrors = true;
            _logWriter.ShowInformation = true;
            _logWriter.ShowMessages = true;
            _logWriter.ShowWarnings = true;
            // 
            // FormBase
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(_logWriter);
            Controls.Add(_MenuStrip1);
            MainMenuStrip = _MenuStrip1;
            Name = "FormBase";
            _MenuStrip1.ResumeLayout(false);
            _MenuStrip1.PerformLayout();
            Load += new EventHandler(FormAppBase_Load);
            ResumeLayout(false);
            PerformLayout();

        }
        private DatagridLogWriter _logWriter;

        protected virtual DatagridLogWriter logWriter
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _logWriter;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _logWriter = value;
            }
        }
        private ToolStripMenuItem _openAppDirMenuItem;

        internal virtual ToolStripMenuItem openAppDirMenuItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _openAppDirMenuItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_openAppDirMenuItem != null)
                {
                    _openAppDirMenuItem.Click -= openAppDirMenuItem_Click;
                }

                _openAppDirMenuItem = value;
                if (_openAppDirMenuItem != null)
                {
                    _openAppDirMenuItem.Click += openAppDirMenuItem_Click;
                }
            }
        }
        private ToolStripMenuItem _exitMenuItem;

        internal virtual ToolStripMenuItem exitMenuItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _exitMenuItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_exitMenuItem != null)
                {
                    _exitMenuItem.Click -= exitMenuItem_Click;
                }

                _exitMenuItem = value;
                if (_exitMenuItem != null)
                {
                    _exitMenuItem.Click += exitMenuItem_Click;
                }
            }
        }
        private ToolStripMenuItem _settingsMenuItem;

        internal virtual ToolStripMenuItem settingsMenuItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _settingsMenuItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_settingsMenuItem != null)
                {
                    _settingsMenuItem.Click -= settingsMenuItem_Click;
                }

                _settingsMenuItem = value;
                if (_settingsMenuItem != null)
                {
                    _settingsMenuItem.Click += settingsMenuItem_Click;
                }
            }
        }
        private MenuStrip _MenuStrip1;

        protected virtual MenuStrip MenuStrip1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _MenuStrip1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _MenuStrip1 = value;
            }
        }
        private ToolStripMenuItem _УправлениеToolStripMenuItem;

        protected virtual ToolStripMenuItem УправлениеToolStripMenuItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _УправлениеToolStripMenuItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _УправлениеToolStripMenuItem = value;
            }
        }
        private ToolStripMenuItem _ЛогToolStripMenuItem;

        internal virtual ToolStripMenuItem ЛогToolStripMenuItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ЛогToolStripMenuItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ЛогToolStripMenuItem != null)
                {
                    _ЛогToolStripMenuItem.Click -= ЛогToolStripMenuItem_Click;
                }

                _ЛогToolStripMenuItem = value;
                if (_ЛогToolStripMenuItem != null)
                {
                    _ЛогToolStripMenuItem.Click += ЛогToolStripMenuItem_Click;
                }
            }
        }
    }
}