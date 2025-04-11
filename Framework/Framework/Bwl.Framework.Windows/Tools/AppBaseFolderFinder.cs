using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    /// <summary>
    /// Класс для поиска папки с настройками и логами
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class AppBaseFolderFinder
    {

        /// <summary>
        /// Системный диск
        /// </summary>
        private static readonly string SystemDrive = Environment.GetEnvironmentVariable("SYSTEMDRIVE") + @"\";

        /// <summary>
        /// Получение актуального пути к папке с данными
        /// </summary>
        /// <param name="sysVariable">Системная переменная</param>
        /// <param name="defaultFolder">Путь к папке (предполагаемый или реальный)</param>
        /// <returns>Путь к папке (создаётся, если отсутствует)</returns>
        public static string GetPathToApplicationBaseFolder(string sysVariable, string defaultFolder)
        {
            string res;
            string sysVarPath = Environment.GetEnvironmentVariable(sysVariable);
            res = !string.IsNullOrWhiteSpace(sysVarPath) ? FindDataFolder(sysVarPath) : FindDataFolder(defaultFolder);
            return res;
        }

        /// <summary>
        /// Поиск папки по пути
        /// </summary>
        /// <param name="folderPath">Путь к папке (предполагаемый или реальный)</param>
        /// <returns>Путь к папке (создаётся если отсутствует)</returns>
        private static string FindDataFolder(string folderPath)
        {
            string res;
            string folderPathDrive = Path.GetPathRoot(folderPath);
            string folderPathWithoutDrive = folderPath.Substring(folderPathDrive.Length);

            if (Directory.Exists(folderPath))
            {
                res = folderPath;
            }
            else
            {
                res = FindFolderOnDisks(folderPathWithoutDrive);
                if (string.IsNullOrEmpty(res))
                {
                    string pathToFolder = Path.Combine(SystemDrive, folderPathWithoutDrive);
                    Directory.CreateDirectory(pathToFolder);
                    res = pathToFolder;
                }
            }

            return res;
        }

        /// <summary>
        /// Поиск папки на дисках
        /// </summary>
        /// <param name="pathWithoutDriveLetter">Путь к папке БЕЗ указания буквы диска</param>
        /// <returns>Путь к папке (если найден)</returns>
        private static string FindFolderOnDisks(string pathWithoutDriveLetter)
        {
            string res = "";
            var drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().OrderByDescending(c => c).Select(f => string.Concat(f.ToString(), @":\")).ToList();
            foreach (var drive in drives)
            {
                string pathToFolder = Path.Combine(drive, pathWithoutDriveLetter);
                if (Directory.Exists(pathToFolder))
                {
                    res = pathToFolder;
                    break;
                }
            }
            return res;
        }

    }
}