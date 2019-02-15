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
        private static int rowsCount;
        private static int columnsCount;

        static void Main(string[] args)
        {
            string pathFile = "C:/Users/Andres Arevalo/source/repos/SkiResortRoute/SkiResortRoute/map.txt";

            // 1. Read and define GeoMap array from file and the differents Paths and Drops into arrays.
            Tuple<int, int, int[,], bool> fileRead = ReadAndDecodeFile(pathFile);

            if (fileRead.Item4 == false) { Console.WriteLine("File malformed"); return; }

            rowsCount = fileRead.Item1;
            columnsCount = fileRead.Item2;

            int[,] GeoMap = fileRead.Item3;
            int[,] Path = new int[rowsCount, columnsCount];
            int[,] Drop = new int[rowsCount, columnsCount];

            // 2. Apply DFS algorithm
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    Navigation PathAux = DFSOperation(i, j, GeoMap);
                    Path[i, j] = PathAux.PathLenght;
                    Drop[i, j] = GeoMap[i, j] - GeoMap[PathAux.NavCoords.xCoord, PathAux.NavCoords.yCoord];
                }
            }

            // 3. Get Coordinates of point with max path and drop in the Path[] and Drop[]

            int[] maxPathCoordXY = new int[2]; // Coordinate X,Y of point with max path and drop
            int maxPath = -1;                  // Have the longest path
            int maxDrop = -1;                  // Have the longest Drop

            // 3.1 Find maxPath and maxDrop 
               GetMaxPath(Path, Drop, maxPathCoordXY, ref maxPath, ref maxDrop);


            // 4. Print Maximum Path
               Console.WriteLine($"Maximal Path is: {maxPath} \nMaximal Drop is: {maxDrop}");

               Console.ReadLine();
        }

        private static void GetMaxPath(int[,] Path, int[,] Drop, int[] maxPathCoordXY, ref int maxPath, ref int maxDrop)
        {
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    if (Path[i, j] > maxPath)
                    {   // if path[i][j] > maxPath, update maxPath and maxDrop
                        maxPath = Path[i, j];
                        maxDrop = Drop[i, j];

                        // Update coordinates too by the max point
                        maxPathCoordXY[0] = i;
                        maxPathCoordXY[1] = j;
                    }
                    if (Path[i, j] == maxPath)
                    {   // IF maxPaths are equals, compare the maxDrop
                        if (Drop[i, j] > maxDrop)
                        {
                            // if drop[i][j] > maxDrop, update maxDrop
                            maxDrop = Drop[i, j];

                            // Update coordinates too by the max point
                            maxPathCoordXY[0] = i;
                            maxPathCoordXY[1] = j;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads the and decode file.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <returns></returns>
        private static Tuple<int, int, int[,], bool> ReadAndDecodeFile(string pathFile)
        {
            var result = Tuple.Create(0, 0, new int[,] { }, false);

            using (StreamReader skiingFile = new StreamReader(pathFile))
            {
                string[] firstLine = skiingFile.ReadLine().Split(' ');

                if (firstLine.Count() != 2)
                    return result;

                int rowsNumber = Convert.ToInt32(firstLine.First());
                int columnsNumber = Convert.ToInt32(firstLine.Last());
                int dimensions = (rowsNumber * columnsNumber);

                int[,] contentFile = ReadFile(skiingFile, rowsNumber, columnsNumber);

                if (contentFile?.Length == 0)
                    return result;

                bool convertStatus = (contentFile?.LongLength == dimensions) ? true : false;

                result = Tuple.Create(rowsNumber, columnsNumber, contentFile, convertStatus);
            }

            return result;
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="skiingFile">The skiing file.</param>
        /// <param name="rowsNumber">The rows number.</param>
        /// <param name="columnsNumber">The columns number.</param>
        /// <returns></returns>
        private static int[,] ReadFile(StreamReader skiingFile, int rowsNumber, int columnsNumber)
        {
            int[,] contentFile = null;

            try
            {
                string line = string.Empty;
                contentFile = new int[rowsNumber, columnsNumber];
                int a = 0;

                while ((line = skiingFile.ReadLine()) != null)
                {
                    var row = line.Split(' ').ToList();

                    int b = 0;
                    row.ForEach(x =>
                    {
                        contentFile[a, b++] = int.Parse(x.ToString());
                    });
                    a++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return contentFile;
        }

        public class Navigation
        {
            public int PathLenght { get; set; }
            public NavigationCoordinates NavCoords { get; set; }
        }

        public class NavigationCoordinates
        {
            public int xCoord { get; set; }
            public int yCoord { get; set; }
        }

        /// <summary>
        /// DFSs the operation.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <param name="geoMap">The geo map.</param>
        /// <returns></returns>
        public static Navigation DFSOperation(int i, int j, int[,] geoMap)
        {
            Navigation pathAndDrop = new Navigation {
                                         PathLenght = 0,
                                         NavCoords = new NavigationCoordinates { xCoord = i, yCoord = j }
            };

            Navigation currentPathAndDrop = new Navigation();
            currentPathAndDrop.PathLenght = 0;
            currentPathAndDrop.NavCoords = new NavigationCoordinates();

            // Searching UP ▲ Direction
            if (j > 0 && geoMap[i , j] > geoMap[i , j - 1])
            {
                currentPathAndDrop = DFSOperation(i, j - 1, geoMap: geoMap);

                // If current path value is larger then Update path full length
                if (currentPathAndDrop.PathLenght > pathAndDrop.PathLenght) pathAndDrop = currentPathAndDrop;
            }

            // Searching DOWN ▼ Direction
            if (j < (columnsCount - 1) && geoMap[i , j] > geoMap[i, j + 1])
            {
                currentPathAndDrop = DFSOperation(i, j + 1, geoMap: geoMap);

                if (currentPathAndDrop.PathLenght > pathAndDrop.PathLenght) pathAndDrop = currentPathAndDrop;
            }

            // Searching LEFT ◄ Direction
            if (i > 0 && geoMap[i, j] > geoMap[i - 1 , j])
            {
                currentPathAndDrop = DFSOperation(i - 1, j, geoMap: geoMap);

                if (currentPathAndDrop.PathLenght > pathAndDrop.PathLenght) pathAndDrop = currentPathAndDrop;
            }

            // Searching RIGHT ► Direction
            if (i < (rowsCount - 1) && geoMap[i , j] > geoMap[i + 1 ,j])
            {
                currentPathAndDrop = DFSOperation(i + 1, j, geoMap: geoMap);

                if (currentPathAndDrop.PathLenght > pathAndDrop.PathLenght) pathAndDrop = currentPathAndDrop;
            }

            // For each recursion set a new Path Full Length
            pathAndDrop.PathLenght++;

            return pathAndDrop;
        }
    }
}
