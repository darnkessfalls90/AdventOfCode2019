using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var min = 367479;
            var max = 893698;

            var numbers = Enumerable.Range(min, max - min);

            var potentialMatches = numbers.AsParallel()
                .Where(i => ContainsDuplicateAdjacent(i) && NeverDecreases(i));

            //potentialMatches.ToList().ForEach(i => Console.WriteLine(i));
            Console.WriteLine("Part 1 Number of matches: " + potentialMatches.Count());

            var potentialMatches2 = numbers//.AsParallel()
                .Where(i => ContainsDoubleOnly(i) && NeverDecreases(i));

            Console.WriteLine("Part 2 Number of matches: " + potentialMatches2.Count());

            Console.ReadLine();
        }

        private static bool ContainsDuplicateAdjacent(int number)
        {
            int lastDigit = 0;
            foreach (int digit in GetDigits(number))
            {
                if (lastDigit == digit)
                    return true;
                lastDigit = digit;
            }
            return false;
        }

        private static bool ContainsDoubleOnly(int number)
        {
            int lastDigit = -1;
            int tail = -1;
            int duble = -1;

            foreach (int digit in GetDigits(number))
            {
                if (lastDigit == digit && tail != digit && duble == -1) duble = digit;
                if (lastDigit == digit && digit == tail && duble == digit) duble = -1;

                tail = lastDigit;
                lastDigit = digit;
            }

            return duble != -1;
        }

        private static bool NeverDecreases(int number)
        {
            int lastDigit = 0;
            foreach (int digit in GetDigits(number))
            {
                if (lastDigit > digit)
                    return false;
                lastDigit = digit;
            }
            return true;
        }

        public static IEnumerable<int> GetDigits(int source)
        {
            int individualFactor = 0;
            int tennerFactor = Convert.ToInt32(Math.Pow(10, source.ToString().Length));
            do
            {
                source -= tennerFactor * individualFactor;
                tennerFactor /= 10;
                individualFactor = source / tennerFactor;

                yield return individualFactor;
            } while (tennerFactor > 1);
        }
    }
}
