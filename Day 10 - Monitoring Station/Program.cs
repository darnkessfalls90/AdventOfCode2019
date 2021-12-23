using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace MonitoringStation
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = Path.Combine(Directory.GetCurrentDirectory(), "Input.txt");
            var fileText = ReadFile(inputFile);
            var asteroids = FindAsteroids(fileText);

            foreach (var asteroid in asteroids)
            {
                asteroid.CanSeeCount = CountCanSee(asteroid, asteroids);
            }
            var max = asteroids.Max(a => a.CanSeeCount);
            var best = asteroids.Where(a => a.CanSeeCount == max).FirstOrDefault();
            Console.WriteLine("Best Asteroid - X: " + best.X + " Y: " + best.Y);
            Console.WriteLine("Asteroids Can See: " + max);

            asteroids = asteroids.OrderBy(a => a.X).ToArray();
            DestroyAsteroids(asteroids, best, fileText.GetLength(0), fileText.GetLength(1));
            asteroids.OrderBy(a => a.DestroyedCount).ToList().ForEach(a => Console.WriteLine(a.DestroyedCount + ": x-" + a.X + " y-" + a.Y + " angleFromHome: " + a.AngleFromHome));
            var twoHundreth = asteroids.Where(a => a.DestroyedCount == 199).FirstOrDefault();
            Console.WriteLine("Two Hundredth: - X: " + twoHundreth.X + " Y: " + twoHundreth.Y);
            Console.ReadLine();
        }

        public static int CountCanSee(Asteroid asteroid, Asteroid[] asteroids)
        {
            int seeCount = 0;
            foreach (var otherAsteroid in asteroids)
            {
                if (otherAsteroid == asteroid) continue;


                if (CanSee(asteroids, asteroid, otherAsteroid))
                {
                    seeCount++;
                }
            }
            return seeCount;
        }

        public static bool CanSee(Asteroid[] asteroids, Asteroid homeBase, Asteroid asteroid)
        {
            var canSee = true;
            var step = GetStep(homeBase, asteroid);
            var currentPoint = new Point(homeBase.X, homeBase.Y);
            while ((currentPoint.X + step.X != asteroid.X ||
                currentPoint.Y + step.Y != asteroid.Y)
                && canSee)
            {
                currentPoint = new Point(currentPoint.X + step.X, currentPoint.Y + step.Y);
                if (asteroids.Any(a => a.X == currentPoint.X && a.Y == currentPoint.Y && a.DestroyedCount == -1))
                    canSee = false;
            }
            return canSee;
        }

        public static void DestroyAsteroids(Asteroid[] asteroids, Asteroid homeBase, int height, int width)
        {
            CalculateAngles(asteroids, homeBase, height, width);

            asteroids = asteroids.OrderBy(a => a.AngleFromHome).ToArray();

            var asteroidsByAngle = SortByAngle(asteroids);

            var currentAsteroid = 0;
            var destroyed = 0;
            while (asteroids.Any(a => a.DestroyedCount == -1))
            {
                foreach (var angle in asteroidsByAngle.Keys)
                {
                    var current = asteroidsByAngle[angle].OrderBy(b => b.DistanceFromHome).FirstOrDefault(a => a.DestroyedCount == -1);

                    if (current != null)
                    {
                        if (current == homeBase)
                        {
                            homeBase.DestroyedCount = 0;
                        }
                        else if (CanSee(asteroids.Where(a => a.DestroyedCount == -1).ToArray(), homeBase, current))
                        {
                            current.DestroyedCount = ++destroyed;
                        }
                    }
                    if (currentAsteroid >= asteroids.Count() - 1)
                        currentAsteroid = 0;
                    else
                        currentAsteroid++;
                }
            }
        }

        public static Dictionary<double, List<Asteroid>> SortByAngle(Asteroid[] astroids){
            var result = new Dictionary<double, List<Asteroid>>();
            foreach(var a in astroids)
            {
                if (result.ContainsKey(a.AngleFromHome))
                {
                    result[a.AngleFromHome].Add(a);
                }
                else
                {
                    var list = new List<Asteroid>();
                    list.Add(a);
                    result.Add(a.AngleFromHome, list);
                }
            }
            return result;
        }

        public static Point GetStep(Asteroid home, Asteroid location)
        {
            var xdif = location.X - home.X;
            var ydif = location.Y - home.Y;

            if (xdif == 0)
            {
                if (ydif > 0)
                    ydif = 1;
                else
                    ydif = -1;
            }
            else if (ydif == 0)
            {
                if (xdif > 0)
                    xdif = 1;
                else
                    xdif = -1;
            }
            else
            {
                var divider = gcd(xdif, ydif);
                if (divider > 1)
                {
                    xdif = division(xdif, divider);
                    ydif = division(ydif, divider);
                }

            }
            return new Point(xdif, ydif);
        }

        public static Asteroid[] FindAsteroids(char[,] map)
        {
            var asteroids = new List<Asteroid>();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int n = 0; n < map.GetLength(1); n++)
                {
                    if (map[i, n] == '#')
                        asteroids.Add(new Asteroid(n, i));

                    Console.Write(map[i, n]);
                }
                Console.WriteLine();
            }
            return asteroids.ToArray();
        }

        public static char[,] ReadFile(string inputFile)
        {
            var readFile = new List<List<char>>();

            var file = new FileInfo(inputFile);
            var stream = new StreamReader(file.OpenRead());
            while (stream.Peek() > 0)
            {
                var line = stream.ReadLine();
                readFile.Add(line.ToList());
            }
            return ConvertTo2DArray(readFile);

        }

        public static t[,] ConvertTo2DArray<t>(List<List<t>> lists)
        {
            var longestList = lists.Max(i => i.Count);
            var result = new t[lists.Count, longestList];

            for (int i = 0; i < lists.Count; i++)
                for (int n = 0; n < longestList; n++)
                {
                    if (n >= lists[i].Count) continue;
                    result[i, n] = lists[i][n];
                }

            return result;
        }

        public static void CalculateAngles(Asteroid[] asteroids, Asteroid center, int height, int width)
        {
            foreach (var asteroid in asteroids)
            {
                var dif = new Point(asteroid.Point.X - center.Point.X, asteroid.Point.Y - center.Point.Y);
                var radians = Math.Atan2(dif.Y, dif.X);

                asteroid.AngleFromHome = (radians * (180 / Math.PI)) + 90;

                if (asteroid.AngleFromHome < 0)
                    asteroid.AngleFromHome = 360 + asteroid.AngleFromHome;

                //asteroid.AngleFromHome = GetAngle(center, new Point(width / 2, 0), asteroid);

                //if (asteroid.AngleFromHome < 0)
                //    asteroid.AngleFromHome = 360 + asteroid.AngleFromHome;

                asteroid.DistanceFromHome = Math.Max(Math.Abs(asteroid.X - center.X), Math.Abs(asteroid.Y - center.Y));
            }
        }

        static int gcd(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            //find the gcd using the Euclid’s algorithm
            while (a != b)
                if (a < b) b = b - a;
                else a = a - b;
            //since at this point a=b, the gcd can be either of them
            //it is necessary to pass the gcd to the main function
            return (a);
        }

        static int division(int a, int b)
        {
            int remainder = a, quotient = 0;
            while (Math.Abs(remainder) >= b)
            {
                if (remainder > 0)
                    remainder -= b;
                else
                    remainder += b;
                quotient++;
            }
            if (a < 0)
                quotient *= -1;
            return (quotient);
        }

        static float lengthSquare(Point p1, Point p2)
        {
            int xDiff = p1.X - p2.X;
            int yDiff = p1.Y - p2.Y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        static float GetAngle(Asteroid homeBase, Point startPoint, Asteroid asteroid)
        {
            var a2 = lengthSquare(startPoint, asteroid.Point);
            var b2 = lengthSquare(homeBase.Point, asteroid.Point);
            var c2 = lengthSquare(homeBase.Point, startPoint);

            float a = (float)Math.Sqrt(a2);
            float b = (float)Math.Sqrt(b2);
            float c = (float)Math.Sqrt(c2);

            float alpha = (float)Math.Acos((b2 + c2 - a2) /
                                               (2 * b * c));

            return (float)(alpha * 180 / Math.PI);
        }
    }

    class Asteroid
    {
        public int CanSeeCount { get; set; }
        public int DestroyedCount { get; set; } = -1;
        public double AngleFromHome { get; set; }
        private Point point = new Point(0,0);
        public int X { get { return point.X; } set { point.X = value; } }
        public int Y { get { return point.Y; } set { point.Y = value; } }
        public Point Point { get => point; set => point = value; }
        public int DistanceFromHome { get; set; }

        public Asteroid(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class AsteroidAngleComparer : IComparer<Asteroid>
    {
        public int Compare(Asteroid x, Asteroid y)
        {
            return x.AngleFromHome.CompareTo(y.AngleFromHome);
        }
    }

}
