using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkiResortRoute
{
    class Program
    {
        /// <summary>
        /// The rows count
        /// </summary>
        private static int rowsCount;

        /// <summary>
        /// The columns count
        /// </summary>
        private static int columnsCount;

        /// <summary>
        /// Navigation Class
        /// </summary>
        public class Navigation
        {
            /// <summary>
            /// Gets or sets the path lenght.
            /// </summary>
            /// <value>
            /// The path lenght.
            /// </value>
            public int PathLenght { get; set; }

            /// <summary>
            /// Gets or sets the nav coords.
            /// </summary>
            /// <value>
            /// The nav coords.
            /// </value>
            public NavigationCoordinates NavCoords { get; set; }
        }

        /// <summary>
        /// Navigation Coodinates Class
        /// </summary>
        public class NavigationCoordinates
        {
            /// <summary>
            /// Gets or sets the x coord.
            /// </summary>
            /// <value>
            /// The x coord.
            /// </value>
            public int xCoord { get; set; }

            /// <summary>
            /// Gets or sets the y coord.
            /// </summary>
            /// <value>
            /// The y coord.
            /// </value>
            public int yCoord { get; set; }
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            string pathFile = "C:/Users/Andres Arevalo/source/repos/SkiResortRoute/SkiResortRoute/4x4.txt";

            #region 1. Read and define GeoMap array from file and the differents Paths and Drops into arrays.
            Tuple<int, int, int[,], bool> fileRead = ReadAndDecodeFile(pathFile);

            if (fileRead.Item4 == false) { Console.WriteLine("File malformed"); return; }

            rowsCount = fileRead.Item1;
            columnsCount = fileRead.Item2;

            int[,] GeoMap = fileRead.Item3;
            int[,] Path = new int[rowsCount, columnsCount];
            int[,] Drop = new int[rowsCount, columnsCount];
            #endregion

            #region 2. Apply DFS algorithm
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    Navigation PathAux = DFSOperation(i, j, GeoMap);
                    Path[i, j] = PathAux.PathLenght;
                    Drop[i, j] = GeoMap[i, j] - GeoMap[PathAux.NavCoords.xCoord, PathAux.NavCoords.yCoord];
                }
            }
            #endregion

            #region 3. Get Coordinates of point with max path and drop in the Path[] and Drop[]

               int[] maxPathCoordXY = new int[2]; // Coordinate X,Y of point with max path and drop
               int maxPath = -1;                  // Have the longest path
               int maxDrop = -1;                  // Have the longest Drop

            // 3.1 Find maxPath and maxDrop 
               GetMaxPath(Path, Drop, maxPathCoordXY, ref maxPath, ref maxDrop);
            #endregion

            #region 4. Print Maximum Path and Drop
                Console.WriteLine($"Maximal Path is: {maxPath} \nMaximal Drop is: {maxDrop}");
            #endregion

            #region 5. Print all Path points from the maximum to the minimum Point (Skiing down)

               // Load respective CoordX and CoordY values
               List<int> listPath = DFSForMaxPathLength(maxPathCoordXY[0], maxPathCoordXY[1], fileRead.Item3);
               listPath.Reverse(); 

               Console.WriteLine($"\nThe Waypoints are: \n\n | XCoord | YCoord | SkiElevation |");

               int[] CoordXY = new int[2];
               int index = 0;
               int temp = 0;
               string printResult = string.Empty;

               foreach (var aux in listPath)
               {
                    CoordXY[index % 2] = aux;
                    index++;
               
                    if(index % 2 == 0)
                    {
                       printResult += string.Format(" |  {0}   |  {1}   | Point value: {2}| \n", temp, aux, 
                                                                                                 fileRead.Item3[CoordXY[0], CoordXY[1]]);    
                    }

                    temp = aux;
               }

               Console.WriteLine(printResult);
            #endregion

            Console.ReadLine();
        }

        /// <summary>
        /// Gets the maximum path.
        /// </summary>
        /// <param name="Path">The path.</param>
        /// <param name="Drop">The drop.</param>
        /// <param name="maxPathCoordXY">The maximum path coord xy.</param>
        /// <param name="maxPath">The maximum path.</param>
        /// <param name="maxDrop">The maximum drop.</param>
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

        /// <summary>
        /// DFSs the length of for maximum path.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="GeoMap">The geo map.</param>
        /// <returns></returns>
        public static List<int> DFSForMaxPathLength(int x, int y, int[,] GeoMap)
        {
            List<int> list = new List<int>();
            List<int> currentPathList = new List<int>();

            // Searching UP ▲ Direction
            if (y > 0 && GeoMap[x , y] > GeoMap[x , y - 1])
            {
                currentPathList = DFSForMaxPathLength(x, y - 1, GeoMap);
                if (currentPathList.Count > list.Count)
                    list = currentPathList;
            }

            // Searching DOWN ▼ Direction
            if (y < (columnsCount - 1) && GeoMap[x, y] > GeoMap[x , y + 1])
            {
                currentPathList = DFSForMaxPathLength(x, y + 1, GeoMap);
                if (currentPathList.Count > list.Count)
                    list = currentPathList;
            }

            // Searching LEFT ◄ Direction
            if (x > 0 && GeoMap[x , y] > GeoMap[x - 1 , y])
            {
                currentPathList = DFSForMaxPathLength(x - 1, y, GeoMap);
                if (currentPathList.Count > list.Count)
                    list = currentPathList;
            }

            // Searching RIGHT ► Direction
            if (x < (rowsCount - 1) && GeoMap[x ,y] > GeoMap[x + 1 ,y])
            {
                currentPathList = DFSForMaxPathLength(x + 1, y, GeoMap);
                if (currentPathList.Count > list.Count)
                    list = currentPathList;
            }

            list.Add(y); list.Add(x); 
            return list;
        }
    }
}
