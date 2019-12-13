using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmplificationCircuit
{
    public class IntcodeInterpreter
    {
        int currentPosition = 0;

        public void Interpret(ref int[] program)
        {
            while (currentPosition < program.Length)
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
                        var input = OnTakeInput();
                        if (input == null) throw new NullReferenceException("Input was null");
                        program = Set(program[currentPosition++], input.Value, program);
                        break;
                    case 4:
                        OnHandleOutput(Get(program[currentPosition++], GetMode(instruction, 1), program));
                        return;
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
                        OnHandleTerminate();
                        break;
                    default:
                        throw new InvalidOperationException("At position " + currentPosition);
                }
            }
        }

        private int Get(int param, int mode, int[] program)
        {
            if (mode == 0)
                return program[param];
            else
                return param;
        }

        private int GetMode(int[] instruction, int curentInstruction)
        {
            if (instruction.Length - 1 < curentInstruction)
                return 0;
            return instruction[curentInstruction];
        }

        private int[] ReadInstruction(int instruction)
        {
            if (instruction < 100)
                return new int[] { instruction };

            var split = GetDigits(instruction);
            split[0] += split[1] * 10;
            split.RemoveAt(1);

            return split.ToArray();
        }

        public List<int> GetDigits(int source)
        {
            var digits = new List<int>();
            while (source > 0)
            {
                digits.Add(source % 10);
                source /= 10;
            }
            return digits;
        }

        private int[] Add(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            program[result] = Get(param1, mode1, program) + Get(param2, mode2, program);
            return program;
        }

        private int[] Multiply(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            program[result] = Get(param1, mode1, program) * Get(param2, mode2, program);
            return program;
        }

        private int[] Set(int pos1, int input, int[] program)
        {
            program[pos1] = input;
            return program;
        }

        private int JumpIfTrue(int param1, int mode1, int param2, int mode2, int currentPosition, int[] program)
        {
            if (Get(param1, mode1, program) > 0)
                return Get(param2, mode2, program);
            return currentPosition;
        }

        private int JumpIfFalse(int param1, int mode1, int param2, int mode2, int currentPosition, int[] program)
        {
            if (Get(param1, mode1, program) == 0)
                return Get(param2, mode2, program);
            return currentPosition;
        }

        private int[] LessThan(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            if (Get(param1, mode1, program) < Get(param2, mode2, program))
                program[result] = 1;
            else
                program[result] = 0;

            return program;
        }

        private int[] Equals(int param1, int mode1, int param2, int mode2, int result, int[] program)
        {
            if (Get(param1, mode1, program) == Get(param2, mode2, program))
                program[result] = 1;
            else
                program[result] = 0;

            return program;
        }

        public int[] FindSentence(int[] program, int result, int pos1, int pos2, int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                for (int n = min; n <= max; n++)
                {
                    var test = new int[program.Length];
                    Array.Copy(program, test, program.Length);
                    test[pos1] = i;
                    test[pos2] = n;
                    Interpret(ref test);
                    if (test[0] == result)
                        return test;
                }
            }

            return program;
        }

        public delegate int? TakeInputEventHandler();
        public event TakeInputEventHandler TakeInput;
        public virtual int? OnTakeInput()
        {
            return TakeInput?.Invoke();
        }

        public delegate void HandleOuptputEventHanlder(int output);
        public event HandleOuptputEventHanlder HandleOutput;
        protected virtual void OnHandleOutput(int output)
        {
            HandleOutput?.Invoke(output);
        }

        public delegate void HandleTerminationEventHandler();
        public event HandleTerminationEventHandler OnTerminate;
        protected virtual void OnHandleTerminate()
        {
            OnTerminate?.Invoke();
        } 
    }
}
