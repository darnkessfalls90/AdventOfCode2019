using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1202ProgramAlarm
{
    public class IntcodeInterpreter
    {
        public static int[] Interpret(int[] program)
        {
            var currentPosition = 0;
            while(currentPosition < program.Length)
            {
                switch (program[currentPosition])
                {
                    case 1:
                        program = Add(program[++currentPosition], 
                            program[++currentPosition], 
                            program[++currentPosition], program);
                        break;
                    case 2:
                        program = Multiply(program[++currentPosition],
                            program[++currentPosition],
                            program[++currentPosition], program);
                        break;
                    case 99:
                        currentPosition += program.Length;
                        break;
                    default:
                        throw new InvalidOperationException("At position " + currentPosition);
                }

                currentPosition++;
            }

            return program;
        }

        private static int[] Add(int pos1, int pos2, int result, int[] program)
        {
            program[result] = program[pos1] + program[pos2];
            return program;
        }

        private static int[] Multiply(int pos1, int pos2, int result, int[] program)
        {
            program[result] = program[pos1] * program[pos2];
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
                    test = Interpret(test);
                    if (test[0] == result)
                        return test;
                }
            }

            return program;
        }
    }
}
