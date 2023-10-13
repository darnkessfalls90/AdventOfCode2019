using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpringdroidAdventure
{
    public class SpringdroidAdventure
    {
        static char[] mapItems = new char[] { '#', '.', '#', (char)10 };
        static long damage;
        static string inputStringPart1 = @"NOT A T
OR T J
NOT B T
OR T J
NOT C T
OR T J
NOT D T
NOT T T
AND T J
WALK
";

        static string inputStringPart2 = @"NOT A J
NOT B T
OR T J
NOT C T
OR T J
AND D J
NOT E T
NOT T T
OR H T
AND T J
RUN
";
        static int inputCounter = 0;
        public static void Main(string[] args)
        {
            var fileInput = File.ReadAllText("input.txt");
            var input = ReadInput(fileInput);

            RunPart1(input);
            RunPart2(input);
        }

        static void RunPart1(long[] input)
        {
            inputCounter = 0;
            inputStringPart1 = inputStringPart1.Replace(Environment.NewLine, "" + (char)10);

            var computer = new IntcodeInterpreter();
            computer.HandleOutput += Computer_HandleOutput;
            computer.TakeInput += Computer_TakeInputPart1;

            var inputClone = input.Clone() as long[];
            computer.Interpret(ref inputClone);

            Console.WriteLine("Damage Taken Part 1: " + damage);
        }

        static void RunPart2(long[] input)
        {
            inputCounter = 0;
            inputStringPart2 = inputStringPart2.Replace(Environment.NewLine, "" + (char)10);

            var computer = new IntcodeInterpreter();
            computer.HandleOutput += Computer_HandleOutput;
            computer.TakeInput += Computer_TakeInputPart2;

            var inputClone = input.Clone() as long[];
            computer.Interpret(ref inputClone);

            Console.WriteLine("Damage Taken Part 2: " + damage);
        }

        private static int? Computer_TakeInputPart1()
        {
            var input = inputStringPart1[inputCounter++];
            return input;
        }

        private static int? Computer_TakeInputPart2()
        {
            var input = inputStringPart2[inputCounter++];
            return input;
        }

        private static void Computer_HandleOutput(long output, ref bool pause)
        {
            if(output < 128)
            {
                Console.Write(Convert.ToChar(output));
            }
            else
            {
                damage = output;
            }
        }

        static long[] ReadInput(string input)
        {
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }
    }
}