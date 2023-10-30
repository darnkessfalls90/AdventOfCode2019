using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PlanetOfDiscord
{
    public class PlanetOfDiscord
    {
        public static void Main(string[] args)
        {
            var grid = ReadInput();
            SolvePart1(grid.DeepClone());
            SolvePart2(grid);
        }

        public static void SolvePart1(BugGrid grid)
        {
            List<int> layouts = new List<int>();
            layouts.Add(grid.GetHashCode());

            while (true)
            {
                grid = grid.MoveAhead1Minute();
                var gLayout = grid.GetHashCode();
                if (layouts.Contains(gLayout))
                {
                    break;
                }
                layouts.Add(gLayout.GetHashCode());
            }

            //PrintGrid(grid.grid);
            Console.WriteLine("Biodiversity: " + grid.GetHashCode());
        }

        public static void SolvePart2(BugGrid grid)
        {
            Dictionary<int, BugGrid> levels = new Dictionary<int, BugGrid>()
            {
                {0, grid }
            };

            var minLevel = 0;
            var maxLevel = 0;

            for(int i = 0; i < 200; i++)
            {
                var newLevels = new Dictionary<int, BugGrid>();
                if (HasInnerEdgeBugs(levels[minLevel].grid))
                {
                    minLevel--;
                    levels.Add(minLevel, new BugGrid(new bool[5, 5]));
                }

                if (HasOuterEdgeBugs(levels[maxLevel].grid))
                {
                    maxLevel++;
                    levels.Add(maxLevel, new BugGrid(new bool[5,5]));
                }

                for (int z = minLevel; z <= maxLevel; z++)
                {
                    var newGrid = new BugGrid(new bool[5, 5]);
                    for (int x = 0; x < levels[z].grid.GetLength(0); x++)
                    {
                        for (int y = 0; y < levels[z].grid.GetLength(1); y++)
                        {
                            if(x == 2 && y == 2) continue;
                            var bugs = GetAdjacentBugs(levels[z],x, y);
                            if(bugs > 2)
                            {
                                newGrid.grid[x, y] = false;
                                continue;
                            }
                            if (levels.ContainsKey(z - 1))
                            {
                                if(x == 0)
                                {
                                    if (levels[z - 1].grid[1, 2])
                                        bugs++;
                                }
                                if (y == 0)
                                {
                                    if (levels[z - 1].grid[2, 1])
                                        bugs++;
                                }
                                if (x == 4)
                                {
                                    if (levels[z - 1].grid[3, 2])
                                        bugs++;
                                }
                                if (y == 4)
                                {
                                    if (levels[z - 1].grid[2, 3])
                                        bugs++;
                                }
                            }
                            if (IsInnerEdge(x, y) && levels.ContainsKey(z +1))
                            {
                                if (x == 1)
                                {
                                    for (int b = 0; b < 5; b++)
                                        if (levels[z + 1].grid[0,b])
                                            bugs++;
                                            
                                }
                                if (y == 1)
                                {
                                    for (int b = 0; b < 5; b++)
                                        if (levels[z + 1].grid[b, 0])
                                            bugs++;

                                }
                                if(x == 3)
                                {
                                    for (int b = 0; b < 5; b++)
                                        if (levels[z + 1].grid[4, b])
                                            bugs++;
                                }
                                if (y == 3)
                                {
                                    for (int b = 0; b < 5; b++)
                                        if (levels[z + 1].grid[b, 4])
                                            bugs++;
                                }
                            }

                            if (levels[z].grid[x, y])
                            {
                                if (bugs == 1)
                                    newGrid.grid[x, y] = true;
                                else
                                    newGrid.grid[x, y] = false;
                            }
                            else
                            {
                                if (bugs == 1 || bugs == 2)
                                    newGrid.grid[x, y] = true;
                                else
                                    newGrid.grid[x, y] = false;
                            }
                        }
                    }
                    newLevels.Add(z, newGrid);
                }
                levels = newLevels;
            }

            //for(int i = minLevel; i <= maxLevel; i++)
            //{
            //    Console.WriteLine("Depth: " + i);
            //    PrintGrid(levels[i].grid);
            //    Console.WriteLine();
            //}

            var total = 0;
            for(int i = minLevel; i <= maxLevel; i++)
            {
                for(int x = 0; x < 5; x++)
                {
                    for(int y = 0; y < 5; y++)
                    {
                        if (levels[i].grid[x, y])
                            total++;
                    }
                }
            }
            Console.WriteLine("Total Bugs: " + total);
        }

        static BugGrid ReadInput()
        {
            bool[,] grid = new bool[5, 5];
            var input = File.ReadAllLines("input.txt");
            for(int i = 0; i < input.Length; i++)
            {
                for(int n = 0; n < input[i].Length; n++)
                {
                    grid[i, n] = input[i][n] == '#';
                }
            }
            return new BugGrid(grid);
        }

        static void PrintGrid(bool[,] grid)
        {
            for(int i = 0; i < grid.GetLength(0); i++)
            {
                for(int n = 0; n < grid.GetLength(1); n++)
                {
                    Console.Write(grid[i,n] ? '#' : '.');
                }
                Console.WriteLine();
            }
        }

        public static Vector2[] adjacents = new Vector2[]
        {
            new Vector2(0, -1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(-1, 0)
        };

        static Vector2[] outerEdge = new Vector2[]
        {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(0,2),
                new Vector2(0,3),
                new Vector2(0,4),

                new Vector2(1,0),
                new Vector2(2,0),
                new Vector2(3,0),
                new Vector2(4,0),

                new Vector2(4,0),
                new Vector2(4,1),
                new Vector2(4,2),
                new Vector2(4,3),
                new Vector2(4,4),

                new Vector2(1,4),
                new Vector2(2,4),
                new Vector2(3,4),
                new Vector2(4,4),

        };

        static Vector2[] innerEdge = new Vector2[]
        {
                new Vector2(2,3),
                new Vector2(3,2),
                new Vector2(2,1),
                new Vector2(1,2)
        };
        static bool HasOuterEdgeBugs(bool[,] level)
        {

            return outerEdge.Count(p => level[(int)p.Y, (int)p.X]) > 0;
        }

        static bool IsOuterEdge(int x, int y)
        {
            return outerEdge.Any(c => c.X == x && c.Y == y);
        }

        static bool HasInnerEdgeBugs(bool[,] level)
        {
            return innerEdge.Count(p => level[(int)p.Y, (int)p.X]) > 0;
        }

        static bool IsInnerEdge(int x, int y)
        {
            return innerEdge.Any(c => c.X == x && c.Y == y);
        }

        private static int GetAdjacentBugs(BugGrid grid, int x, int y)
        {
            var bugs = 0;

            foreach(var adj in adjacents)
            {
                var newx = (int)(x + adj.X);
                var newy = (int)(y + adj.Y);

                if(newx == 2 && newy == 2)
                {
                    continue;
                }
                else if(newx >= 0 && newx < 5 && newy >= 0 && newy < 5){
                    if (grid.grid[newx, newy]) bugs++;
                }
            }

            return bugs;
        }
    }
}