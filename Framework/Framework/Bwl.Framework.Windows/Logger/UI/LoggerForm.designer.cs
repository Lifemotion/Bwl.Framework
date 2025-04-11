using System.Diagnostics;

namespace Bwl.Framework.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class LoggerForm : System.Windows.Forms.Form
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggerForm));
            _datagridLogWriter1 = new DatagridLogWriter();
            SuspendLayout();
            // 
            // _datagridLogWriter1
            // 
            _datagridLogWriter1.Dock = System.Windows.Forms.DockStyle.Fill;
            _datagridLogWriter1.FilterText = "";
            _datagridLogWriter1.Location = new System.Drawing.Point(0, 0);
            _datagridLogWriter1.LogEnabled = true;
            _datagridLogWriter1.Margin = new System.Windows.Forms.Padding(0);
            _datagridLogWriter1.Name = "_datagridLogWriter1";
            _datagridLogWriter1.ShowDebug = false;
            _datagridLogWriter1.ShowErrors = true;
            _datagridLogWriter1.ShowInformation = true;
            _datagridLogWriter1.ShowMessages = true;
            _datagridLogWriter1.ShowWarnings = true;
            _datagridLogWriter1.Size = new System.Drawing.Size(574, 272);
            _datagridLogWriter1.TabIndex = 2;
            // 
            // LoggerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(574, 272);
            Controls.Add(_datagridLogWriter1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "LoggerForm";
            Text = "Лог";
            ResumeLayout(false);

        }
        internal DatagridLogWriter _datagridLogWriter1;
    }
}