using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetOfDiscord
{
    public class BugGrid
    {
        public bool[,] grid = new bool[5, 5];

        public BugGrid(bool[,] grid) { 
            this.grid= grid;
        }

        public BugGrid MoveAhead1Minute()
        {
            var newGrid = new bool[5,5];

            for (int i = 0; i < 5; i++)
                for (int n = 0; n < 5; n++)
                {
                    var bugs = GetAdjacentBugs(i, n);
                    if (grid[i, n])
                    {
                        if(bugs == 1)
                            newGrid[i, n] = true;
                        else
                            newGrid[i, n] = false;
                    }
                    else
                    {
                        if (bugs == 1 || bugs == 2)
                            newGrid[i, n] = true;
                        else
                            newGrid[i, n] = false;
                    }
                }

            return new BugGrid(newGrid);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            var power = 0;
            for (int i = 0; i < 5; i++)
                for (int n = 0; n < 5; n++)
                {
                    if (grid[i, n])
                        hash |= (int)Math.Pow(2, power);
                    power++;
                }
            return hash;
        }

        public int GetAdjacentBugs(int x, int y, bool skipMiddle = false)
        {
            var bugs = 0;
            if(x - 1 >= 0)
            {
                if (skipMiddle && x - 1 == 2 && y == 2)
                if (grid[x - 1, y]) bugs++;
            }
            if (y - 1 >= 0)
            {
                if (grid[x, y-1]) bugs++;
            }
            if(x + 1 < 5)
            {
                if (grid[x + 1, y]) bugs++;
            }
            if (y + 1 < 5)
            {
                if (grid[x, y+1]) bugs++;
            }
            return bugs;
        }

        public BugGrid DeepClone()
        {
            return new BugGrid(grid.Clone() as bool[,]);
        }
    }
}
