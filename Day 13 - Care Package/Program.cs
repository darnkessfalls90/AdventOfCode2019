using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarePackage
{
    class Program
    {
        static List<long> output = new List<long>();
        static long currentScore = 0;
        static Dictionary<Vector2, long> board = new Dictionary<Vector2, long>();
        static Dictionary<int, char> sprites = new Dictionary<int, char>()
        {
            {0, ' '},
            {1, '|' },
            {2, '■' },
            {3, '_' },
            {4, 'O' }
        };
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var fileInput = File.ReadAllText("input.txt");
            var input = ReadInput(fileInput);

            var interpreter = new IntcodeInterpreter();

            interpreter.HandleOutput += Interpreter_HandleOutput;

            interpreter.TakeInput += Interpreter_TakeInput;

            interpreter.Interpret(ref input);

            var board = GetBoard();
            var blockCount = board.Values.Count(v => v == 2);
            DrawBoard(board);
            Console.WriteLine("Number of blocks on board: " + blockCount);

            input = ReadInput(fileInput);
            input[0] = 2;

            interpreter.Interpret(ref input);

            board = GetBoard();
            DrawBoard(board);
            Console.ReadLine();
        }

        private static int? Interpreter_TakeInput()
        {
            board = GetBoard();
            DrawBoard(board);
            return CalculateMove();
        }

        static int CalculateMove()
        {
            if (board.Count == 0) return 0;

            var ball = board.First(kv => kv.Value == 4);
            var paddle = board.First(kv => kv.Value == 3);

            if (paddle.Key.X > ball.Key.X)
            {
                return -1;
            }
            else if (paddle.Key.X < ball.Key.X)
            {
                return 1;
            }

            return 0;
        }

        static void Interpreter_HandleOutput(long output, ref bool pause)
        {
            Program.output.Add(output);
        }

        static long[] ReadInput(string input)
        {
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }

        static void DrawBoard(Dictionary<Vector2, long> board)
        {
            Console.SetCursorPosition(0, 0);

            var minX = board.Keys.Select(p => p.X).Min();
            var maxX = board.Keys.Select(p => p.X).Max();

            var minY = board.Keys.Select(p => p.Y).Min();
            var maxY = board.Keys.Select(p => p.Y).Max();

            for (int y = (int)minY; y <= maxY; y++)
            {
                for (int x = (int)maxX; x >= minX; x--)
                {
                    var point = new Vector2(x, y);
                    if (board.ContainsKey(point))
                    {
                        Console.Write(sprites[(int)board[point]]);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Current Score: " + currentScore);
            //Thread.Sleep(10);
        }

        static Dictionary<Vector2, long> GetBoard()
        {
            var board = new Dictionary<Vector2, long>();
            for (int i = 0; i < output.Count; i += 3)
            {
                var position = new Vector2(output[i], output[i + 1]);
                if (position.X == -1)
                {
                    currentScore = output[i + 2];
                    continue;
                }

                if (board.ContainsKey(position))
                {
                    board[position] = output[i + 2];
                }
                else
                {
                    board.Add(position, output[i + 2]);
                }
            }
            return board;
        }
    }
}