using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public class WinFormsUI
    {
        /// <summary>
        /// Initalizes WinForms module initialization
        /// </summary>
        [ModuleInitializer]
        public static void InitializeBase()
        {
            // Sets the settings form factory
            SettingsStorageBase.SetSettingsFormFactory(SettingsDialogFactory.Create());
        }

        private static bool _isRunning = false;

        /// <summary>
        /// Returns true if the application is running (and was started via WinFormsUI!) and there's at least one form.
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                // Check if any forms are open or if headless mode is running
                return _isRunning && Application.OpenForms.Count > 0;
            }
        }

        /// <summary>
        /// Initializes the WinForms UI.
        /// </summary>
        static WinFormsUI()
        {
            Application.EnableVisualStyles();
        }

        /// <summary>
        /// Starts the main form.
        /// </summary>
        /// <param name="mainForm"></param>
        public static void StartMainForm(IUIWindow mainForm)
        {
            StartMainForm((Form)mainForm);
        }

        /// <summary>
        /// Starts the main form.
        /// </summary>
        /// <param name="form"></param>
        public static void StartMainForm(Form form)
        {
            if (IsRunning) return;
            _isRunning = true;
            Application.Run(form);
            _isRunning = false;
        }

        /// <summary>
        /// Hides the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HideForm(object sender, EventArgs e)
        {
            // Hide the dummy form
            ((Form)sender).Hide();
        }

        /// <summary>
        /// Starts the headless application.
        /// </summary>
        public static void StartHeadless()
        {
            if (IsRunning) return;
            var dummyForm = CreateDummyForm();
            dummyForm.Shown += HideForm; // Hide it immediately
            _isRunning = true;
            Application.Run(CreateDummyForm());
            _isRunning = false;
            dummyForm.Shown -= HideForm; // Remove the handler to remove form from memory
        }

        /// <summary>
        /// Creates a dummy form for headless mode so we can get access to UI thread
        /// </summary>
        /// <returns></returns>
        private static Form CreateDummyForm()
        {
            return new Form()
            {
                Visible = false,
                ShowInTaskbar = false,
                Opacity = 0d,
                FormBorderStyle = FormBorderStyle.None,
                Size = new Size(0, 0)
            };
        }

        /// <summary>
        /// Invokes an action on the UI thread.
        /// </summary>
        /// <param name="action"></param>
        public static void UIThreadInvoke(Action action)
        {
            while (!IsRunning) Thread.Sleep(100);
            if (Application.OpenForms.Count > 0)
            {
                Application.OpenForms[0]!.Invoke(action);
            }
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void StopApplication()
        {

            try
            {
                if (Application.OpenForms.Count > 0)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        try
                        {
                            form.Close();
                        }
                        catch (Exception ex)
                        {
                            // Do nothing
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Do nothing
            }

        }

    }
}