using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopTenWords
{
    /// <summary>
    /// Интерфейс для чтения файлов
    /// </summary>
    interface ITextFileReader
    {
        /// <summary>
        /// Получить текст из файла
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns>Текст из файл по указанному пути</returns>
        string GetText(string FilePath);
    }

    /// <summary>
    /// Класс для чтения файлов формата .txt
    /// </summary>
    class TxtFileReader : ITextFileReader
    {
        public string GetText(string FilePath)
        {
            return File.ReadAllText(FilePath);
        }
    }

    /// <summary>
    /// Класс для чтения файлов поддерживаемых форматов
    /// </summary>
    class FileReader 
    {
        private ITextFileReader reader;
        /// <summary>
        /// Метод выбирает нужный класс под соответствующий формат файла.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns>Текст из файла</returns>
        public string ReadFile(string FilePath)
        {
            switch (Path.GetExtension(FilePath))
            {
                case ".txt":
                    reader = new TxtFileReader();
                    return reader.GetText(FilePath);
                default:
                    throw new Exception($"{Path.GetExtension(FilePath)} не поддерживаемый формат");
            }
        }

    }

    /// <summary>
    /// Класс для паралельного чтения нескольких файлов
    /// </summary>
    public static class ParallelTextFilesReader
    {
        /// <summary>
        /// Контейнер для прочитанных файлов
        /// </summary>
        static List<string> Texts;
        /// <summary>
        /// Блокиратор для синхронизации потоков
        /// </summary>
        static object locker = new object();
        /// <summary>
        /// Список поддерживаемых форматов
        /// </summary>
        static string[] SupportedTextFiles = { ".txt" };

        /// <summary>
        /// Добавляет текст в общий контейнер с синхронизацией потоков
        /// </summary>
        /// <param name="text"></param>
        static void AddToTexts(string text)
        {
            if (Texts == null)
                Texts = new List<string>();

            lock (locker)
            {
                Texts.Add(text);
            }
        }

        /// <summary>
        /// Получить все тексты из указанных файлов
        /// </summary>
        /// <param name="Files"></param>
        /// <returns>Список с текстами из указанных файлов</returns>
        static public IEnumerable<string> GetAllTexts(IEnumerable<string> Files)
        {
            
            if (Files == null)
                throw new Exception("В директории нет файлов");
            List<string> SupportedFiles = Files.Where(a => SupportedTextFiles.Any(b => b == Path.GetExtension(a))).ToList();
            if (!SupportedFiles.Any())
                throw new Exception($"В директории нет файлов поддерживаемого формата. Поддерживаемые форматы: {String.Join(' ',SupportedTextFiles)}");

            Texts = new List<string>();


            //Вариант через Threads
            List<Thread> startedThreads = new List<Thread>();
            foreach (var File in SupportedFiles)
            {

                Thread t = new Thread(() => AddToTexts(new FileReader().ReadFile(File)));
                t.Start();
                startedThreads.Add(t);
            }

            foreach (Thread t in startedThreads)
                t.Join();

            //Вариант через tasks
            //Parallel.ForEach(SupportedFiles, (File) => { Texts.Add(new FileReader().ReadFile(File)); });


            return Texts;
        }



    }


    


}
