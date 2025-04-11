using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Bwl.Framework.Avalonia;
using Bwl.Framework.Avalonia.AdditionalControls.Classes;
using System;
using System.Collections.Generic;
using System.Threading;
using AvaloniaUI = Bwl.Framework.Avalonia.AvaloniaUI;

namespace Bwl.Framework.Test.Avalonia
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // Example: this way you can run multiple windows at the same time
            Task.Run(async () =>
            {
                var res = await InputBox.ShowAsync("This is a test input box", "Input box", "Default response");
                await MessageBox.ShowAsync(res, "Message box", MessageBox.MessageBoxButtons.OK, MessageBox.MessageBoxIcon.Information);
                var forms = new List<Window>();
                Dispatcher.UIThread.Invoke(() => forms.AddRange([new TestWindow(), new TestAutoUI().AppForm])); // All operations related to Avalonia should be done in the UI thread, including creation of windows
                foreach (var form in forms)
                {
                    // List above and this event are used only as an example on how to stop an app from within the code if you don't use MainWindow
                    form.Closed += (object? sender, EventArgs e) =>
                    {
                        forms.Remove(form);
                        if (forms.Count == 0) AvaloniaUI.StopApplication(); // Headless Avalonia requires call to Stop in order to stop working - it will close all opened windows too
                    };
                    Dispatcher.UIThread.Post(() => form.Show());
                }
            });

            // If you use Console, you can use it to stop the app too

            //// Example: you can actually use classic "Press any key to continue..." to stop the app - you'll need a thread though...
            //new Thread(new ThreadStart(() =>
            //{
            //    Console.WriteLine("Press any key to stop the app...");
            //    Console.ReadKey();
            //    Avalonia.AvaloniaUI.StopApplication();
            //}))
            //{ IsBackground = true }.Start();

            //// Example: this way you can run a background thread that will output something (in this case - time) to the console
            //new Thread(new ThreadStart(() =>
            //{
            //    do
            //    {
            //        Console.WriteLine($"{DateTime.Now}");
            //        Thread.Sleep(1000);
            //    } while (true);
            //}))
            //{ IsBackground = true }.Start();

            Thread.Sleep(5000); // To demonstrate tha forms will be created only after the UI thread is initialized

            // Use StartHeadless if you don't want to have a window at the startup but may need to show any windows later (or create them in the background, like in the example above)
            AvaloniaUI.StartHeadless();

            // ... or call StartMainWindow to run a single window, no call to Stop needed - app will stop when window is closed
            AvaloniaUI.StartMainWindow(new TestWindow());

            // Just remember - only one at a time, StartHeadless would not work if you called StartMainWindow (unless you stopped the execution, that is) and vice versa and it WILL throw an exception.
            // Additional windows WILL run just fine, though

            AvaloniaUI.StopApplication();
        }

        // Required for visual designer
        // Interesting fact - if you want to use Avalonia designer with libraries just add Program.cs with this method to the project, you don't need to call it anywhere, just have it in the project
        public static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.GetAvaloniaAppBuilder();
    }
}
