using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;

namespace TopTenWords
{
    static class WordMatcher
    {
        /// <summary>
        /// Поиск слов в тексте по заданному паттерну
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns>Возвращает список слов</returns>
        static IEnumerable<string> MatchWords(string text, string pattern)
        {
            try
            {
                MatchCollection matchList = Regex.Matches(text.ToLower(),pattern);
                return matchList.Cast<Match>().Select(match => match.Value);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        /// <summary>
        /// Класс для поиска слов в нескольких текстах паралельно
        /// </summary>
        public static class ParallelWordMatcher
        {
            /// <summary>
            /// Контейнер для всех найденых слов
            /// </summary>
            static List<string> MatchedWords;
            /// <summary>
            /// Блокиратор для синхронизации потоков
            /// </summary>
            static object locker = new object();

            /// <summary>
            /// Добавляет слова в общий контейнер с синхронизацией потоков
            /// </summary>
            /// <param name="Words"></param>
            static void AddToMatchedWords(List<string> Words)
            {
                if (MatchedWords == null)
                    MatchedWords = new List<string>();

                lock (locker)
                {
                    MatchedWords.AddRange(Words);
                }
            }

            /// <summary>
            /// Возвращает все слова из нескольких строк, соответствующие паттерну
            /// </summary>
            /// <param name="Texts"></param>
            /// <param name="pattern"></param>
            /// <returns>Список слов</returns>
            static public IEnumerable<string> GetAllWords(IEnumerable<string> Texts, string pattern)
            {
                MatchedWords = new List<string>();


                List<Thread> startedThreads = new List<Thread>();
                foreach (var Text in Texts)
                {
                    Thread t = new Thread(() => AddToMatchedWords(MatchWords(Text, pattern).ToList()));
                    t.Start();
                    startedThreads.Add(t);
                }

                foreach (Thread t in startedThreads)
                    t.Join();

                return MatchedWords;
            }
        }


    }
}
