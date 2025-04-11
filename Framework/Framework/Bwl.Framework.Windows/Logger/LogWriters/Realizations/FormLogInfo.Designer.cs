using System;
using System.Diagnostics;

namespace Bwl.Framework.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class FormLogInfo : System.Windows.Forms.Form
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogInfo));
            _btn_CloseForm = new System.Windows.Forms.Button();
            _btn_CloseForm.Click += new EventHandler(_btn_CloseForm_Click);
            _btn_CopyText = new System.Windows.Forms.Button();
            _btn_CopyText.Click += new EventHandler(_btn_CopyText_Click);
            _rtb_LogInfo = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // _btn_CloseForm
            // 
            resources.ApplyResources(_btn_CloseForm, "_btn_CloseForm");
            _btn_CloseForm.Name = "_btn_CloseForm";
            _btn_CloseForm.UseVisualStyleBackColor = true;
            // 
            // _btn_CopyText
            // 
            resources.ApplyResources(_btn_CopyText, "_btn_CopyText");
            _btn_CopyText.Name = "_btn_CopyText";
            _btn_CopyText.UseVisualStyleBackColor = true;
            // 
            // _rtb_LogInfo
            // 
            resources.ApplyResources(_rtb_LogInfo, "_rtb_LogInfo");
            _rtb_LogInfo.Name = "_rtb_LogInfo";
            _rtb_LogInfo.ReadOnly = true;
            // 
            // FormLogInfo
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(_rtb_LogInfo);
            Controls.Add(_btn_CopyText);
            Controls.Add(_btn_CloseForm);
            Name = "FormLogInfo";
            ShowIcon = false;
            Shown += new EventHandler(FormLogInfo_Shown);
            ResumeLayout(false);
            PerformLayout();

        }
        private System.Windows.Forms.Button _btn_CloseForm;
        private System.Windows.Forms.Button _btn_CopyText;
        private System.Windows.Forms.RichTextBox _rtb_LogInfo;
    }
}