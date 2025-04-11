# Bwl.Framework.Avalonia

**Bwl.Framework.Avalonia** is the cross-platform UI component of Bwl.Framework built atop [Avalonia](https://avaloniaui.net/). It provides a consistent API for building rich desktop interfaces across Windows, Linux, and macOS. This library is designed to work hand-in-hand with the core Bwl.Framework library, offering UI functionality where Windows-specific implementations are not desired.

## Features

- **Cross-Platform UI:** Build and run your applications on multiple platforms using a single code base.
- **Avalonia Integration:** Leverages Avalonia's powerful controls, styling, and data-binding features.
- **AutoUI & Settings:** Easily integrate with the shared configuration and logging of Bwl.Framework.
- **Launcher and Shutdown:** Provides helper methods to initialize, run headless, start a main window, and stop the application gracefully.
- **FormBase & FormAppBase Support:** Simplifies creation of AutoUI applications with built-in support for Avalonia-based forms.
- **Visual Designer Support:** Includes a `BuildAvaloniaApp` function required by other libraries and the Avalonia visual designer.

## Usage

### Initialization

When you add this library to your project it will automatically initialize on startup of your application. Be sure that you don't also reference `Bwl.Framework.Windows` as this will cause a conflict.

Initialization sets up the underlying Avalonia application, loads default theme colors, and initializes required factories (for example, the settings form factory).

### Running Windows

You have two options for running your UI:

1. **Headless Mode:**  
  Use `StartHeadless()` to launch Avalonia without an initial window. This mode blocks the calling thread until you call `StopApplication()` and is useful when you want to create and show windows later on demand.
  
2. **Main Window Mode:**  
  Use `StartMainWindow(Window mainWindow)` to launch Avalonia with a single main window. Once the window is closed, the application will terminate.
  

Below is an example that demonstrates running multiple windows:

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Bwl.Framework.Avalonia;
using Bwl.Framework.Avalonia.AdditionalControls.Classes;
class AvaloniaExample
{
    public static void Main(string[] args)
    {
        // Example: Running multiple windows on the UI thread.
        new Thread(new ThreadStart(async () =>
        {
            // Show an InputBox and a MessageBox asynchronously.
            var res = await InputBox.Show("This is a test input box", "Input box", "Default response");
            await MessageBox.Show(res, "Message box", MessageBox.MessageBoxButtons.OK, MessageBox.MessageBoxIcon.Information);

            // Create multiple windows.
            var forms = new List<Window>();
            Dispatcher.UIThread.Invoke(() =>
            {
                forms.AddRange(new List<Window> { new TestWindow(), new TestAutoUI().AppForm });
            });

            // Show windows and register for close events.
            foreach (var form in forms)
            {
                form.Closed += (sender, e) =>
                {
                    forms.Remove(form);
                    if (forms.Count == 0)
                        AvaloniaUI.StopApplication();
                };

                Dispatcher.UIThread.Post(() => form.Show());
            }
        })).Start();

        // Allow time for the UI thread to initialize forms.
        Thread.Sleep(5000);

        // Choose one of the following modes to run your application:

        // To run without an initial window, start headless.
        AvaloniaUI.StartHeadless();

        // OR to run with a single main window:
        // AvaloniaUI.StartMainWindow(new TestWindow());

        // Once your windows are closed or you call StopApplication(), the app will terminate.
        AvaloniaUI.StopApplication();
    }
}
```

### Theme and Settings Integration

**Bwl.Framework.Avalonia** automatically sets default theme colors and registers the settings dialog factory during initialization. To customize themes, call:

```csharp
// Customize the theme colors. 
var lightTheme = new ColorPaletteResources { Accent = Color.Parse("#31587D"), RegionColor = Colors.White, ErrorText = Colors.Red };
var darkTheme = new ColorPaletteResources { Accent = Color.Parse("#1E364D"), RegionColor = Colors.Black, ErrorText = Colors.Yellow };

// Set the theme colors for the default variant and density style. 
AvaloniaUI.SetThemeColors(ThemeVariant.Default, DensityStyle.Compact, lightTheme, darkTheme);
```

### Creating FormBase Applications

To create a FormBase application with Avalonia, you can use the provided **FormBase** or **FormAppBase** (**FormBase** with **AppBase**) classes. For example:

1. **Setting Up Your Window:**
  
  - Create a new Avalonia Window.
  - Add the namespace for Bwl.Framework.Avalonia (e.g., `xmlns:local="clr-namespace:Bwl.Framework.Avalonia;assembly=Bwl.Framework.Avalonia"`) to your Window tag.
  - Inside the Window tag, add an instance of `FormAppBase`:

```xml
 <Window xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
         xmlns:local="clr-namespace:Bwl.Framework.Avalonia;assembly=Bwl.Framework.Avalonia"
         x:Class="MyApp.MainWindow">
     <local:FormAppBase x:Name="FormAppBase">
         <local:FormAppBase.FormContent>
             <!-- Define your UI elements here -->
         </local:FormAppBase.FormContent>
     </local:FormAppBase>
 </Window>
```

2. **Initializing the Form:**
  
  In the code-behind (e.g., `MainWindow.axaml.cs`), initialize your form by calling:
  

```csharp
public MainWindow()
{
    InitializeComponent();
    FormAppBase.Init();
}
```

This setup provides a declarative way to define UI elements and leverage Bwl.Framework’s AutoUI capabilities.

### Visual Designer Support

For a better design-time experience in Avalonia, **Bwl.Framework.Avalonia** includes a helper function for building the Avalonia app:

```csharp
public static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.AvaloniaAppBuilder;
```

Include a `Program.cs` with this function in your project to enable visual designer support with Avalonia.

### Stopping the Application

To completely stop the Avalonia application, call:

```csharp
AvaloniaUI.StopApplication();
```

This method closes all open windows and terminates the application.

## Additional Information

- **Threading:**  
  All operations that involve window creation, display, or manipulation must be performed on Avalonia's UI thread. Use `Dispatcher.UIThread.Invoke` or `Dispatcher.UIThread.Post` as needed.
  
- **Integration with Bwl.Framework:**  
  **Bwl.Framework.Avalonia** is designed to work seamlessly with the core Bwl.Framework libraries, integrating configuration, logging, and UI functionality.