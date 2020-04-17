using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TopTenWords
{
    /// <summary>
    /// Класс для поиска файлов в папке
    /// </summary>
    static class FileFinder
    {
        /// <summary>
        /// Получить список файлов по указанному пути
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <returns>Список файлов</returns>
        public static IEnumerable<string> GetFilesList (string FolderPath)
        { 
            try
            {
                return Directory.EnumerateFiles(FolderPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
