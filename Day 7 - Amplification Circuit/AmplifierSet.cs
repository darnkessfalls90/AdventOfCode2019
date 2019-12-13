using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmplificationCircuit
{
    public class AmplifierSet : List<Amplifier>
    {
        public bool FeedbackLoop { get; set; }
        bool terminate;
        public AmplifierSet()
        {
            
        }

        private void Amp_OnTerminate()
        {
            terminate = true;
        }

        public int GetOutputSignal(int[] program, int[] phaseSettings, int inputSignal, int NumOfAmps)
        {
            this.Clear();
            terminate = false;
            for (var i = 0; i < NumOfAmps; i++)
            {
                var amp = new Amplifier(program.DeepCopy(), i);
                amp.OnTerminate += Amp_OnTerminate;
                Add(amp);
            }

            for (int i = 0; i < Count; i++)
            {
                var amp = this[i];
                inputSignal = amp.GetOutputSignal(phaseSettings[i], inputSignal);

                if (i == this.Count -1 && FeedbackLoop && !terminate)
                    i = -1;
            }
            return inputSignal;
        }
    }

    public class Amplifier : IntcodeInterpreter
    {
        int[] program;
        int currentInput;
        int output;
        public int NumberInSeries { get; set; }
        private readonly int[] parameters = new int[2];

        public Amplifier(int[] program, int NumberInSeries)
        {
            this.program = program;
        }

        public int GetOutputSignal(int phaseSetting, int inputSignal)
        {
            parameters[0] = phaseSetting;
            parameters[1] = inputSignal;
            Interpret(ref program);
            return output;
        }

        protected override void OnHandleOutput(int output)
        {
            this.output = output;
            base.OnHandleOutput(output);
        }

        public override int? OnTakeInput()
        {
            var param = currentInput == 0 ? currentInput++ : 1;
            return parameters[param];
        }
    }

    public static class Extension
    {
        public static t[] DeepCopy<t>(this t[] toCopy)
        {
            var toReturn = new t[toCopy.Length];
            Array.Copy(toCopy, toReturn, toCopy.Length);
            return toReturn;
        }
    }
}
