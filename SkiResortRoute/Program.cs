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
            int[][] matrix = new int[][]
            {
                new int[] { 4, 4},
                new int[] { 4, 8, 7, 3 },
                new int[] { 2, 5, 9, 3 },
                new int[] { 6, 3, 2, 5 },
                new int[] { 4, 4, 1, 6 }
            };

            // Read the file as one string.
            StreamReader skiingFile = new StreamReader("");
            string myString = skiingFile.ReadToEnd();
            

        }
    }
}
