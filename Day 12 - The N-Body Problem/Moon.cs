using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NBodyProblem
{
    public class Moon
    {
        public Coordinate Position { get; set; }
        public Coordinate Velocity { get; set; }

        public int CalculateEnergy()
        {
            return (Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z))
                * (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Moon) || obj is null) return false;
            var moon = (Moon)obj;

            return moon.Position == Position && moon.Velocity == Velocity;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ (Velocity.GetHashCode() * 31);
        }

        public override string ToString()
        {
            return "pos = <x=" + Position.X + ", y=" + Position.Y + ", z=" + Position.Z
                    + ">, vel = <x=" + Velocity.X + ", y=" + Velocity.Y + ", z=" + Velocity.Z + ">";
        }
    }

    public class Coordinate{
        public int X;
        public int Y;
        public int Z;

        public Coordinate(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate) || obj is null) return false;
            var coord = (Coordinate)obj;

            return coord.X == X && coord.Y == Y && coord.Z == Z;
        }

        public override int GetHashCode()
        {
            return (137 * X) + (149 * Y) + (163 * Z);
        }
    }
}
