using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SetandForget
{
    public class SetandForget
    {
        static int currentX = 0;
        static int currentY = 0;
        static List<Vector3> map = new List<Vector3>();
        static List<List<string>> paths = new List<List<string>>();
        static List<char> inputString;
        static char[] mapItems = new char[] { '#', '.', '^', (char)10 };

        public static Dictionary<Direction, Vector2> movements = new Dictionary<Direction, Vector2>()
        {
            {Direction.North, new Vector2(0, -1) },
            {Direction.South, new Vector2(0, 1)},
            {Direction.East, new Vector2(1, 0)},
            {Direction.West, new Vector2(-1, 0) }
        };

        public static Dictionary<int, string> Rotations = new Dictionary<int, string>();

        static SetandForget()
        {
            Rotations.Add((int)Direction.North | ((int)Direction.East << 16), "R");
            Rotations.Add((int)Direction.East | ((int)Direction.South << 16), "R" );
            Rotations.Add((int)Direction.South | ((int)Direction.West << 16), "R" );
            Rotations.Add((int)Direction.West | ((int)Direction.North << 16), "R" );
            Rotations.Add((int)Direction.North | ((int)Direction.West << 16), "L" );
            Rotations.Add((int)Direction.East | ((int)Direction.North << 16), "L" );
            Rotations.Add((int)Direction.South | ((int)Direction.East << 16), "L" );
            Rotations.Add((int)Direction.West | ((int)Direction.South << 16), "L" );
        }

        public static void Main(string[] args)
        {
            var fileInput = File.ReadAllText("input.txt");
            var input = ReadInput(fileInput);

            var computer = new IntcodeInterpreter();
            computer.HandleOutput += Computer_HandleOutput;

            var inputClone = input.Clone() as long[];
            computer.Interpret(ref inputClone);

            //LoadTestMap();

            var scaffolds = map.Where(v => v.Z == 35).ToArray();

            var intersections = GetIntersections(scaffolds);
            DrawMap(map, intersections);

            var alignmentParams = intersections.Select(v => v.X * v.Y).Sum();
            Console.WriteLine("Alignmet Parameter: " + alignmentParams);

            var intersectionsCount = new Dictionary<Vector3, int>();

            MapPaths(map.Where(v => ((char)v.Z) == '^').First(), Direction.North, Direction.East, scaffolds, new List<string>(), new List<Vector3>(), intersections, new Dictionary<Vector3, int>());
            MapPaths(map.Where(v => ((char)v.Z) == '^').First(), Direction.North, Direction.West, scaffolds, new List<string>(), new List<Vector3>(), intersections, new Dictionary<Vector3, int>());

            var subsequence = FindSubSequences();
            var sequence = FindSequence(subsequence.Path, subsequence.PathComponents);
            inputString = CreateInput(sequence, subsequence.PathComponents);

            Console.WriteLine();
            computer.TakeInput += Computer_TakeInput;
            input[0] = 2;

            computer.Interpret(ref input);

            Console.ReadLine();
        }

        static int inputCounter = 0;
        private static int? Computer_TakeInput()
        {
            var input = inputString[inputCounter++];
            Console.Write((char)input);
            return input;
        }

        private static void Computer_HandleOutput(long output, ref bool pause)
        {
            if (mapItems.Any(c => c == output))
            {
                if (output != 10)
                {
                    map.Add(new Vector3(currentX++, currentY, output));
                }
                else
                {
                    currentX = 0;
                    currentY++;
                }
            }
            else if(output < 256)
            {
                Console.Write((char)output);
                if((char)output == '?')
                {
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Dust Cleaned: " + output);
            }
        }

        static long[] ReadInput(string input)
        {
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }

        public static void DrawMap(List<Vector3> map, Vector3[] intersections)
        {
            var minX = (int)map.Min(v => v.X);
            var minY = (int)map.Min(v => v.Y);

            var maxX = (int)map.Max(v => v.X);
            var maxY = (int)map.Max(v => v.Y);

            for (int i = minY; i <= maxY; i++)
            {
                for (int n = minX; n <= maxX; n++)
                {
                    var point = map.FirstOrDefault(v => v.X == n && v.Y == i);
                    if (point == default(Vector3)) continue;

                    if (intersections.Contains(point))
                        Console.Write("0");
                    else
                        Console.Write((char)point.Z);
                }
                Console.WriteLine("");
            }
        }

        public static Vector3[] GetIntersections(Vector3[] scaffolds)
        {
            var intersections = new List<Vector3>();
            

            for(int i = 0; i < scaffolds.Length; i++)
            {
                if (map.FirstOrDefault(v => v.X == scaffolds[i].X && scaffolds[i].Y == v.Y + 1).Z == 35
                    && map.FirstOrDefault(v => v.X == scaffolds[i].X && scaffolds[i].Y == v.Y - 1).Z == 35
                    && map.FirstOrDefault(v => v.X + 1 == scaffolds[i].X && scaffolds[i].Y == v.Y).Z == 35
                    && map.FirstOrDefault(v => v.X - 1 == scaffolds[i].X && scaffolds[i].Y == v.Y).Z == 35)
                    intersections.Add(scaffolds[i]);
            }
            return intersections.ToArray();
        }

        public static void MapPaths(Vector3 currentPostion, Direction facing, Direction toGo, Vector3[] scaffolds, List<string> path, List<Vector3> visited, Vector3[] intersections, Dictionary<Vector3, int> intersectionTracker)
        {
            var rotation = GetRotation(facing, toGo);
            var movement = movements[toGo];
            var forward = 0;
            var passedIntersections = new List<Vector3>();
            while (true)
            {
                var moveTo = scaffolds.FirstOrDefault(s => currentPostion.X + movement.X == s.X
                                                       && currentPostion.Y + movement.Y == s.Y);
                if (moveTo == default(Vector3)) break;
                if (visited.Contains(moveTo)) break;

                forward++;
                currentPostion = moveTo;

                if (intersections.Contains(currentPostion))
                {
                    if (!intersectionTracker.ContainsKey(currentPostion))
                        intersectionTracker.Add(currentPostion, 1);
                    else
                    {
                        intersectionTracker[currentPostion]++;
                        visited.Add(currentPostion);    
                    }
                    if (intersectionTracker[currentPostion] > 2)
                    {
                        forward = 0;
                        break;
                    }

                    var curPath = new List<string>(path);
                    curPath.Add(rotation + forward);
                    MoveDirections(currentPostion, toGo, scaffolds, new List<string>(curPath), new List<Vector3>(visited), intersections, new Dictionary<Vector3, int>(intersectionTracker)); 
                }
                else
                {
                    visited.Add(currentPostion);
                }
            }
            if (forward == 0) return; //We can't go that way

            path.Add(rotation + forward);

            if (visited.Count() >= scaffolds.Length)
            {
                paths.Add(path);
                return;
            }

            MoveDirections(currentPostion, toGo, scaffolds, new List<string>(path), new List<Vector3>(visited), intersections, new Dictionary<Vector3, int>(intersectionTracker));
        }

        public static void MoveDirections(Vector3 currentPostion, Direction facing, Vector3[] scaffolds, List<string> path, List<Vector3> visited, Vector3[] intersections, Dictionary<Vector3, int> intersectionTracker)
        {
            var directionToMove = new Direction[2];
            switch (facing)
            {
                case Direction.North:
                case Direction.South:
                    directionToMove[0] = Direction.East;
                    directionToMove[1] = Direction.West;
                    break;
                case Direction.East:
                case Direction.West:
                    directionToMove[0] = Direction.North;
                    directionToMove[1] = Direction.South;
                    break;
            }

            foreach (var directin in directionToMove)
            {
                var movement = movements[directin];

                if (visited.Any(v => currentPostion.X + movement.X == v.X && currentPostion.Y + movement.Y == v.Y))
                    continue; //We already visited these, move on.

                MapPaths(currentPostion, facing, directin, scaffolds, path, visited, intersections, intersectionTracker);
            }

        }

        public static string GetRotation(Direction start, Direction end)
        {
            if (start == end) return "";
            return Rotations[((int)start | ((int)end) << 16)];
        }

        public static List<char> CreateInput(string sequence, Dictionary<char, List<string>> subsequences)
        {
            var input = new List<char>();
            foreach(char sequenceChar in sequence)
            {
                input.Add(sequenceChar);
            }
            input.Add((char)10);

            foreach(KeyValuePair<char, List<string>> pair in subsequences)
            {
                foreach(string part in pair.Value)
                {
                    for(int i = 0; i < part.Length; i++)
                    {
                        input.Add(part[i]);
                        if (i == 0) input.Add((char)44);
                    }
                    input.Add((char)44);
                }
                input.RemoveAt(input.Count - 1);
                input.Add((char)10);
            }

            input.Add((char)110);
            input.Add((char)10);

            return input;
        }

        public static dynamic FindSubSequences()
        {
            foreach (List<string> path in paths)
            {
                var tareApartPath = new List<string>(path);

                Dictionary<char, List<string>> pathComponents = new Dictionary<char, List<string>>();

                foreach (char sequencePart in new char[] {'A', 'B', 'C'})
                {
                    var testPath = new List<string>();
                    var lastInstances = 0;
                     for(int i = 3; i < 11; i++)
                    {
                        if (i == 3) lastInstances = 0;
                        testPath = tareApartPath.Take(i).ToList();
                        var instances = path.ListContains(testPath, i);
                        if (instances == 0) break;
                        if (instances < lastInstances) break;
                        lastInstances = instances;
                    }
                    testPath.RemoveAt(testPath.Count - 1);

                    pathComponents[sequencePart] = testPath;
                    tareApartPath = tareApartPath.RemoveSequences(testPath);
                }

                if (tareApartPath.Count == 0)
                    return new { Path = path, PathComponents = pathComponents };
            }

            return null;
        }

        public static string FindSequence(List<string> path, Dictionary<char, List<string>> components)
        {
            var sequence = new StringBuilder();
            while(path.Count > 0)
            {
                foreach(KeyValuePair<char, List<string>> component in components)
                {
                    if(path.IndexOf(component.Value) == 0)
                    {
                        path.RemoveRange(0, component.Value.Count);
                        sequence.Append(component.Key + ",");
                        break;
                    }
                }
            }
            return sequence.Remove(sequence.Length - 1, 1).ToString();
        }

        public static void LoadTestMap()
        {
            var lines = File.ReadAllLines("TestMap.txt");
            int x = 0;
            int y = 0;

            foreach(string line in lines)
            {
                foreach(char l in line)
                {
                    map.Add(new Vector3(x, y, (int)l));
                    x++;
                }
                y++;
                x = 0;
            }
        }
    }

    public enum Direction
    {
        North = 1,
        South = 2,
        West = 4,
        East = 8
    }
}