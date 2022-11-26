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
            var map = MapTractorBeams(input, out int total);
            DrawMap(map);
            Console.WriteLine("Total Beams: " + total);
            Console.ReadLine();
        }

        public static long[,] MapTractorBeams(long[] input, out int total)
        {
            total = 0;
            var result = new long[50,50];
            var computer = new AsyncIntcodeInterpreter();
            var minx = 0;
            for (int y = 0; y < 50; y++)
            {
                bool foundOne = false;
                for(int x = minx; x < 50; x++)
                {
                    computer.Begin(input);

                    computer.GiveInput(x);
                    computer.GiveInput(y);
                    var space = computer.RecieveOutput(1).First();
                    if (space == 1)
                    {
                        if (!foundOne)
                            minx = x;
                        result[x, y] = 1;
                        total++;
                        foundOne = true;
                    }
                    else
                    {
                        if (foundOne) break;
                    }
                }
            }
            return result;
        }

        public static void DrawMap(long[,] map)
        {
            for (int y = 0; y < 50; y++)
            {
                Console.Write(y < 10 ? " " + y : y);
                for (int x = 0; x < 50; x++)
                {
                    Console.Write(map[x, y] == 1 ? (x < 10 ? " " + x : x) : " .");
                }
                Console.WriteLine();
            }
        }
    }
}