# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Bwl.Framework — общая VB.NET библиотека, реализующая механизмы хранения настроек, логирование, базовые классы приложений (консольных и WinForms). Автор: Игорь Кошелев.

## Language

Весь код написан на **VB.NET**. Опции компилятора: `OptionExplicit On`, `OptionStrict Off`, `OptionInfer On`, `OptionCompare Binary`.

## Build

```bash
# Сборка всех проектов (Debug + Release)
tools\vs-build-all.exe -debug -release -m BuildAll.sln BuildAll.Fw4.sln

# Сборка отдельного solution через MSBuild
msbuild Bwl.Framework.sln /p:Configuration=Debug

# Очистка (удаление артефактов сборки и .vs)
git clean -X -d -f
```

Выходные каталоги: `debug\dll\` (Debug), `release\dll\` (Release).

Pre/Post-Build events вызывают `tools\NetRevisionTool.exe` для автоматической подстановки версий.

## Tests

Unit-тесты в проекте `Bwl.Framework.Tests` используют **NUnit 3.13.3** (.NET Framework 4.8).

```bash
# Запуск всех тестов
nunit3-console.exe Bwl.Framework.Tests\bin\Debug\Bwl.Framework.Tests.dll

# Или через Visual Studio Test Explorer
```

Также есть `Bwl.Framework.Test` — интерактивное WinForms-приложение для ручного тестирования.

## Target Frameworks

- Основные проекты: **.NET Framework 4.8** (legacy .vbproj формат)
- `Bwl.Framework.Standard`: **netstandard2.0** (SDK-style .vbproj)
- Варианты `.Mono.vbproj` и `.Fw4.vbproj` для совместимости с Mono и другими версиями .NET Framework

## Solutions

- **`BuildAll.sln`** — зонтичный solution (все проекты, ~11 штук)
- **`Bwl.Framework.sln`** — ядро фреймворка + тесты (4 проекта)
- **`Bwl.Framework.Standard.sln`** — netstandard2.0 сборка
- Несколько специализированных `Bwl.Network.ClientServer*.sln`

## Architecture

Два основных модуля:

### Bwl.Framework (ядро)
- **AppBase** — базовые классы приложений: `FormAppBase` (WinForms), `ConsoleAppBase` (консоль), `ServiceLocator`
- **Settings** — иерархическое хранилище настроек в INI-файлах с буферизацией. Настройки создаются через `CreateChildStorage` и могут ограничиваться по `UserGroups`
- **Logger** — иерархическая система логирования с dispatcher/receiver паттерном и подключаемыми `ILogWriter`
- **AutoUI** — автоматическая генерация WinForms UI по атрибутам (`DisplayAttribute`, `ParameterAttribute`, `StateAttribute`)
- **refs-src/** — разделяемые исходники, подключаемые как ссылки: `IniFile`, `Serializer`, `CryptoTools`, `StringTools` и др.

### Bwl.Network.ClientServer (сеть)
- **Sockets** — TCP-клиент/сервер (`NetClient`, `NetServer`, `NetMessage`)
- **Remoting** — удалённое управление приложениями (AutoUI, Settings, Logs по сети)
- **NetDetect** — обнаружение сервисов в сети (`NetBeacon`, `NetFinder`)
- **SerialCable** — транспорт через последовательный порт
- **CmdRemoting** — командная строка для удалённого управления

### Dependency graph
```
Bwl.Network.ClientServer → Bwl.Framework
Bwl.Framework.Tests      → Bwl.Framework
Все Test/Tool проекты    → Bwl.Network.ClientServer + Bwl.Framework
```

## External Dependencies

Минимальные: только стандартные сборки .NET Framework + NUnit для тестов. NuGet-пакеты управляются через `packages.config`.
