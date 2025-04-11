# Bwl.Framework.Windows

**Bwl.Framework.Windows** is the Windows‐specific UI component of the Bwl.Framework library. It provides helper classes and methods for classic Windows Forms applications. This library simplifies launching and managing WinForms-based user interfaces, including support for headless mode (to obtain a UI thread without showing an initial form) and single‐window mode.

## Features

- **WinForms Integration:** Automatically enables visual styles and initializes required factories.
- **Headless and Main Form Modes:** Choose between a headless start (to initialize the UI thread for later use) or launching a single main window.
- **UI Thread Helpers:** Provides methods to invoke actions on the UI thread.
- **Settings Form Integration:** Registers settings form factories compatible with Windows Forms.

## Usage

### Initialization

When you add this library to your project it will automatically initialize on startup of your application. Be sure that you don't also reference `Bwl.Framework.Avalonia` as this will cause a conflict.

Initialization automatically sets up the settings form factory and enables visual styles for your application.

### Running Windows

There are two ways to start your Windows Forms application:

1. **Headless Mode:**  
   Use `StartHeadless()` to start the UI thread without showing an initial window. This is useful when you want to create windows later on demand.

2. **Main Form Mode:**  
   Use `StartMainForm(Form form)` to start the application with a single main window. The application terminates when this window is closed.

### Example: Launching Multiple Forms

Below is an example in C#. The example demonstrates running multiple windows concurrently and closing the application once all windows are closed.

``` csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Bwl.Framework.Windows;

class WinFormsExample
{
    static void Main()
    { 
        // Start a background thread to create and show multiple windows.
        Thread t1 = new Thread(() =>
        {
            // Show an input box and a message box
            string res = InputBox.Show("This is a test input box", "Input box", "Default response");
            MessageBox.Show(res, "Message box", MessageBoxButtons.OK, MessageBoxIcon.Information);

            List<Form> forms = new List<Form>();

            // Invoke on the UI thread to create forms (example forms: TestFormAppBase and TestAutoUI.AppForm)
            WinFormsUI.UIThreadInvoke(() =>
            {
                // These are example forms provided in the test suite
                forms.Add(new TestFormAppBase());
                forms.Add(new TestAutoUI().AppForm);
            });

            // Register event handlers to close the application when all forms have been closed.
            foreach (var form in forms)
            {
                form.FormClosed += (sender, e) =>
                {
                    forms.Remove(form);
                    if (forms.Count == 0)
                    {
                        WinFormsUI.StopApplication();
                    }
                };

                // Show each form on the UI thread.
                WinFormsUI.UIThreadInvoke(() => form.Show());
            }
        });
        t1.Start();

        // Wait some time to ensure the UI thread is initialized
        Thread.Sleep(5000);

        // Option 1: Start headless (no visible window) to later create windows on demand.
        WinFormsUI.StartHeadless();

        // Option 2: Alternatively, start the application with a main form.
        // WinFormsUI.StartMainForm(new TestFormAppBase());
    }
}
```

### Stopping the Application

To stop the application (for example, when running in headless mode), call:

``` csharp
WinFormsUI.StopApplication();
```

This method attempts to close all open forms.

## Additional Information

- **UI Thread Invocation:**  
  Ensure that any interactions with Windows Forms controls occur on the UI thread. Use `WinFormsUI.UIThreadInvoke(Action action)` to safely execute code on the UI thread.

- **Settings Integration:**  
  The settings form factory is automatically registered during initialization using the `SettingsDialogFactory`.