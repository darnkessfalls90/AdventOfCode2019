using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpacePolice
{
    class Program
    {
        private static Vector2 currentPosition = new Vector2(0, 0);
        // private static Dictionary<Vector2, int> painted = new Dictionary<Vector2, int>(); //Part 1
        private static Dictionary<Vector2, int> painted = new Dictionary<Vector2, int>() { { new Vector2(0, 0), 1 } }; //Part 2
        private static Vector2 moveDirection = new Vector2(0, -1);

        static void Main(string[] args)
        {
            var fileInput = File.ReadAllText("input.txt");
            var input = ReadInput(fileInput);

            var interpreter = new IntcodeInterpreter();

            interpreter.TakeInput += Interpreter_TakeInput;
            interpreter.HandleOutput += Interpreter_HandleOutput;

            interpreter.Interpret(ref input);

            Console.WriteLine("Number of squares painted: " + painted.Count);
            DrawPainted();
            Console.ReadLine();
        }

        private static bool PaintOutput = true;
        private static void Interpreter_HandleOutput(long output, ref bool pause)
        {
            if (PaintOutput)
            {
                if (painted.ContainsKey(currentPosition)) painted[currentPosition] = (int)output;
                else painted.Add(currentPosition, (int)output);
            }
            else
            {
                if(output == 0)
                {
                    moveDirection = new Vector2(moveDirection.Y * -1, moveDirection.X);
                }
                else
                {
                    moveDirection = new Vector2(moveDirection.Y, moveDirection.X * -1);
                }

                currentPosition = new Vector2(currentPosition.X + moveDirection.X, currentPosition.Y + moveDirection.Y);
            }
            PaintOutput = !PaintOutput;
        }

        private static int? Interpreter_TakeInput()
        {
            if (!painted.ContainsKey(currentPosition)) return 0;
            return painted[currentPosition];
        }

        static long[] ReadInput(string input)
        {
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }

        static void DrawPainted()
        {
            var minX = painted.Keys.Select(p => p.X).Min();
            var maxX = painted.Keys.Select(p => p.X).Max();

            var minY = painted.Keys.Select(p => p.Y).Min();
            var maxY = painted.Keys.Select(p => p.Y).Max();

            for(int y = (int)minY; y <= maxY; y++)
            {
                for(int x = (int)maxX; x >= minX; x--)
                {
                    if (!painted.ContainsKey(new Vector2(x, y))) Console.ForegroundColor = ConsoleColor.Black;
                    else Console.ForegroundColor = painted[new Vector2(x, y)] == 1 ? ConsoleColor.White : ConsoleColor.Black;

                    Console.Write("■");
                }
                Console.WriteLine();
            }
        }
    }
}
