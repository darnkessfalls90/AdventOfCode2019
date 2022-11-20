using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ManyWorldsInterpretation
{
    public class State
    {
        public int CollectedKeys { get; set; }
        public long Length { get; set; }

        public bool HasKey(char c)
        {
            return (CollectedKeys & KeyValue(c)) > 0;
        }

        public void AddKey(char c)
        {
            CollectedKeys |= KeyValue(c);
        }

        public static int KeyValue(char c)
        {
            return 1 << (c - 'a');
        }

    }

    public class FillState : State, ICloneable
    {
        public List<char> DoorsBlocking { get; set; } = new List<char>();
        public Vector3 Postion { get; set; }
        public List<Vector3> Path { get; set; }

        public FillState(int collectedKeys, Vector3 postion, long length)
        {
            CollectedKeys = collectedKeys;
            Postion = postion;
            Length = length;
            Path = new List<Vector3>() { postion };
        }   

        

        public override int GetHashCode()
        {
            return CollectedKeys ^ Postion.GetHashCode();
        }

        public object Clone()
        {
            return new FillState(CollectedKeys, Postion, Length)
            { Path = new List<Vector3>(Path), DoorsBlocking = new List<char>(DoorsBlocking)};
        }
    }

    public class WalkState : State, ICloneable
    {
        public Dictionary<char, Vector3> Postions { get; set; }

        private WalkState()
        {
            CollectedKeys = 0;
            Postions = new Dictionary<char, Vector3>();
        }

        public WalkState(int collectedKeys, Dictionary<char, Vector3> postion, long length)
        {
            CollectedKeys = collectedKeys;
            Postions = postion;
            Length = length;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(WalkState)) return false;

            WalkState other = (WalkState)obj;

            return other.CollectedKeys == CollectedKeys
                && other.Postions.Except(Postions).Count() == 0
                && Postions.Except(other.Postions).Count() == 0;
        }

        public override int GetHashCode()
        {
            return CollectedKeys ^ Postions.GetHashCode();
        }

        public object Clone()
        {
            return new WalkState(CollectedKeys, Postions, Length);
        }
    }
}
