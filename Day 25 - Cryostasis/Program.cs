using CategorySix;

namespace Cryostasis
{
    public class Cryostasis
    {
        public static void Main(string[] args)
        {
            var program = IntcodeInterpreter.ReadInput("input.txt");
            var computer = new IntcodeInterpreter();
            computer.TakeInput += Computer_TakeInput;
            computer.HandleOutput += Computer_HandleOutput;
            computer.Interpret(ref program);
        }

        private static void Computer_HandleOutput(long output, ref bool pause)
        {
            Console.Write((char)output);
        }

        private static long? Computer_TakeInput()
        {
            var car = Console.Read();
            if(car == 13)
                car = Console.Read();
            return car;
        }
    }
}