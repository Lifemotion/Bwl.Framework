# Bwl.Framework

[![Build & Test](https://github.com/Lifemotion/Bwl.Framework/actions/workflows/build.yml/badge.svg)](https://github.com/Lifemotion/Bwl.Framework/actions/workflows/build.yml)
[![NuGet Bwl.Framework](https://img.shields.io/nuget/v/Bwl.Framework?label=Bwl.Framework&logo=nuget)](https://www.nuget.org/packages/Bwl.Framework)
[![NuGet Bwl.Framework.WinForms](https://img.shields.io/nuget/v/Bwl.Framework.WinForms?label=Bwl.Framework.WinForms&logo=nuget)](https://www.nuget.org/packages/Bwl.Framework.WinForms)
[![License](https://img.shields.io/github/license/Lifemotion/Bwl.Framework)](LICENSE)

A set of VB.NET / .NET libraries for building applications: hierarchical settings with auto-save, structured logging, base classes for console and GUI apps, and an AutoUI system for automatic interface generation.

## Packages

| Package | Targets | Description |
|---------|---------|-------------|
| [`Bwl.Framework`](https://www.nuget.org/packages/Bwl.Framework) | `netstandard2.0` `net8.0` | Cross-platform core: Settings, Logger, AutoUI interfaces, ConsoleAppBase |
| [`Bwl.Framework.WinForms`](https://www.nuget.org/packages/Bwl.Framework.WinForms) | `net8.0-windows` `net48` | WinForms components: FormAppBase, SettingsDialog, DatagridLogWriter, AutoUI controls |

## Installation

```bash
# Core (cross-platform — Windows, Linux, macOS)
dotnet add package Bwl.Framework

# WinForms components (Windows only)
dotnet add package Bwl.Framework.WinForms
```

## Key Features

### Settings — Hierarchical Settings Storage

Settings are organized in a tree structure with typed values, auto-save to INI files, and user group-based access control.

```vb
' Create a root storage with auto-save to file
Dim root = New SettingsStorageRoot("app-settings.ini", "MyApp")

' Typed settings
Dim port = root.CreateIntegerSetting("Port", 8080, "Server port")
Dim host = root.CreateStringSetting("Host", "localhost", "Address")
Dim debug = root.CreateBooleanSetting("Debug", False, "Debug mode")

' Nested categories
Dim dbSettings = root.CreateChildStorage("Database")
Dim connStr = dbSettings.CreateStringSetting("ConnectionString", "")

' Settings restricted to specific user groups
Dim secret = root.CreatePasswordSetting("ApiKey", "API key",, {"admin"})

' Save / reload
root.SaveSettings()
root.ReloadSettings()
```

Supported types: `StringSetting`, `IntegerSetting`, `DoubleSetting`, `BooleanSetting`, `VariantSetting`, `PasswordSetting`.

### Logger — Hierarchical Logging

A logging system with parent-child logger hierarchy, pluggable writers, and typed events.

```vb
Dim logger = New Logger("Main")
Dim childLogger = logger.CreateChildLogger("Network")

logger.AddMessage("Application started")
childLogger.AddWarning("Connection timeout")
childLogger.AddError("Error: " & ex.Message)

' Attach writers
logger.AddLogWriter(New ConsoleLogWriter())
logger.AddLogWriter(New SimpleFileLogWriter("app.log"))

' WinForms: DatagridLogWriter — visual log in a DataGridView
```

### ConsoleAppBase — Console Application Scaffold

A console application with built-in settings, logging, and interactive configuration support.

```vb
Public Class MyApp
    Inherits ConsoleAppBase

    Sub New()
        MyBase.New()
        Dim port = RootStorage.CreateIntegerSetting("Port", 8080)
        Logger.AddMessage("Starting on port " & port.Value)
    End Sub
End Class

Sub Main()
    Dim app = New MyApp()
    app.Start()    ' Process command-line arguments
    ' ... application logic ...
    app.Wait()
End Sub
```

### AutoUI — Automatic Interface Generation

A system for creating UI elements (buttons, textboxes, listboxes, images) programmatically, with both local (WinForms) and remote (network) display capabilities.

```vb
Dim ui = New AutoUI()
Dim btn = New AutoButton(ui, "startBtn")
btn.Info.Caption = "Start"
AddHandler btn.Click, Sub() logger.AddMessage("Start button clicked")

Dim status = New AutoTextbox(ui, "status")
status.Text = "Ready"
```

### Other Utilities

- **AutoSettings** — automatic binding of object properties to settings via reflection
- **SettingsStorageBackup** — scheduled automatic backup of settings
- **GlobalStates** — global registry of metrics and states with TTL
- **IniFile** — INI file reader/writer (net8.0)
- **CryptoTools** — string encryption/decryption (TripleDES)
- **Serializer** — JSON serialization via DataContractJsonSerializer
- **RunLimiter** — call rate limiting
- **MailSender** — SMTP email sending with configurable settings (WinForms)

## WinForms Components (Bwl.Framework.WinForms)

For applications with a graphical interface:

- **FormAppBase** — base application form with integrated settings and logging
- **SettingsDialog** — ready-made settings editor dialog (tree view, user group access support)
- **DatagridLogWriter** — visual log as a filterable DataGridView
- **LoggerForm** — standalone log viewer form
- **AutoUIDisplay / AutoUIForm** — controls for rendering AutoUI elements
- **Remote AutoUI** — controls for remote AutoUI display over the network

## Building from Source

```bash
# Build all projects
dotnet build Bwl.Framework/Bwl.Framework.vbproj
dotnet build Bwl.Framework.WinForms/Bwl.Framework.WinForms.vbproj

# Run tests
dotnet test Bwl.Framework.Tests/Bwl.Framework.Tests.vbproj

# Create NuGet packages
dotnet pack -c Release -o ./nupkgs
```

## License

[Apache 2.0](LICENSE)

## Author

Sasha Sovenko — [GitHub](https://github.com/Lifemotion)
