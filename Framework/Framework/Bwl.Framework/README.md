# Bwl.Framework

## Description

Bwl.Framework allows you to easily add configuration and logging to your project, as well as to use a set of common classes and methods.

Among additional features - CryptoTools with AES support; JSON serialization; and more.

Base Bwl.Framework library is a .NET Standard 2.0 library. To have access to Windows-specific features, use Bwl.Framework.Windows library. To use cross-platform GUI and features - use Bwl.Framework.Avalonia library.

## Usage

### AppBase

The main component that this library gives you access to is AppBase. AppBase automatically creates logger and settings storage that you can use in your application to easily manage application settings and write events to the log. Settings that you create are saved into settings file in **conf** directory, logs are saved in files in **logs** directory.

Here is an example of creating AppBase and getting access to RootLogger and RootStorage:

```csharp
var appBase = new AppBase();
var logger = appBase.RootLogger;
var settingsStorage = appBase.RootStorage;
```

### SetSettingsFormFactory

In SettignsStorage there are two functions that allow you to call settings form. These functions are used for applications with graphical interface and, used in a console application, will throw an error. If you use Bwl.Framework.Avalonia or Bwl.Framework.Windows correctly everything is already done, but in case you would want to create an application with the different UI framework you'll need to set up a factory for a settings form. Let's look at the example for Avalonia:

```csharp
// Example of SettingsDialogFactory for Avalonia
public class SettingsDialogFactory : ISettingsFormFactory
{
    public ISettingsForm CreateSettingsForm()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            return new SettingsDialog();
        }
        else
        {
            return Dispatcher.UIThread.Invoke(() => new SettingsDialog());
        }
    }

    private static SettingsDialogFactory _instance;
    public static SettingsDialogFactory Create()
    {
        if (_instance == null)
        {
            _instance = new SettingsDialogFactory();
        }
        return _instance;
    }
}
```

In the example above the class **SettingsDialogFactory** implements an interface **ISettingsFormFactory**. This factory is a singleton, meaning that we call Create to get it whenever we want in code. It creates and returns a Window named **SettingsDialog** - that window, in fact, is an implementation of an interface **ISettingsForm**.

To set the factory in SettingsStorage you need to execute this line of code:

```csharp
SettingsStorageBase.SetSettingsFormFactory(SettingsDialogFactory.Create());
```

In the example above the SetSettingsFormFactory function is static, so you don't need to create SettingsStorageBase. You need to execute this line of code once and after that it will work no matter what SettingsStorage you use, whether it's ClonedSettingsStorage or SettingsStorageRoot not connected to any AppBase.

### UIWindowFactories

Sometimes there's a case when you need to open a window from the library. To make sure the library can stay agnostic of UI framework you use Bwl.Framework has UIWindowFactories class. It allows you to register a window factory from inside the library that supports UI and then call it from the library.

Let's look at example from the same repository as Bwl.Framework - Bwl.Network.ClientServer. It contains two classes, CmdlineServer and CmdlineClient, that you can use to access the command line on a remote machine. One of the options CmdlineClient offers is an UI window. But Bwl.Network.ClientServer is agnostic of UI framework and UI functionality is available in Bwl.Network.ClientServer.Windows (WinForms, Windows only) or Bwl.Network.ClientServer.Avalonia (Avalonia, cross-platform). It's already done in these libraries but that's how you can do it:

1. In root directory of your project create a file containing a class - for example **UIElementRegistration.cs**
  
2. Add the next code

```csharp
  public static class UIElementRegistration
  {
      [ModuleInitializer]
      public static void Initialize()
      {
          UIWindowFactories.RegisterWindowFactory("CmdlineUi", new Func<IUIWindow>(() => new CmdlineUi()));
      }
  }
```
  
In the example above **[ModuleInitializer]** will automatically execute this code when library is loaded. The function **Initialize()** executes code that registers window factory named **CmdlineUI**. which, in turn, returns a new Window called **CmdlineUI**, which, in turn, implements an interface **IUIWindow**.
  
3. After the code above is executed you might create a new window by calling UIWindowFactories:
  
```csharp
  public IUIWindow CreateCmdForm()
  {
      if (!UIWindowFactories.GetAvailableFactories().Contains("CmdlineUi")) return null;
      return UIWindowFactories.CreateWindow("CmdlineUi")
  }
```
  
In the example above **GetAvailableFactories()** returns the list of all factories registered in **UIWindowFactories**. You can get a new window by calling a function **CreateWindow** and specifying the name of a window you want to get. After that you can work with window as usual.

### AsyncResetEvent

Sometimes when you work with task you need to be able to stop it and wait until it stops. Using CancellationToken will result in an exception and you won't be able to wait until the task is completed. For that you can use `AsyncResetEvent`. It is a class that implements IAsyncDisposable and allows you to wait for an event to be set or reset. It is similar to `AutoResetEvent`/`ManualResetEvent`, but it is asynchronous and can be used in async/await code. `WaitAsync` method is similar to `WaitOne` method in `AutoResetEvent`/`ManualResetEvent`, but it is asynchronous and can be used in async/await code to pause task without blocking the context (similar to `Task.Delay`).