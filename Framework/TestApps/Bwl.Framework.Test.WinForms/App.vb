Imports System.Runtime.Versioning
Imports System.Threading
Imports System.Windows.Forms
Imports Bwl.Framework.Windows

<SupportedOSPlatform("windows")>
Public Module App

    Sub Main()

        ' An example of running multiple windows at the same time
        Task.Run(Sub()
                     WinFormsUI.UIThreadInvoke(
                     Sub()
                         Dim res = InputBox.Show("This is a test input box", "Input box", "Default response")
                         MessageBox.Show(res, "Message box", MessageBoxButtons.OK, MessageBoxIcon.Information)
                     End Sub)
                     Dim forms = New List(Of Form)()
                     WinFormsUI.UIThreadInvoke(Sub() forms.AddRange({New TestFormAppBase(), New TestAutoUI().AppForm})) ' Operations with forms should be called on UI thread
                     For Each form In forms
                         AddHandler form.FormClosed,
                         Sub()
                             forms.Remove(form)
                             If forms.Count = 0 Then
                                 WinFormsUI.StopApplication()
                             End If
                         End Sub
                         WinFormsUI.UIThreadInvoke(Sub()
                                                       form.Show()
                                                       form.BringToFront()
                                                   End Sub)
                     Next
                 End Sub)

        ' If you use Console, you can use it to stop the app
        'Dim t2 = New Thread(
        '    New ThreadStart(
        '    Sub()
        '        Console.WriteLine("Press any key to exit")
        '        Console.ReadKey()
        '        WinFormsUI.StopApplication()
        '    End Sub)) With {.IsBackground = True}
        't2.Start()

        ' Example: this way you can run a background thread that will output something (in this case - time) to the console
        'Dim t3 = New Thread(
        '    New ThreadStart(
        '    Sub()
        '        While True
        '            Console.WriteLine(DateTime.Now)
        '            Thread.Sleep(1000)
        '        End While
        '    End Sub)) With {.IsBackground = True}
        't3.Start()

        Thread.Sleep(5000) ' To demonstrate tha forms will be created only after the UI thread is initialized

        ' Use StartHeadless if you don't want to have a window at the startup but may need to show any windows later (or create them in the background, like in the example above)
        WinFormsUI.StartHeadless()

        '... Or call StartMainForm to run a single window, no call to Stop needed - app will stop when window Is closed
        WinFormsUI.StartMainForm(New TestFormAppBase())

        ' Just remember - only one at a time, StartHeadless would Not work if you called StartMainForm (unless you stopped the execution, that Is) And vice versa And it WILL throw an exception.
        ' Additional windows WILL run just fine, though

    End Sub
End Module
