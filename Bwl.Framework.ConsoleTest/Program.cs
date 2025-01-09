using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Bwl.Framework.Test.Console
{
    internal class Program
    {
        private readonly static AppBase _app = new AppBase(true, "Bwl.Framework.ConsoleTest", true, false, "settings.conf");
        private readonly static SettingsStorage _settings = _app.RootStorage;
        private readonly static Logger _logger = _app.RootLogger;
        private readonly static BwlConsoleTestLoggerWriter _logWriter = new BwlConsoleTestLoggerWriter(true);

        private static Task _logTask;
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static BooleanSetting _valBool;
        private static IntegerSetting _valInt;
        private static DoubleSetting _valDbl;
        private static StringSetting _valStr;
        private static VariantSetting _valVar;

        static void Main(string[] args)
        {
            _logger.ConnectWriter(_logWriter);

            _valBool = _settings.CreateBooleanSetting("ValBool", true);
            _valInt = _settings.CreateIntegerSetting("ValInt", 321);
            _valDbl = _settings.CreateDoubleSetting("ValDouble", 325.3);
            _settings.CreateTextFileContentSetting("TestTextFile");
            var ch1 = _settings.CreateChildStorage("ChildStorage1");
            _valStr = ch1.CreateStringSetting("ValString", "DefaultValue", "Настройка с текстом");
            var ch2 = _settings.CreateChildStorage("ChildStorage2");
            _valVar = ch2.CreateVariantSetting("ValVariant", "Value1", new string[] { "Value1", "Value2", "Value3" }, "Настройка с вариантами");
            _app.RootStorage.SaveSettings();
            _settings.SaveSettings();

            //_logger.AddMessage("App started");

            _logTask = Task.Run(() => LogSettingsValue(_cancellationTokenSource.Token));
            ShowMainMenu();
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch
            {
                // Do nothing
            }
            finally
            {
                _logTask.Wait();
                _cancellationTokenSource.Dispose();
            }
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                try
                {
                    System.Console.Clear();
                    System.Console.WriteLine($"{_app.AppName} v{Assembly.GetExecutingAssembly().GetName().Version}");
                    System.Console.WriteLine("Main menu:");
                    System.Console.WriteLine("0 - exit the app");
                    System.Console.WriteLine("1 - show log");
                    System.Console.WriteLine("2 - change settings");
                    System.Console.Write("Selected option: ");
                    var option = int.Parse(System.Console.ReadLine());
                    System.Console.WriteLine();

                    switch (option)
                    {
                        case 0:
                            return; // Leaving the app
                        case 1:
                            ShowLog();
                            break;
                        case 2:
                            ShowSettings(_settings);
                            break;
                        default:
                            throw new InvalidOperationException("Incorrect option");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleWriteError($"Please try again. Error: {ex.Message}", true);
                }
            }
        }

        /// <summary>
        /// This function contiuously adds values from settings to the log with 5 second intervals
        /// </summary>
        private async static Task LogSettingsValue(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _logger.Add(LogEventType.debug, $"{_valBool.Name} = {_valBool.Value}");
                    _logger.Add(LogEventType.debug, $"{_valInt.Name} = {_valInt.Value}");
                    _logger.Add(LogEventType.debug, $"{_valDbl.Name} = {_valDbl.Value}");
                    _logger.Add(LogEventType.debug, $"{_valStr.Name} = {_valStr.Value}");
                    _logger.Add(LogEventType.debug, $"{_valVar.Name} = {_valVar.Value}");
                    await Task.Delay(5000, token);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private static void ShowLog()
        {
            System.Console.Clear();
            System.Console.WriteLine("Log:");
            var origConsoleColor = System.Console.ForegroundColor;

            var colorVal = 1;
            var counter = 1;
            foreach (var message in _logWriter.GetLogRecords(DateTime.MinValue, new[] { LogEventType.all, LogEventType.debug,
                                LogEventType.information, LogEventType.message, LogEventType.warning, LogEventType.errors }, "", 100))
            {
                var msg = $"{message.DateTime} [{message.Type}] {message.Text}";
                // Change color randomly for each message
                System.Console.ForegroundColor = (ConsoleColor)(colorVal);
                System.Console.WriteLine(msg, true);
                counter++;
                if (counter > 5)
                {
                    colorVal++;
                    if (colorVal > 5) colorVal = 1;
                    counter = 1;
                }
            }
            System.Console.ForegroundColor = origConsoleColor;
            ConsolePause();
        }

        protected static bool ShowSettings(ISettingsStorage storageToShow)
        {
            while (true)
            {
                try
                {
                    System.Console.Clear();
                    System.Console.WriteLine($"Settings storage \"{storageToShow.FriendlyCategoryName}\" ({storageToShow.CategoryName}):");
                    System.Console.WriteLine("0 - return");

                    int optionCounter = 1; // Option counter
                    foreach (var settingStorage in storageToShow.ChildStorages)
                    {
                        System.Console.WriteLine($"{optionCounter} - (Category) {settingStorage.FriendlyCategoryName} ({settingStorage.CategoryName})");
                        optionCounter++;
                    }
                    foreach (var setting in storageToShow.GetSettings())
                    {
                        System.Console.WriteLine($"{optionCounter} - (Setting) {setting.FriendlyName} ({setting.Name}) = {setting.ValueAsString}");
                        optionCounter++;
                    }
                    System.Console.WriteLine($"{optionCounter} - return to main menu");
                    System.Console.Write("Selected option: ");
                    var option = int.Parse(System.Console.ReadLine());
                    System.Console.WriteLine();

                    // There must be a switch here but it doesn't support non-constant variables... So this will have to do.
                    if (option < 0 || option > optionCounter) throw new Exception("Incorrect option");
                    if (option == 0) return false;
                    if (option == optionCounter) return true;
                    if (option <= storageToShow.ChildStorages.Length)
                    {
                        var result = ShowSettings(storageToShow.ChildStorages[option - 1]);
                        if (result) return true;
                    }
                    else
                    {
                        ChangeSetting(storageToShow.GetSettings()[option - storageToShow.ChildStorages.Length - 1]);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleWriteError($"Please try again. Error: {ex.Message}", true);
                }
            }

            return true;
        }

        private static void ChangeSetting(SettingOnStorage setting)
        {
            System.Console.Clear();
            System.Console.WriteLine($"Setting \"{setting.FriendlyName}\" ({setting.Name})");
            System.Console.WriteLine($"Type: {setting.GetType().Name}");
            System.Console.WriteLine($"Value: {setting.ValueAsString}");
            if (setting.GetType() == typeof(VariantSetting))
            {
                var values = ((VariantSetting)setting).GetVariants();
                System.Console.WriteLine($"Variants:");
                for (int i = 0; i < values.Length; i++)
                {
                    System.Console.WriteLine($"{i + 1} - {values[i]}");
                }
                System.Console.WriteLine("Select new value (empty string to return): ");
                // Force user to input options until the right one is selected or empty string is input
                while (true)
                {
                    var newValue = System.Console.ReadLine();
                    if (newValue?.Length == 0) return;
                    if (int.TryParse(newValue, out int newIntValue) && newIntValue > 0 && newIntValue <= values.Length)
                    {
                        setting.ValueAsString = values[newIntValue - 1];
                        _settings.SaveSettings();
                        return;
                    }
                    ConsoleWriteError("Incorrect option. Please try again.", true);
                }
            }
            else if (setting.GetType() == typeof(TextFileContentSetting))
            {
                throw new InvalidOperationException($"Error: this type of setting cannot be edited in console mode. Please edit the file directly.");
            }
            else
            {
                System.Console.WriteLine("Enter new value (empty string to return, spacebar to input empty value): ");
                var newValue = System.Console.ReadLine();
                if (newValue?.Length == 0) return;
                if (String.IsNullOrWhiteSpace(newValue)) setting.ValueAsString = "";
                try
                {
                    switch (setting)
                    {
                        case BooleanSetting boolSetting:
                            boolSetting.Value = Boolean.Parse(newValue);
                            break;
                        case IntegerSetting intSetting:
                            intSetting.Value = Int32.Parse(newValue);
                            break;
                        case DoubleSetting doubleSetting:
                            doubleSetting.Value = Double.Parse(newValue.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        default:
                            setting.ValueAsString = newValue;
                            break;
                    }
                    _settings.SaveSettings();
                }
                catch (Exception ex)
                {
                    ConsoleWriteError($"Error: {ex.Message}", true);
                }
            }
        }

        private static void ConsoleWriteMessage(string msg, bool newLine)
        {
            var origForeColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.White;
            if (newLine)
                System.Console.WriteLine(msg);
            else
                System.Console.Write(msg);
            _logger.AddMessage(msg);
            System.Console.ForegroundColor = origForeColor;
        }

        private static void ConsoleWriteError(string msg, bool newLine)
        {
            var origForeColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            if (newLine)
                System.Console.WriteLine(msg);
            else
                System.Console.Write(msg);
            _logger.AddError(msg);
            System.Console.ForegroundColor = origForeColor;
            ConsolePause();
        }

        private static void ConsolePause()
        {
            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
        }
    }
}
