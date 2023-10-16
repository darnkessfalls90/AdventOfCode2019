using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SlamShuffle
{
    public class SlamShuffle
    {
        static string dealCommand = "deal with increment ";
        static string stackCommand = "deal into new stack";
        static string cutCommand = "cut ";

        public static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            Console.WriteLine("Part 1: " + GetPositionAfterShuffle(2019, 10007, input));
            Part2(input);
            Console.ReadLine();
        }

        public static void Part2(string[] inputs)
        {
            BigInteger size = 119315717514047;
            BigInteger iterations = 101741582076661;
            BigInteger position = 2020;
            BigInteger offsetDif = 0;
            BigInteger incrementMultiplier = 1;

            foreach (var input in inputs)
            {
                RunCalculation(ref incrementMultiplier, ref offsetDif, size, input);
            }
            (BigInteger increment, BigInteger offset) = getSequence(iterations, incrementMultiplier, offsetDif, size);
            var finalPos = (offset + position * increment) % size;
            Console.WriteLine("Part 2: " + finalPos);
        }

        public static void RunCalculation(ref BigInteger incrementMultiplier, ref BigInteger offsetDifference, BigInteger size, string line)
        {
            if (line.StartsWith(dealCommand))
            {
                var offset = new string(line.Skip(dealCommand.Length).ToArray());
                var offsetInt = int.Parse(offset);
                var bigOffset = new BigInteger(offsetInt);
                incrementMultiplier *= BigInteger.ModPow(bigOffset, size - 2, size);
            }
            else if (line.StartsWith(stackCommand))
            {
                incrementMultiplier *= -1;
                offsetDifference += incrementMultiplier;
            }
            else if (line.StartsWith(cutCommand))
            {
                var offset = new string(line.Skip(cutCommand.Length).ToArray());
                var offsetInt = int.Parse(offset);
                offsetDifference += offsetInt * incrementMultiplier;
            }

            incrementMultiplier = BigMod(incrementMultiplier, size);
            offsetDifference = BigMod(offsetDifference, size);
        }

        public static long NewPositionFromDeal(long currentPosition, long offset, long deckSize)
        {
            return (currentPosition * offset) % deckSize;
        }

        public static long NewPositionFromNewPile(long currentPostion, long decksize)
        {
            return decksize - currentPostion - 1;
        }

        public static long NewPositionFromCut(long currentPostion, long offset, long deckSize)
        {
            return (long)nfmod((currentPostion - offset), deckSize);
        }

        public static long GetPositionAfterShuffle(long number, long deckSize, string[] input)
        {
            var currentPosition = number;
            foreach (var line in input)
            {
                if (line.StartsWith(dealCommand))
                {
                    var offset = new string(line.Skip(dealCommand.Length).ToArray());
                    var offsetInt = int.Parse(offset);
                    currentPosition = NewPositionFromDeal(currentPosition, offsetInt, deckSize);
                }
                else if (line.StartsWith(stackCommand))
                {
                    currentPosition = NewPositionFromNewPile(currentPosition, deckSize);
                }
                else if (line.StartsWith(cutCommand))
                {
                    var offset = new string(line.Skip(cutCommand.Length).ToArray());
                    var offsetInt = int.Parse(offset);
                    currentPosition = NewPositionFromCut(currentPosition, offsetInt, deckSize);
                }
            }
            return currentPosition;
        }

        static double nfmod(double a, double b)
        {
            return a - b * Math.Floor(a / b);
        }

        static BigInteger BigMod(BigInteger a, BigInteger b)
        {
            return (a % b + b) % b;
        }

        static (BigInteger increment, BigInteger offset) getSequence(BigInteger iterations, BigInteger incrementMultiplier, BigInteger offsetDifference, BigInteger size)
        {
            var increment = BigInteger.ModPow(incrementMultiplier, iterations, size);
            var temp = (1 - incrementMultiplier) % size;

            var offset = offsetDifference * (1 - increment) * Inversion(temp, size);
            offset = BigMod(offset, size);
            
            return (increment, offset);
        }

        static BigInteger Inversion(BigInteger a, BigInteger b)
        {
            return BigInteger.ModPow(a, b - 2, b);
        }
    }
}