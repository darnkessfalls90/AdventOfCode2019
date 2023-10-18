using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace CategorySix
{
    public class CategorySix
    {
        static List<IntcodeInterpreter> computers = null;
        static Dictionary<long, Queue<long>> inputs = new Dictionary<long, Queue<long>>();
        static Dictionary<long, Queue<long>> outputs = new Dictionary<long, Queue<long>>();
        static int currentMachine = 0;
        static bool Stop = false;
        static bool idle = false;
        static long natX, natY;
        static long lastSentY;

        public static void Main(string[] args)
        {
            long[] program = IntcodeInterpreter.ReadInput("input.txt");
            computers = SetupComputers(50, program);
            var idleticks = 0;
            while (!Stop)
            {
                computers[currentMachine].Step();
                currentMachine = (currentMachine + 1) % 50;

                if (currentMachine == 49)
                {
                    if (idle)
                    {
                        idleticks++;
                        if (idleticks > 1250)
                        {
                            if (lastSentY == natY)
                            {
                                Console.WriteLine(natY);
                                Stop = true;
                                continue;
                            }

                            inputs[0].Enqueue(natX);
                            inputs[0].Enqueue(natY);

                            lastSentY = natY;

                            idleticks = 0;
                        }
                    }
                    else
                    {
                        idleticks = 0;
                    }
                    idle = true;
                }
            }
        }

        public static List<IntcodeInterpreter> SetupComputers(long size, long[] program)
        {
            List<IntcodeInterpreter> computers = new List<IntcodeInterpreter>();
            for (long i = 0; i < size; i++)
            {
                var computer = new IntcodeInterpreter();
                var newProgram = new long[program.Length];
                program.CopyTo(newProgram, 0);
                computer.InterpretByStep(newProgram);
                computers.Add(computer);
                inputs.Add(i, new Queue<long>());
                outputs.Add(i, new Queue<long>());
                inputs[i].Enqueue(i);
                computer.TakeInput += Computer_TakeInput;
                computer.HandleOutput += Computer_HandleOutput1; ;
            }

            inputs.Add(255, new Queue<long>());
            outputs.Add(255, new Queue<long>());
            return computers;
        }

        private static void Computer_HandleOutput1(long output, ref bool pause)
        {
            idle = false;
            outputs[currentMachine].Enqueue(output);
            if (outputs[currentMachine].Count >= 3)
            {
                var toMachine = outputs[currentMachine].Dequeue();
                var x = outputs[currentMachine].Dequeue();
                var y = outputs[currentMachine].Dequeue();
                //Console.WriteLine($"Sending package from {currentMachine} to {toMachine}: {x},{y}");

                if (toMachine == 255)
                {
                    Console.WriteLine($"Sending package from {currentMachine} to {toMachine}: {x},{y}");
                    natX = x;
                    natY = y;
                    //Stop = true;
                    return;
                }

                inputs[(long)toMachine].Enqueue(x);
                inputs[(long)toMachine].Enqueue(y);
            }
        }

        private static long? Computer_TakeInput()
        {
            if (inputs[currentMachine].Count == 0) return -1;
            var input = (long)inputs[currentMachine].Dequeue();
            idle = false;
            //Console.WriteLine($"{currentMachine} taking input {input}");
            return input;
        }
    }
}