using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetandForget
{
    public static class ListExtensions
    {
        public static int ListContains<T>(this List<T> source, List<T> search, int startIndex)
        {
            if (search.Count > source.Count - startIndex)
                return 0;

            return Enumerable.Range(startIndex, (source.Count - startIndex) - (search.Count + 1))
                .Select(a => source.Skip(a).Take(search.Count))
                .Count(a => a.SequenceEqual(search));
        }

        public static int IndexOf<T>(this List<T> sequence, List<T> pattern)
        {
            var patternLength = pattern.Count;
            var matchCount = 0;

            for (var i = 0; i < sequence.Count; i++)
            {
                if (sequence[i]!.Equals(pattern[matchCount]))
                {
                    matchCount++;
                    if (matchCount == patternLength)
                    {
                        return i - patternLength + 1;
                    }
                }
                else
                {
                    matchCount = 0;
                }
            }

            return -1;
        }

        public static List<T> RemoveSequences<T>(this List<T> source, List<T> sequence)
        {
            var index = IndexOf(source, sequence);
            while (index > -1)
            {
                source.RemoveRange(index, sequence.Count);
                index = IndexOf(source, sequence);
            }
            return source;
        }
    }
}
