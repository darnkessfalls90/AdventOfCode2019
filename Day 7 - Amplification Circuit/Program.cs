using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmplificationCircuit
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new int[] { 3, 8, 1001, 8, 10, 8, 105, 1, 0, 0, 21, 34, 47, 72, 81, 94, 175, 256, 337, 418, 99999, 3, 9, 102, 3, 9, 9, 1001, 9, 3, 9, 4, 9, 99, 3, 9, 101, 4, 9, 9, 1002, 9, 5, 9, 4, 9, 99, 3, 9, 1001, 9, 5, 9, 1002, 9, 5, 9, 1001, 9, 2, 9, 1002, 9, 5, 9, 101, 5, 9, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 1001, 9, 4, 9, 102, 4, 9, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 99, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 99 };
            var ampSet = new AmplifierSet();
            var maxSignal = FindMaxOutput(program, new int[] { 4, 3, 2, 1, 0 }, ampSet, false);
            Console.WriteLine("Max Output Part 1: ");
            Console.WriteLine(maxSignal);

            maxSignal = FindMaxOutput(program, new int[] { 9, 7, 8, 5, 6 }, ampSet, true);
            Console.WriteLine("Max Output Part 2: ");
            Console.WriteLine(maxSignal);
            Console.ReadLine();
        }

        public static int FindMaxOutput(int[] program, int[] inputSignals, AmplifierSet set, bool feedback)
        {
            int maxOutput = -1;
            int[] maxSignal = new int[1] { -1 };

            var combinations = new List<int[]>();
            combinations.Add(inputSignals);
            FindCombinations(inputSignals, 0, ref combinations);
            foreach (var signal in combinations)
            {
                //signal.ToList().ForEach(item => Console.Write(item));
                //Console.WriteLine();
                set.FeedbackLoop = feedback;

                var output = 0;
                try
                {
                    output = set.GetOutputSignal(program, signal, 0, 5);
                }
                catch (StackOverflowException)
                {
                    Console.Write("StackOverflow: ");
                    set.ForEach(i => Console.Write(i));
                    Console.WriteLine();
                }
                //sConsole.WriteLine(output);
                if (output > maxOutput)
                {
                    maxSignal = signal;
                    maxOutput = output;
                }
            }
            return maxOutput;
        }

        public static void FindCombinations(int[] list, int position, ref List<int[]> combinations)
        {
            if (position == list.Count()) return;

            for (int i = 0; i < list.Count(); i++)
            {
                if (position == i) continue;
                var tempList = list.DeepCopy();
                Swap(ref tempList[i], ref tempList[position]);

                if (!combinations.Any(a => a.SequenceEqual(tempList)))
                    combinations.Add(tempList);

                FindCombinations(tempList, position + 1, ref combinations);
            };
        }

        public static void Swap(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
