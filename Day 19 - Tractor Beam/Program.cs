using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Intcode;

namespace TractorBeam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = IntcodeInterpreter.ReadInput("input.txt");
            var map = MapTractorBeams(50, input, out int total, out int level10Min, out int level10XMax, out int level50XMin, out int level50XMax);
            DrawMap(map, 50);


            //These end up being wrong. I don't know why. But close enough to give me approximations to speed up the program
            var minAngle = CalculateAngle(level10Min, 10, level50XMin, 50);
            var maxAngle = CalculateAngle(level10XMax, 10, level50XMax, 50);

            //First Y where the beam is 100 across, approximately. A good place to start a binary search
            var firstwith100 = FindFirstWith(100, minAngle, maxAngle);

            var result = FindAnswer2(99, firstwith100, minAngle, maxAngle, input);

            Console.WriteLine("Total Beams: " + total);
            Console.WriteLine("First place to pick up the sleigh: X-" + result.X + "; Y-" + result.Y + ";");
            Console.WriteLine("Answer 2: " + ((result.X * 10000) + result.Y));
            Console.ReadLine();
        }

        public static long[,] MapTractorBeams(int length, long[] input, out int total, out int level10XMin, out int level10XMax,
            out int level50XMin, out int level50XMax)
        {
            level10XMin = 0;
            level10XMax = 0;
            level50XMin = 0;
            level50XMax = 0;
            total = 0;
            var result = new long[length, length];
            using (var computer = new AsyncIntcodeInterpreter())
            {
                var minx = 0;
                for (int y = 0; y < length; y++)
                {
                    bool foundOne = false;
                    for (int x = minx; x < length; x++)
                    {
                        computer.Begin(input);

                        computer.GiveInput(x);
                        computer.GiveInput(y);
                        var space = computer.RecieveOutput(1).First();
                        if (space == 1)

                        {
                            if (!foundOne)
                            {
                                minx = x;
                                if (y == 9) level10XMin = x;
                                if (y == 49) level50XMin = x;
                            }
                            result[x, y] = 1;
                            total++;
                            foundOne = true;
                        }
                        else
                        {
                            if (foundOne)
                            {
                                if (y == 9) level10XMax = x - 1;
                                if (y == 49) level50XMax = x - 1;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        static bool IsValid(int x, int y, long[] input)
        {
            using (var computer = new AsyncIntcodeInterpreter())
            {
                computer.Begin(input);

                computer.GiveInput(x);
                computer.GiveInput(y);
                var space = computer.RecieveOutput(1).First();
                return space == 1;
            }
        }

        static decimal CalculateAngle(int x1, int y1, int x2, int y2)
        {
            return ((decimal)x2 - x1) / ((decimal)y2 - y1);
        }

        static int FindFirstWith(int width, decimal minAngle, decimal maxAngle)
        {
            return (int)Math.Floor((width + 1) / (maxAngle - minAngle));
        }

        static Vector2 FindAnswer2(int length, int startingY, decimal minAngle, decimal maxAngle, long[] input)
        {
            while (startingY < 10000)
            {
                var minX = (int)Math.Ceiling(minAngle * startingY);
                var valid = false;
                while (!IsValid(minX, startingY, input)) minX++;
                if (IsValid(minX + length, startingY - length, input))
                {
                    return new Vector2(minX, startingY - length);
                }
                startingY++;
            }
            return new Vector2();
        }

        public static void DrawMap(long[,] map, int length)
        {
            for (int y = 0; y < length; y++)
            {
                Console.Write(y < 10 ? "0" + y : y);
                for (int x = 0; x < length; x++)
                {
                    Console.Write(map[x, y] == 1 ? (x < 10 ? " " + x : x) : "  ");
                }
                Console.WriteLine();
            }
        }
    }
}