using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResortRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathFile = "C:/Users/Andres Arevalo/source/repos/SkiResortRoute/SkiResortRoute/4x4.txt";

            // 1. Read the file as one string.
            Tuple<int, int, List<int>, bool> fileRead = ReadAndDecodeFile(pathFile);


        }

        /// <summary>
        /// Reads the and decode file.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <returns></returns>
        private static Tuple<int, int, List<int>, bool> ReadAndDecodeFile(string pathFile)
        {
            var result = Tuple.Create(0, 0, new List<int>(), false);

            using (StreamReader skiingFile = new StreamReader(pathFile))
            {
                string[] firstLine = skiingFile.ReadLine().Split(' ');
                
                if (firstLine.Count() != 2)
                    return result;

                int rowsNumber = Convert.ToInt32(firstLine.First());
                int columnsNumber = Convert.ToInt32(firstLine.Last());
                int dimensions = (rowsNumber * columnsNumber);

                var contentFileRaw = skiingFile.ReadToEnd().Replace(" ", string.Empty)
                                                           .Replace("\r", string.Empty)
                                                           .Split('\n').ToList();

                var fileDecode = ReadFile(contentFileRaw);

                if (fileDecode?.Any() != true)
                    return result;

                bool convertStatus = (fileDecode.Count == dimensions) ? true: false;

                result = Tuple.Create(rowsNumber, columnsNumber, fileDecode, convertStatus);
            }

            return result;
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="contentFileRaw">The content file raw.</param>
        /// <returns></returns>
        private static List<int> ReadFile(List<string> contentFileRaw)
        {
            List<int> contentFile = new List<int>();

            try
            {
                foreach (var row in contentFileRaw)
                    row.ToList().ForEach(x => contentFile.Add(Convert.ToInt32(x.ToString())));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return contentFile;
        }
    }
}
