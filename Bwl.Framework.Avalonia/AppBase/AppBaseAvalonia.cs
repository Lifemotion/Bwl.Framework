namespace Bwl.Framework.Avalonia;

/// <summary>
/// This class exists only to add AutoUI property to AppBase.
/// </summary>
public class AppBaseAvalonia : AppBase
{
    public AppBaseAvalonia() : 
        base()
    {
    }

    public AppBaseAvalonia(bool initFolders, string appName, bool useBufferedStorage, bool checkSettingsHash = true, string settingsFileName = "settings.conf") :
        base(initFolders, appName, useBufferedStorage, checkSettingsHash, settingsFileName)
    {
    }

    public AppBaseAvalonia(bool initFolders, string appName, bool useBufferedStorage, string baseFolderOverride, int maxLogFilesCount = 5, long maxLogFileLength = 10 * 1024 * 1024, bool isReadOnlySettings = false, bool onlyActiveSettings = false, bool checkSettingsHash = true, string settingsFileName = "settings.conf") : 
        base(initFolders, appName, useBufferedStorage, baseFolderOverride, maxLogFilesCount, maxLogFileLength, isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName)
    {
    }

    public AppBaseAvalonia(bool initFolders, string appName, bool useBufferedStorage, string settingsFolderOverride, string logsFolderOverride, string dataFolderOverride, int maxLogFilesCount = 5, long maxLogFileLength = 10 * 1024 * 1024, bool isReadOnlySettings = false, bool onlyActiveSettings = false, bool checkSettingsHash = true, string settingsFileName = "settings.conf") : 
        base(initFolders, appName, useBufferedStorage, settingsFolderOverride, logsFolderOverride, dataFolderOverride, maxLogFilesCount, maxLogFileLength, isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName)
    {
    }

    public override void Init(int maxLogFilesCount = 5, long maxLogFileLength = 10 * 1024 * 1024, bool isReadOnlySettings = false, bool onlyActiveSettings = false, bool checkSettingsHash = true, string settingsFileName = "settings.conf")
    {
        base.Init(maxLogFilesCount, maxLogFileLength, isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName);
        RootStorage.SetSettingsFormUiHandler(new SettingsFormUiHandlerAvalonia());
    }
}
