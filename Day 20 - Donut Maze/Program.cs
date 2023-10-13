using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DonutMaze
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var textInput = File.ReadAllLines("input.txt");
            var map = MapInput(textInput, out Dictionary<Vector2, string> doors);
            var start = doors.First(v => v.Value.Equals("AA"));
            //var steps = WalkMapPart1(start.Key, map, doors);
            //Console.WriteLine("Shortest Steps Part 1: " + steps);
            var steps = WalkMapPart2(new Vector3(start.Key, 0), map, doors);
            Console.WriteLine("Shortest Steps Part 2: " + steps);
        }

        public static char[,] MapInput(string[] input, out Dictionary<Vector2, string> doors)
        {
            var ret = new char[input.First().Length, input.Length];
            doors = new Dictionary<Vector2, string >();
            for (int y = 0; y < input.Length - 1; y++)
            {
                for (int x = 0; x < input[y].Length - 1; x++)
                {
                    var character = input[y][x];
                    ret[x, y] = character;

                    if (char.IsLetter(character))
                    {
                        if(IsDoorEntrance(input, new Vector2(x, y))){
                            if (y < input.Length - 1 && char.IsLetter(input[y + 1][x]))
                            {
                                doors.Add(new Vector2(x, y), string.Concat(input[y + 1][x], character));
                            }
                            else if(y > 0 && char.IsLetter(input[y - 1][x]))
                            {
                                doors.Add(new Vector2(x, y), string.Concat(character, input[y - 1][x]));
                            }
                            else if (x < input[y].Length - 1 && char.IsLetter(input[y][x + 1]))
                            {
                                doors.Add(new Vector2(x, y), string.Concat(input[y][x+1], character));
                            }
                            else if (x > 0 && char.IsLetter(input[y][x - 1]))
                            {
                                doors.Add(new Vector2(x, y), string.Concat(character, input[y][x - 1] ));
                            }
                        }
                    }
                }
            }
            return ret;
        }
        
        public static int WalkMapPart1(Vector2 startingPoint, char[,] map, Dictionary<Vector2, string> doors)
        {
            var que = new Queue<Vector2>();
            var distance = new Dictionary<Vector2, int>();
            que.Enqueue(startingPoint);
            distance.Add(startingPoint, -1);

            while(que.Count > 0)
            {
                var currentPoint = que.Dequeue();

                foreach(var point in GetSurrounding(currentPoint))
                {
                    if (distance.ContainsKey(point)) continue;
                    if (map[(int)point.X, (int)point.Y].Equals('.'))
                    {
                        que.Enqueue(point);
                        distance.Add(point, distance[currentPoint] + 1);
                    }
                    if(char.IsLetter(map[(int)point.X, (int)point.Y]))
                    {
                        var door = doors.FirstOrDefault(c => c.Key == point);
                        if (door.Equals(default(KeyValuePair<Vector2, string>))) continue;

                        if(door.Value == "ZZ")
                        {
                            return distance[currentPoint];
                        }

                        var otherDoor = doors.First(c => c.Value == door.Value && !c.Equals(door));
                        if (otherDoor.Equals(default(KeyValuePair<string, Vector2>))) throw new Exception("Could not find other door");

                        foreach(var newPoint in GetSurrounding(otherDoor.Key))
                        {
                            if (map[(int)newPoint.X, (int)newPoint.Y].Equals('.'))
                            {
                                distance.Add(door.Key, distance[currentPoint] + 1);
                                distance.Add(otherDoor.Key, distance[currentPoint] + 1);
                                que.Enqueue(newPoint);
                                distance.Add(newPoint, distance[currentPoint] + 1);
                            }
                        }
                    }
                }
            }
            return 0;            
        }

        public static int WalkMapPart2(Vector3 startingPoint, char[,] map, Dictionary<Vector2, string> doors)
        {
            var que = new Queue<Vector3>();
            var distance = new Dictionary<Vector3, int>();
            que.Enqueue(startingPoint);
            distance.Add(startingPoint, -1);

            while (que.Count > 0)
            {
                var currentPoint = que.Dequeue();

                foreach (var point in GetSurrounding(currentPoint))
                {
                    if (distance.ContainsKey(point)) continue;
                    if (map[(int)point.X, (int)point.Y].Equals('.'))
                    {
                        que.Enqueue(point);
                        distance.Add(point, distance[currentPoint] + 1);
                    }
                    if (char.IsLetter(map[(int)point.X, (int)point.Y]))
                    {
                        var door = doors.FirstOrDefault(c => c.Key.X == point.X && c.Key.Y == point.Y);
                        if (door.Equals(default(KeyValuePair<Vector2, string>))) continue;

                        if (door.Value == "ZZ")
                        {
                            if (currentPoint.Z == 0)
                                return distance[currentPoint];
                            else
                                continue;
                        }
                        if (door.Value == "AA")
                        {
                            continue;
                        }

                        if (currentPoint.Z == 0 && IsDoorOutside(map, door.Key))
                        {
                            continue;
                        }

                        var otherDoor = doors.First(c => c.Value == door.Value && !c.Equals(door));
                        if (otherDoor.Equals(default(KeyValuePair<string, Vector2>))) throw new Exception("Could not find other door");

                        foreach (var newPoint in GetSurrounding(otherDoor.Key))
                        {
                            if (map[(int)newPoint.X, (int)newPoint.Y].Equals('.'))
                            {
                                var parity = IsDoorOutside(map, door.Key) ? -1 : 1;
                                var newVector = new Vector3(newPoint, currentPoint.Z + parity);

                                if (distance.ContainsKey(newVector)) continue;

                                distance.Add(new Vector3(door.Key, currentPoint.Z), distance[currentPoint] + 1);
                                distance.Add(new Vector3(otherDoor.Key, currentPoint.Z + parity), distance[currentPoint] + 1);
                                que.Enqueue(newVector);
                                distance.Add(newVector, distance[currentPoint] + 1);
                            }
                        }
                    }
                }
            }
            return 0;
        }

        static List<Vector2> transforms = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
        };

        public static IEnumerable<Vector2> GetSurrounding(Vector2 point)
        {
            foreach(Vector2 trans in transforms)
            {
                yield return new Vector2(point.X + trans.X, point.Y + trans.Y);
            }
        }

        public static IEnumerable<Vector3> GetSurrounding(Vector3 point)
        {
            foreach (Vector2 trans in transforms)
            {
                yield return new Vector3(point.X + trans.X, point.Y + trans.Y, point.Z);
            }
        }

        public static bool IsDoorEntrance(string[] input, Vector2 point)
        {
            bool nextToLetter = false;
            bool nextToPath = false;
            foreach(var nexTo in GetSurrounding(point))
            {
                if (nexTo.Y < 0 || nexTo.Y > input.Length - 1) continue;
                if (nexTo.X < 0 || nexTo.X > input[0].Length - 1) continue;

                if (input[(int)nexTo.Y][(int)nexTo.X] == '.') nextToPath = true;
                if(char.IsLetter(input[(int)nexTo.Y][(int)nexTo.X])) nextToLetter = true;
            }
            return nextToPath && nextToLetter;
        }

        public static bool IsDoorOutside(char[,] map, Vector2 door)
        {
            if (door.X < 2 || door.X > map.GetLength(0) - 3) return true;
            if (door.Y < 2 || door.Y > map.GetLength(1) - 3) return true;
            return false;
        }

    }
}