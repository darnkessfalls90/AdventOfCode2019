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
                Position = new Coordinate(8, 0, 8),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Europa = new Moon()
            {
                Position = new Coordinate(0, -5, -10),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Ganymede = new Moon()
            {
                Position = new Coordinate(16, 10, -5),
                Velocity = new Coordinate(0, 0, 0)
            };

            var Callisto = new Moon()
            {
                Position = new Coordinate(19, -10, -7),
                Velocity = new Coordinate(0, 0, 0)
            };

            var moons = new Moon[] { Io, Europa, Ganymede, Callisto };

            int i = 0;
            int xstate = -1, ystate = -1, zstate = -1;
            while(xstate == -1 || ystate == -1 || zstate == -1)
            {
                CalculateVelocity(moons);
                ApplyVelocity(moons);

                if(i == 999)
                {
                    DisplayMoons(moons);
                    Console.WriteLine("Total Energy Part 1: " + moons.Sum(m => m.CalculateEnergy()));
                }

                i++;

                if (xstate == -1 && moons.All(m => m.Velocity.X == 0))
                    xstate = i;
                if (ystate == -1 && moons.All(m => m.Velocity.Y == 0))
                    ystate = i;
                if (zstate == -1 && moons.All(m => m.Velocity.Z == 0))
                    zstate = i;

                
            }


            long foo = lcm(xstate, ystate, zstate) * 2;
            Console.WriteLine("Returns to previous state at: " + foo + " steps");
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
                Console.WriteLine(moon.ToString());
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

        static long gcf(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long lcm(int a, long b)
        {
            return (a / gcf(a, b)) * b;
        }

        static long lcm(int a, int b, int c)
        {
            return lcm(a, lcm(b, (long)c));
        }
    }
}
