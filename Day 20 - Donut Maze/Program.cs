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
            var map = MapInput(textInput, out Dictionary<Point, string> doors);
            var start = doors.First(v => v.Value.Equals("AA"));
            var steps = WalkMapPart1(start.Key, map, doors);
            Console.WriteLine("Shortest Steps Part 1: " + steps);
        }

        public static char[,] MapInput(string[] input, out Dictionary<Point, string> doors)
        {
            var ret = new char[input.First().Length, input.Length];
            doors = new Dictionary<Point, string >();
            for (int y = 0; y < input.Length - 1; y++)
            {
                for (int x = 0; x < input[y].Length - 1; x++)
                {
                    var character = input[y][x];
                    ret[x, y] = character;

                    if (char.IsLetter(character))
                    {
                        if(IsDoorEntrance(input, new Point(x, y))){
                            if (y < input.Length - 1 && char.IsLetter(input[y + 1][x]))
                            {
                                doors.Add(new Point(x, y), string.Concat(input[y + 1][x], character));
                            }
                            else if(y > 0 && char.IsLetter(input[y - 1][x]))
                            {
                                doors.Add(new Point(x, y), string.Concat(character, input[y - 1][x]));
                            }
                            else if (x < input[y].Length - 1 && char.IsLetter(input[y][x + 1]))
                            {
                                doors.Add(new Point(x, y), string.Concat(input[y][x+1], character));
                            }
                            else if (x > 0 && char.IsLetter(input[y][x - 1]))
                            {
                                doors.Add(new Point(x, y), string.Concat(character, input[y][x - 1] ));
                            }
                        }
                    }
                }
            }
            return ret;
        }
        
        public static int WalkMapPart1(Point startingPoint, char[,] map, Dictionary<Point, string> doors)
        {
            var que = new Queue<Point>();
            var distance = new Dictionary<Point, int>();
            que.Enqueue(startingPoint);
            distance.Add(startingPoint, -1);

            while(que.Count > 0)
            {
                var currentPoint = que.Dequeue();

                foreach(var point in GetSurrounding(currentPoint))
                {
                    if (distance.ContainsKey(point)) continue;
                    if (map[point.X, point.Y].Equals('.'))
                    {
                        que.Enqueue(point);
                        distance.Add(point, distance[currentPoint] + 1);
                    }
                    if(char.IsLetter(map[point.X, point.Y]))
                    {
                        var door = doors.FirstOrDefault(c => c.Key == point);
                        if (door.Equals(default(KeyValuePair<Point, string>))) continue;

                        if(door.Value == "ZZ")
                        {
                            return distance[currentPoint];
                        }

                        var otherDoor = doors.First(c => c.Value == door.Value && !c.Equals(door));
                        if (otherDoor.Equals(default(KeyValuePair<string, Point>))) throw new Exception("Could not find other door");

                        foreach(var newPoint in GetSurrounding(otherDoor.Key))
                        {
                            if (map[newPoint.X, newPoint.Y].Equals('.'))
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

        static List<Point> transforms = new List<Point>
        {
            new Point(0, 1),
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
        };

        public static IEnumerable<Point> GetSurrounding(Point point)
        {
            foreach(Point trans in transforms)
            {
                yield return new Point(point.X + trans.X, point.Y + trans.Y);
            }
        }

        public static bool IsDoorEntrance(string[] input, Point point)
        {
            bool nextToLetter = false;
            bool nextToPath = false;
            foreach(var nexTo in GetSurrounding(point))
            {
                if (nexTo.Y < 0 || nexTo.Y > input.Length - 1) continue;
                if (nexTo.X < 0 || nexTo.X > input[0].Length - 1) continue;

                if (input[nexTo.Y][nexTo.X] == '.') nextToPath = true;
                if(char.IsLetter(input[nexTo.Y][nexTo.X])) nextToLetter = true;
            }
            return nextToPath && nextToLetter;
        }
    }
}