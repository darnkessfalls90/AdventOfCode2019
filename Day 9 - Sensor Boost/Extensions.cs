using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorBoost
{
    public static class Extensions
    {
        public static t[] DeepCopy<t>(this t[] toCopy)
        {
            var toReturn = new t[toCopy.Length];
            Array.Copy(toCopy, toReturn, toCopy.Length);
            return toReturn;
        }
    }
}
