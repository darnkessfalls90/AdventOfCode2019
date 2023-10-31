using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategorySix
{
    public class IntcodeInterpreter
    {
        long currentPosition = 0;
        long relativeBase = 0;
        long[] program;

        public void Interpret(ref long[] program)
        {
            currentPosition = 0;
            while (currentPosition < program.Length)
            {
                long[] instruction = ReadInstruction((long)program[currentPosition++]);

                switch (instruction[0])
                {
                    case 1:
                        program = Add(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 2:
                        program = Multiply(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 3:
                        var input = OnTakeInput();
                        if (input == null) throw new NullReferenceException("Input was null");
                        program = Set((long)program[currentPosition++], GetMode(instruction, 1), input.Value, ref program);
                        break;
                    case 4:
                        if (OnHandleOutput(Get(program[currentPosition++], GetMode(instruction, 1), program)))
                            return;
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
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 8:
                        program = Equals(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 9:
                        relativeBase += Get(program[currentPosition++], GetMode(instruction, 1), program);
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

        public void InterpretByStep(long[] program)
        {
            this.program = program;
        }

        public void Step()
        {
            if (currentPosition < program.Length)
            {
                long[] instruction = ReadInstruction((long)program[currentPosition++]);

                switch (instruction[0])
                {
                    case 1:
                        program = Add(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 2:
                        program = Multiply(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 3:
                        var input = OnTakeInput();
                        if (input == null) throw new NullReferenceException("Input was null");
                        program = Set((long)program[currentPosition++], GetMode(instruction, 1), input.Value, ref program);
                        break;
                    case 4:
                        if (OnHandleOutput(Get(program[currentPosition++], GetMode(instruction, 1), program)))
                            return;
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
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 8:
                        program = Equals(program[currentPosition++], GetMode(instruction, 1),
                            program[currentPosition++], GetMode(instruction, 2),
                            (long)program[currentPosition++], GetMode(instruction, 3), ref program);
                        break;
                    case 9:
                        relativeBase += Get(program[currentPosition++], GetMode(instruction, 1), program);
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

        private long Get(long param, long mode, long[] program)
        {
            if (mode == 0)
            {
                if (param > program.Length)
                    return 0;
                return program[param];
            }
            if (mode == 2)
            {
                if (relativeBase + param >= program.Length)
                    return 0;
                return program[relativeBase + param];
            }
            else
                return param;
        }

        private long GetMode(long[] instruction, long curentInstruction)
        {
            if (instruction.Length - 1 < curentInstruction)
                return 0;
            return instruction[curentInstruction];
        }

        private long[] ReadInstruction(long instruction)
        {
            if (instruction < 100)
                return new long[] { instruction };

            var split = GetDigits(instruction);
            split[0] += split[1] * 10;
            split.RemoveAt(1);

            return split.ToArray();
        }

        public List<long> GetDigits(long source)
        {
            var digits = new List<long>();
            while (source > 0)
            {
                digits.Add(source % 10);
                source /= 10;
            }
            return digits;
        }

        private long[] Add(long param1, long mode1, long param2, long mode2, long result, long resultMode, ref long[] program)
        {
            Set(result, resultMode, Get(param1, mode1, program) + Get(param2, mode2, program), ref program);
            return program;
        }

        private long[] Multiply(long param1, long mode1, long param2, long mode2, long result, long resultMode, ref long[] program)
        {
            Set(result, resultMode, Get(param1, mode1, program) * Get(param2, mode2, program), ref program);
            return program;
        }

        private long[] Set(long pos1, long mode1, long input, ref long[] program)
        {
            if (mode1 == 2)
                pos1 += (long)relativeBase;

            if (pos1 >= program.Length)
                Array.Resize(ref program, (int)pos1 + 1);

            program[pos1] = input;
            return program;
        }

        private long JumpIfTrue(long param1, long mode1, long param2, long mode2, long currentPosition, long[] program)
        {
            if (Get(param1, mode1, program) > 0)
                return (long)Get(param2, mode2, program);
            return currentPosition;
        }

        private long JumpIfFalse(long param1, long mode1, long param2, long mode2, long currentPosition, long[] program)
        {
            if (Get(param1, mode1, program) == 0)
                return (long)Get(param2, mode2, program);
            return currentPosition;
        }

        private long[] LessThan(long param1, long mode1, long param2, long mode2, long result, long resultMode, ref long[] program)
        {
            if (Get(param1, mode1, program) < Get(param2, mode2, program))
                Set(result, resultMode, 1, ref program);
            else
                Set(result, resultMode, 0, ref program);

            return program;
        }

        private long[] Equals(long param1, long mode1, long param2, long mode2, long result, long resultMode, ref long[] program)
        {
            if (Get(param1, mode1, program) == Get(param2, mode2, program))
                Set(result, resultMode, 1, ref program);
            else
                Set(result, resultMode, 0, ref program);

            return program;
        }

        public long[] FindSentence(long[] program, long result, long pos1, long pos2, long min, long max)
        {
            for (long i = min; i <= max; i++)
            {
                for (long n = min; n <= max; n++)
                {
                    var test = new long[program.Length];
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

        public delegate long? TakeInputEventHandler();
        public event TakeInputEventHandler TakeInput;
        public virtual long? OnTakeInput()
        {
            return TakeInput?.Invoke();
        }

        public delegate void HandleOuptputEventHanlder(long output, ref bool pause);
        public event HandleOuptputEventHanlder HandleOutput;
        protected virtual bool OnHandleOutput(long output)
        {
            bool pause = false;
            HandleOutput?.Invoke(output, ref pause);
            return pause;
        }

        public delegate void HandleTerminationEventHandler();
        public event HandleTerminationEventHandler OnTerminate;
        protected virtual void OnHandleTerminate()
        {
            OnTerminate?.Invoke();
        }

        public static long[] ReadInput(string fileName)
        {
            var input = File.ReadAllText(fileName);
            var strings = input.Split(',');
            return strings.Select(n => long.Parse(n)).ToArray();
        }
    }
}
