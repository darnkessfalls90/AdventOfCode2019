using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1202ProgramAlarm
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] program = { 1, 12, 2, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 10, 1, 19, 2, 9,
                19, 23, 2, 13, 23, 27, 1, 6, 27, 31, 2, 6, 31, 35, 2, 13, 35, 39, 1, 39, 10, 43,
                2, 43, 13, 47, 1, 9, 47, 51, 1, 51, 13, 55, 1, 55, 13, 59, 2, 59, 13, 63, 1, 63,
                6, 67, 2, 6, 67, 71, 1, 5, 71, 75, 2, 6, 75, 79, 1, 5, 79, 83, 2, 83, 6, 87, 1, 5,
                87, 91, 1, 6, 91, 95, 2, 95, 6, 99, 1, 5, 99, 103, 1, 6, 103, 107, 1, 107, 2, 111,
                1, 111, 5, 0, 99, 2, 14, 0, 0 };

            program[1] = 12;
            program[2] = 2;
            Console.WriteLine("Part 1");
            Console.WriteLine("Input: " + string.Join(",", program.Select(p => p.ToString())));

            var part1 = new int[program.Length];
            Array.Copy(program, part1, program.Length);

            part1 = IntcodeInterpreter.Interpret(part1);
            Console.WriteLine("Output: " + string.Join(",", part1.Select(p => p.ToString())));
            Console.WriteLine("\r\nPart 2");

            var part2 = new int[program.Length];
            Array.Copy(program, part2, program.Length);

            part2 = IntcodeInterpreter.FindSentence(program, 19690720, 1, 2, 0, 99);
            Console.WriteLine("Output: " + string.Join(",", part2.Select(p => p.ToString())));
            Console.ReadLine();
        }
    }
}
