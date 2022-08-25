using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OxygenSystem
{
    internal class DroidController : ICloneable
    {
        long[] program;
        IntcodeInterpreter computer;
        public Vector2 CurrentPosition { get; set; }
        public List<Vector2> Path { get; set; }
        public DroidController(long[] program, Vector2 currentPosition)
        {
            this.program = program;
            computer = new IntcodeInterpreter();
            computer.TakeInput += Computer_TakeInput;
            computer.HandleOutput += Computer_HandleOutput;

            CurrentPosition = currentPosition;
            Path = new List<Vector2>();
            Path.Add(currentPosition);
        }

        public Direction Direction { get; set; }
        public StatusCode Status { get; private set; }

        public void Run()
        {
            computer.Interpret(ref program);   
        }

        private void Computer_HandleOutput(long output, ref bool pause)
        {
            Status = (StatusCode)output;
            pause = true;
            if (Status != StatusCode.NoChange)
            {
                CurrentPosition += movements[Direction];
                Path.Add(CurrentPosition);
            }
        }

        private int? Computer_TakeInput()
        {
            return (int)Direction;
        }

        public object Clone()
        {
            return new DroidController((long[])program.Clone(), CurrentPosition) { Direction = Direction, Status = Status, Path = new List<Vector2>(Path) };
        }

        public static Dictionary<Direction, Vector2> movements = new Dictionary<Direction, Vector2>()
        {
            {Direction.North, new Vector2(0, 1) },
            {Direction.South, new Vector2(0, -1)},
            {Direction.East, new Vector2(1, 0)},
            {Direction.West, new Vector2(-1, 0) }
        };
    }

    public enum Direction
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4
    } 

    public enum StatusCode
    {
        NoChange = 0,
        Success = 1,
        OSFound = 2
    }
}
