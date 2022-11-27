using Intcode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TractorBeam
{
    public class AsyncIntcodeInterpreter : IDisposable
    {
        BackgroundWorker bg;
        Queue<long> output = new Queue<long>();
        Queue<int> input = new Queue<int>();
        private IntcodeInterpreter computer;

        public AsyncIntcodeInterpreter()
        {
            bg = new BackgroundWorker();
            bg.DoWork += BackgroundWorker_DoWork;

            computer = new IntcodeInterpreter();
            computer.TakeInput += Computer_TakeInput;
            computer.HandleOutput += Computer_HandleOutput;
        }

        public void Begin(long[] program)
        {
            while (bg.IsBusy)
                Thread.Sleep(10);
            bg.RunWorkerAsync( program );
        }

        public void Dispose()
        {
            if (bg != null)
                bg.Dispose();
        }

        public void GiveInput(int input)
        {
            this.input.Enqueue(input);
        }

        public long[] RecieveOutput(int legnth)
        {
            while (output.Count < legnth)
                Thread.Sleep(10);
            return output.DequeueMany(legnth).ToArray();
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var program = (dynamic)e.Argument as long[];
            computer.Interpret(ref program);
        }

        private void Computer_HandleOutput(long output, ref bool pause)
        {
            this.output.Enqueue(output);
        }

        private int? Computer_TakeInput()
        {
            while (this.input.Count == 0)
                Thread.Sleep(10);
            return this.input.Dequeue();
        }
    }

    public static class QueueExtender
    {
        public static IEnumerable<T> DequeueMany<T>(this Queue<T> queue, int length)
        {
            var list = new List<T>();
            for(int i = 0; i < length; i++)
            {
                list.Add(queue.Dequeue());
            }
            return list.AsEnumerable<T>();
        }
    }
}
