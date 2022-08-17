using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace NBodyProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            var Io = new Moon()
            {
                Position = new Coordinate(-8, -10, 0),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Europa = new Moon()
            {
                Position = new Coordinate(5, 5, 10),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Ganymede = new Moon()
            {
                Position = new Coordinate(2, -7, 3),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Callisto = new Moon()
            {
                Position = new Coordinate(9, -8, -3),
                Velocity = new Coordinate(0, 0, 0)
            };

            var moons = new Moon[] { Io, Europa, Ganymede, Callisto };

            var states = new List<string>();
            
            int i = 0;
            while(!states.Contains(CalculateHash(moons)))
            {
                states.Add(CalculateHash(moons));

                CalculateVelocity(moons);
                ApplyVelocity(moons);

                if(i == 99)
                {
                    DisplayMoons(moons);
                    Console.WriteLine("Total Energy Part 1: " + moons.Sum(m => m.CalculateEnergy()));
                }

                i++;
            }

            DisplayMoons(moons);
            Console.WriteLine("Returns to previous state at: " + i + " steps");
            Console.ReadLine();
        }

        public static void CalculateVelocity(Moon[] moons)
        {
            for (int i = 0; i < moons.Length; i++)
            {
                for (int n = i; n < moons.Length; n++)
                {
                    if (i == n) continue;

                    var moon1 = moons[i];
                    var moon2 = moons[n];

                    if (moon1.Position.X > moon2.Position.X)
                    {
                        moon1.Velocity.X -= 1;
                        moon2.Velocity.X += 1;
                    }
                    else if (moon1.Position.X < moon2.Position.X)
                    {
                        moon1.Velocity.X += 1;
                        moon2.Velocity.X -= 1;
                    }

                    if (moon1.Position.Y > moon2.Position.Y)
                    {
                        moon1.Velocity.Y -= 1;
                        moon2.Velocity.Y += 1;
                    }
                    else if (moon1.Position.Y < moon2.Position.Y)
                    {
                        moon1.Velocity.Y += 1;
                        moon2.Velocity.Y -= 1;
                    }

                    if (moon1.Position.Z > moon2.Position.Z)
                    {
                        moon1.Velocity.Z -= 1;
                        moon2.Velocity.Z += 1;
                    }
                    else if (moon1.Position.Z < moon2.Position.Z)
                    {
                        moon1.Velocity.Z += 1;
                        moon2.Velocity.Z -= 1;
                    }
                }
            }
        }

        public static void ApplyVelocity(Moon[] moons)
        {
            foreach(Moon moon in moons)
            {
                moon.Position.X += moon.Velocity.X;
                moon.Position.Y += moon.Velocity.Y;
                moon.Position.Z += moon.Velocity.Z;
            }
        }

        public static void DisplayMoons(Moon[] moons)
        {
            foreach (Moon moon in moons)
            {
                Console.WriteLine("pos = <x=" + moon.Position.X + ", y=" + moon.Position.Y + ", z=" + moon.Position.Z
                    + ">, vel = <x=" + moon.Velocity.X + ", y=" + moon.Velocity.Y + ", z=" + moon.Velocity.Z + ">");
            }
        }

        public static string CalculateHash(Moon[] moons)
        {
            var hash = new StringBuilder();
            foreach(var moon in moons)
            {
                hash.Append(moon.ToString());
            }
            return hash.ToString();
        }
    }
}
