using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyWithAChanceCfAsteroids
{
    public class IntcodeInterpreter
    {
        public static int[] Interpret(ref int[] program, int[] inputs)
        {
            var currentPosition = 0;
            var inputPosition = 0;
            var outputs = new List<int>();
            while(currentPosition < program.Length)
            {
                int[] instruction = ReadInstruction(program[currentPosition++]);

                switch (instruction[0])
                {
                    case 1:
                        program = Add(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            program[currentPosition++], program);
                        break;
                    case 2:
                        program = Multiply(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            program[currentPosition++], program);
                        break;
                    case 3:
                        program = Set(program[currentPosition++], inputs[inputPosition++], program);
                        break;
                    case 4:
                        outputs.Add(Get(program[currentPosition++], GetMode(instruction, 1), program));
                        break;
                    case 5:
                        currentPosition = JumpIfTrue(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2), currentPosition, program);
                        break;
                    case 6:
                        currentPosition = JumpIfFalse(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2), currentPosition, program);
                        break;
                    case 7:
                        program = LessThan(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            program[currentPosition++], program);
                        break;
                    case 8:
                        program = Equals(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            program[currentPosition++], program);
                        break;
                    case 99:
                        currentPosition += program.Length;
                        break;
                    default:
                        throw new InvalidOperationException("At position " + currentPosition);
                }
            }

            return outputs.ToArray();
        }

        private static int Get(int param, int mode, int[] program)
        {
            if (mode == 0)
                return program[param];
            else
                return param;
        }

        private static int GetMode(int[] instruction, int curentInstruction)
        {
            if (instruction.Length - 1 < curentInstruction)
                return 0;
            return instruction[curentInstruction];
        }

        private static int[] ReadInstruction(int instruction)
        {
            if (instruction < 100)
                return new int[] { instruction };

            var split = GetDigits(instruction);
            split[0] += split[1] * 10;
            split.RemoveAt(1);

            return split.ToArray();
        }

        public static List<int> GetDigits(int source)
        {
            var digits = new List<int>();
            while(source > 0)
            {
                digits.Add(source % 10);
                source /= 10;
            }
            return digits;
        }

        private static int[] Add(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            program[result] = Get(param1, mode1, program) + Get(param2, mode2, program);
            return program;
        }

        private static int[] Multiply(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            program[result] = Get(param1, mode1, program) * Get(param2, mode2, program);
            return program;
        }

        private static int[] Set(int pos1, int input,  int[] program)
        {
            program[pos1] = input;
            return program;
        }

        private static int JumpIfTrue(int param1, int mode1, int param2, int mode2, int currentPosition, int[] program)
        {
            if (Get(param1, mode1, program) > 0)
                return Get(param2, mode2, program);
            return currentPosition;
        }

        private static int JumpIfFalse(int param1, int mode1, int param2, int mode2, int currentPosition, int[] program)
        {
            if (Get(param1, mode1, program) == 0)
                return Get(param2, mode2, program);
            return currentPosition;
        }

        private static int[] LessThan(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            if (Get(param1, mode1, program) < Get(param2, mode2, program))
                program[result] = 1;
            else
                program[result] = 0;

            return program;
        }

        private static int[] Equals(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            if (Get(param1, mode1, program) == Get(param2, mode2, program))
                program[result] = 1;
            else
                program[result] = 0;

            return program;
        }

        public static int[] FindSentence(int[] program, int result, int pos1, int pos2, int min, int max)
        {
            for(int i = min; i <= max; i++)
            {
                for ( int n = min; n <= max; n++)
                {
                    var test = new int[program.Length];
                    Array.Copy(program, test, program.Length);
                    test[pos1] = i;
                    test[pos2] = n;
                    Interpret(ref test, new int[0]);
                    if (test[0] == result)
                        return test;
                }
            }

            return program;
        }
    }
}
