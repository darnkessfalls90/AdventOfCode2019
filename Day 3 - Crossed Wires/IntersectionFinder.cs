using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CrossedWires
{
    public class IntersectionFinder
    {
        private static Dictionary<char, Func<List<Point>, int, List<Point>>> functions = new Dictionary<char, Func<List<Point>, int, List<Point>>>()
                {
                    {'U', Up },
            {'D', Down },
            {'L', Left },
            {'R', Right }
                };

        public static Point FindClosestPoint(string[] path1, string[] path2)
        {
            var drawnPath1 = DrawPath(path1);
            var drawnPath2 = DrawPath(path2);

            var intersections = drawnPath1.Intersect(drawnPath2).ToList();

            var distance = intersections.Where(p => p.X != 0 && p.Y != 0).Min(p => p.DistanceFromZero());

            return intersections.Where(p => p.DistanceFromZero().Equals(distance)).First();
        }

        public static int FindShortestStepsToIntersection(string[] path1, string[] path2)
        {
            var drawnPath1 = DrawPath(path1);
            var drawnPath2 = DrawPath(path2);

            var intersections = drawnPath1.Intersect(drawnPath2).ToList();
            int currentSteps = 0;
            foreach (var intersects in intersections)
            {
                if (intersects == new Point(0, 0))
                    continue;
                var stepsTo1 = drawnPath1.IndexOf(intersects);
                var stepsTo2 = drawnPath2.IndexOf(intersects);

                var totalSteps = stepsTo1 + stepsTo2;

                if (currentSteps == 0 || currentSteps > totalSteps)
                    currentSteps = totalSteps;
            }
            return currentSteps;
        }

        public static List<Point> DrawPath(string[] path1)
        {
            var path = new List<Point>();
            path.Add(new Point(0, 0));

            foreach (string direction in path1)
            {

                var move = direction[0];
                var amount = int.Parse(direction.Substring(1));

                path = functions[move](path, amount);
            }

            return path;
        }

        #region Functions

        private static List<Point> Up(List<Point> path, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var currentPoint = path[path.Count - 1];
                path.Add(new Point(currentPoint.X, currentPoint.Y + 1));
            }
            return path;
        }

        private static List<Point> Down(List<Point> path, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var currentPoint = path[path.Count - 1];
                path.Add(new Point(currentPoint.X, currentPoint.Y - 1));
            }
            return path;
        }

        private static List<Point> Left(List<Point> path, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var currentPoint = path[path.Count - 1];
                path.Add(new Point(currentPoint.X - 1, currentPoint.Y));
            }
            return path;
        }

        private static List<Point> Right(List<Point> path, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var currentPoint = path[path.Count - 1];
                path.Add(new Point(currentPoint.X + 1, currentPoint.Y));
            }
            return path;
        }

        #endregion
    }

    public static class PointExt
    {
        public static int DistanceFromZero(this Point p)
        {
            return Math.Abs(p.X) + Math.Abs(p.Y);
        }
    }

    class PointDistanceComparer : IComparer<Point>
    {
        public int Compare(Point x, Point y)
        {
            if (x.DistanceFromZero() == y.DistanceFromZero())
                return 0;
            if (x.DistanceFromZero() < y.DistanceFromZero())
                return -1;
            return 1;
        }
    }
}
