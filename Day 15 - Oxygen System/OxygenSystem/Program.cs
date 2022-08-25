using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace OxygenSystem
{
    public class OxygenSystem{
        public static void Main(string[] args)
        {
            var fileInput = File.ReadAllText("input.txt");
            var input = ReadInput(fileInput);
            var map = new List<Vector3>();
            map.Add(new Vector3(0, 0, 3));
            var paths = new List<Vector2[]>();
            MapSystem(map, new DroidController(input, new Vector2(0, 0)), paths);
            var shortest = paths.First(p => p.Length == paths.Min(l => l.Length));
            DrawMap(map, shortest);
            Console.WriteLine("Minimum Path: " + (shortest.Length - 1));

            paths = new List<Vector2[]>();
            var OS = map.First(m => m.Z == (int)StatusCode.OSFound);
            MapPaths(new Vector2(OS.X, OS.Y), map, new List<Vector2>(), paths);
            Console.WriteLine("Longest Path from OS: " + (paths.Max(p => p.Length) - 1));

            Console.ReadLine();
        }

        static long[] ReadInput(string input)
        {
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }

        static void MapSystem(List<Vector3> map, DroidController controller, List<Vector2[]> paths)
        {
            for (int i = 1; i < 5; i++)
            {
                var toMoveTo = controller.CurrentPosition + DroidController.movements[(Direction)i];

                var movingTo = map.FirstOrDefault(l => l.X == toMoveTo.X && l.Y == toMoveTo.Y);
                if (movingTo != default(Vector3)) {
                    if (movingTo.Z == (int)StatusCode.OSFound)
                    {
                        var fullPath = new Vector2[controller.Path.Count + 1];
                        Array.Copy(controller.Path.ToArray(), fullPath, controller.Path.Count);    
                        fullPath[controller.Path.Count] = toMoveTo;
                        paths.Add(fullPath);
                    }
                    continue; 
                }

                var newController = (DroidController)controller.Clone();
                newController.Direction = (Direction)i;
                newController.Run();

                if (newController.Status != StatusCode.NoChange)
                {
                    if (newController.Status == StatusCode.OSFound)
                    {
                        paths.Add(newController.Path.ToArray());
                    }
                    map.Add(new Vector3(newController.CurrentPosition.X, newController.CurrentPosition.Y, (int)newController.Status));
                    MapSystem(map, newController, paths);
                }
                else
                {
                    newController.CurrentPosition += DroidController.movements[newController.Direction];
                    map.Add(new Vector3(newController.CurrentPosition.X, newController.CurrentPosition.Y, (int)newController.Status));
                }

            }
        }

        public static void DrawMap(List<Vector3> map, Vector2[] ShortestPath)
        {
            var minX = (int)map.Min(v => v.X);
            var minY = (int)map.Min(v => v.Y);

            var maxX = (int)map.Max(v => v.X);
            var maxY = (int)map.Max(v => v.Y);

            for (int i = minX; i <= maxX; i++)
            {
                for (int n = minY; n <= maxY; n++)
                {
                    var point = map.FirstOrDefault(v => v.X == i && v.Y == n);
                    if (ShortestPath.Any(s => s.X == i && s.Y == n))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (point.Z == (int)StatusCode.Success)
                            Console.Write("█");
                        else if (point.Z == (int)StatusCode.OSFound)
                            Console.Write("O");
                        else if (i == 0 && n == 0)
                            Console.Write("B");
                        Console.ResetColor();
                    }
                    else if (point == default(Vector3))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("█");
                        Console.ResetColor();
                    }
                    else if (point.Z == (int)StatusCode.NoChange)
                        Console.Write("█");
                    else if (point.Z == (int)StatusCode.Success)
                        Console.Write(" ");
                    else if (point.Z == (int)StatusCode.OSFound)
                        Console.Write("O");
                    else if (i == 0 && n == 0)
                        Console.Write("B");

                }
                Console.WriteLine("");
            }
        }

        public static void MapPaths(Vector2 CurrentPosition, List<Vector3> map, List<Vector2> Path, List<Vector2[]> Paths)
        {
            Path.Add(CurrentPosition);

            bool moved = false;
            for (int i = 1; i < 5; i++)
            {
                var toMoveTo = CurrentPosition + DroidController.movements[(Direction)i];

                var movingTo = map.FirstOrDefault(l => l.X == toMoveTo.X && l.Y == toMoveTo.Y);
                if (movingTo == default(Vector3) || movingTo.Z == (int)StatusCode.NoChange
                    || Path.Any(p => p.X == movingTo.X && p.Y == movingTo.Y))
                {
                    continue;
                }
                moved = true;
                MapPaths(toMoveTo, map, new List<Vector2>(Path), Paths);
            }

            if (!moved)
                Paths.Add(Path.ToArray());
        }
    }
}