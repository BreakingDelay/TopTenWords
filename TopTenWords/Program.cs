using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TopTenWords
{
    class Program
    {
        static string DefaultFolderPath = Environment.CurrentDirectory + "\\TextExamples\\";
        static string FolderPath;
        static int MinWordsLength = 0;
        static int WordsToShowCount = 10;
        static void Main(string[] args)
        {
            while (true)
            {

                Console.WriteLine("Введите путь к папке (enter для поиска по папке TextExamples):");
                FolderPath = Console.ReadLine();
                FolderPath = String.IsNullOrWhiteSpace(FolderPath) ? DefaultFolderPath : FolderPath;

                try
                {
                    var Files = FileFinder.GetFilesList(FolderPath);

                    var Texts = ParallelTextFilesReader.GetAllTexts(Files);


                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("Введите минимальную длинну слова (enter для поиска по любой длинне):");
                            string input = Console.ReadLine();
                            if (!String.IsNullOrWhiteSpace(input))
                                MinWordsLength = Convert.ToInt32(input);
                            else 
                                MinWordsLength = 0;

                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    string Pattern =String.Concat(@"\b\w{", MinWordsLength, @",}\b");
                    var AllWords = WordMatcher.ParallelWordMatcher.GetAllWords(Texts, Pattern);

                    var TopWords = AllWords.GroupBy(s => s).OrderByDescending(g => g.Count()).Take(WordsToShowCount).Select(s => s.Key);

                    foreach (var item in TopWords)
                    {
                        Console.WriteLine($"{item}") ;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
