using System;
using System.Collections.Generic;

namespace TheTyrannyOfTheRocketEquation
{
    class Part1
    {
        public static void Execute(List<int> MyModules, bool interactive)
        {
            long fuelForCurrentModules = 0;
            MyModules.ForEach(m => fuelForCurrentModules += CalculateFuelRequirement(m));
            Console.WriteLine("Current Module Weight: " + fuelForCurrentModules); 

            while (interactive)
            {
                Console.WriteLine("Enter Mass: ");
                var massString = Console.ReadLine();
                if (massString == "x")
                {
                    break;
                }
                if (!int.TryParse(massString, out int mass))
                {
                    Console.WriteLine("Invalid Input. Please enter a valid integer.");
                    continue;
                }

                var fuel = CalculateFuelRequirement(mass);
                Console.WriteLine("Fuel Requirement: " + fuel);
            }
        }

        public static int CalculateFuelRequirement(int mass)
        {
            return Convert.ToInt32(Math.Floor(mass / 3.0)) - 2;
        }
    }
}
