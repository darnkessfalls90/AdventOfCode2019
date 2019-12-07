using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalOrbitMap
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = Path.Combine(Directory.GetCurrentDirectory(), "Input.txt");
            var inputs = File.ReadLines(inputFile);
            var orbitMap = SpaceObject.MapOrbits(inputs.ToArray());
            var orbits = orbitMap.CountOrbits(0);
            Console.WriteLine("Number of Orbits: " + orbits);

            orbitMap = orbitMap.Find("YOU");
            var steps = orbitMap.ShortestStepsFromParentToParentOf("SAN");
            Console.WriteLine("Shortest Steps to Santa: " + steps);

            Console.ReadLine();
        }
    }
}
