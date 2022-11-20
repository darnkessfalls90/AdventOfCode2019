using ManyWorldsInterpretation;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ManyWorldsInterpretation
{
    public class Program
    {
        //public static List<Vector3> map;
        public static long shortestPath = int.MaxValue;
        static Dictionary<char, List<KeyLength>> paths = new Dictionary<char, List<KeyLength>>();
        static Vector3[] Keys;
        static int AllKeys = 0;

        public static Vector2[] movements = new Vector2[]
        {
            (new Vector2(1, 0)),
            (new Vector2(0, 1)),
            (new Vector2(-1, 0)),
            (new Vector2(0, -1)),
        };

        public static void Main(string[] args)
        {
            var fileInput = File.ReadAllLines("input.txt");
            var map = ReadInput(fileInput);

            Keys = map.Where(v => IsKey(v.Z)).ToArray();

            foreach (var key in Keys)
            {
                AllKeys |= FillState.KeyValue((char)key.Z);
            }

            var start = map.First(m => m.Z == '@');

            var watch = Stopwatch.StartNew();

            FillPaths(map, Keys, new List<Vector3>() { start });

            watch.Stop();
            //PrintPaths();
            Console.WriteLine("Fill Paths Elapsed Time: " + (watch.ElapsedMilliseconds / 60000.0));

            watch.Restart();
            WalkPaths(new List<WalkState>() { new WalkState(0, new Dictionary<char, Vector3>() { { '@', start } }, 0) });
            watch.Stop();

            Console.WriteLine("Walk Paths Elapsed Time: " + (watch.ElapsedMilliseconds / 60000.0));

            Console.WriteLine("Shortest Path: " + shortestPath);

            map = UpdateMapForPart2(map);
            shortestPath = int.MaxValue;
            //DrawMap(map);

            var starts = map.Where(m => m.Z >= '1' && m.Z <= '9');

            watch.Restart();
            FillPaths(map, Keys, starts.ToList());
            watch.Stop();

            Console.WriteLine("Fill Paths Part 2 Elapsed Time: " + (watch.ElapsedMilliseconds / 60000.0));

            var positions = new Dictionary<char, Vector3>();
            foreach (var s in starts)
            {
                positions.Add((char)s.Z, s);
            }
            watch.Restart();
            WalkPaths(new List<WalkState>() { new WalkState(0, positions, 0) });
            watch.Stop();

            Console.WriteLine("Walk Paths Part 2 Elapsed Time: " + (watch.ElapsedMilliseconds / 60000.0));

            Console.WriteLine("Shortest Path: " + shortestPath);

            Console.ReadLine();
        }

        public static List<Vector3> UpdateMapForPart2(List<Vector3> map)
        {
            var start = map.First(m => m.Z == '@');
            map.Remove(start);
            start.Z = '#';
            map.Add(start);

            var startup = map.FirstOrDefault(m => m.X == start.X && m.Y == start.Y - 1);
            map.Remove(startup);
            startup.Z = '#';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X && m.Y == start.Y + 1);
            map.Remove(startup);
            startup.Z = '#';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X - 1 && m.Y == start.Y);
            map.Remove(startup);
            startup.Z = '#';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X + 1 && m.Y == start.Y);
            map.Remove(startup);
            startup.Z = '#';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X - 1 && m.Y == start.Y - 1);
            map.Remove(startup);
            startup.Z = '1';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X - 1 && m.Y == start.Y + 1);
            map.Remove(startup);
            startup.Z = '2';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X + 1 && m.Y == start.Y - 1);
            map.Remove(startup);
            startup.Z = '3';
            map.Add(startup);

            startup = map.FirstOrDefault(m => m.X == start.X + 1 && m.Y == start.Y + 1);
            map.Remove(startup);
            startup.Z = '4';
            map.Add(startup);

            return map;
        }

        public static void FillPaths(List<Vector3> map, Vector3[] keys, List<Vector3> starts)
        {
            paths.Clear();
            foreach (var start in starts)
            {
                var starterKeys = new List<KeyLength>();
                //FindKeys(map, new State(0, start, null, 0), new KeyLength(), starterKeys);
                FindKeys_BFS(map, new List<FillState>() { new FillState(0, start, 0) }, starterKeys);
                paths.Add((char)start.Z, starterKeys);
                foreach (KeyLength c in starterKeys)
                {
                    var state = new FillState(FillState.KeyValue(c.Key), map.First(m => m.Z == c.Key), 0);
                    var foundPaths = paths.Where(p => p.Value.Any(k => k.Key == c.Key) && p.Key != start.Z);
                    var keyLengths = new List<KeyLength>();
                    if (foundPaths != null)
                    {
                        foreach (var path in foundPaths)
                        {
                            var newKeyLength = new KeyLength();
                            var existing = path.Value.First(d => d.Key == c.Key && d.Key != start.Z);
                            newKeyLength.Key = path.Key;
                            newKeyLength.Distance = existing.Distance;
                            newKeyLength.DoorsBlocking = existing.DoorsBlocking;

                            state.AddKey(existing.Key);
                            keyLengths.Add(newKeyLength);
                        }
                    }

                    //FindKeys(map, state, new KeyLength(), keyLengths);
                    FindKeys_BFS(map, new List<FillState>() { state }, keyLengths);
                    if (!paths.ContainsKey(c.Key))
                        paths.Add(c.Key, keyLengths);
                    else
                        paths[c.Key].AddRange(keyLengths);
                }
            }
        }

        static void WalkPaths(List<WalkState> currentStates)
        {
            List<WalkState> toCheck = new List<WalkState>();
            foreach (WalkState currentState in currentStates)
            {
                foreach (var position in currentState.Postions)
                {
                    var currentPaths = paths[(char)position.Value.Z]
                        .Where(c => !currentState.HasKey(c.Key));

                    foreach (var path in currentPaths)
                    {
                        if (path.DoorsBlocking.Any(a => !currentState.HasKey(char.ToLower(a)))) continue;

                        var newPositions = new Dictionary<char, Vector3>(currentState.Postions);
                        var newPosition = Keys.First(a => a.Z == path.Key);
                        newPositions[position.Key] = newPosition;

                        var state = new WalkState(currentState.CollectedKeys, newPositions
                           , path.Distance + currentState.Length);

                        if (!(newPosition.Z == position.Key))
                            state.AddKey((char)newPosition.Z);

                        if (state.Length >= shortestPath) continue;

                        if (state.CollectedKeys == AllKeys)
                        {
                            var length = state.Length;

                            if (shortestPath > length)
                            {
                                shortestPath = length;
                            }
                            continue;
                        }                   

                        if (toCheck.Any(s => s.Equals(state)))
                        {
                            var otherStates = toCheck.Where(c => c.Equals(state)).ToList();
                            otherStates.Add(state);
                            toCheck = toCheck.Except(otherStates).ToList();
                            var statesMin = otherStates.Min(t => t.Length);
                            toCheck.Add(otherStates.Where(s => s.Length == statesMin).First());
                        }
                        else
                            toCheck.Add(state);
                    }
                }
            }

            //toCheck = toCheck.GroupBy(x => x).SelectMany(s => s.Where(t => t.Length == s.Min(q => q.Length))).ToList();

            if (toCheck.Any())
                WalkPaths(toCheck);
        }

        static List<Vector3> ReadInput(string[] input)
        {
            var map = new List<Vector3>();
            int x = 0;
            int y = 0;

            foreach (string line in input)
            {
                foreach (char c in line)
                {
                    map.Add(new Vector3(x, y, (int)c));
                    x++;
                }
                y++;
                x = 0;
            }

            return map;
        }

        static void FindKeys(List<Vector3> map, FillState currentState, KeyLength currentKeyLength, List<KeyLength> allKeyLegths)
        {
            currentState.Path.Add(currentState.Postion);

            if (IsKey(currentState.Postion.Z) && !currentState.HasKey((char)currentState.Postion.Z))
            {
                currentKeyLength.Key = (char)currentState.Postion.Z;
                currentKeyLength.Distance = currentState.Length;
                if (allKeyLegths.Any(c => c.Key == currentKeyLength.Key))
                {
                    var other = allKeyLegths.First(c => c.Key == currentKeyLength.Key);
                    if (currentKeyLength.Distance < other.Distance)
                    {
                        allKeyLegths.Remove(other);
                        allKeyLegths.Add(currentKeyLength);
                    }
                }
                else
                {
                    allKeyLegths.Add(currentKeyLength);
                }
            }

            if (IsDoor(currentState.Postion.Z))
            {
                currentKeyLength.DoorsBlocking.Add((char)currentState.Postion.Z);
            }

            foreach (var movement in movements)
            {
                var newPosition = map.First(m => m.X == currentState.Postion.X + movement.X
                                        && m.Y == currentState.Postion.Y + movement.Y);

                if (newPosition.Z == (int)'#') continue;
                if (currentState.Path.Contains(newPosition)) continue;

                var state = currentState.Clone() as FillState;

                state.Postion = newPosition;

                state.Length++;

                FindKeys(map, state, new KeyLength() { DoorsBlocking = new List<char>(currentKeyLength.DoorsBlocking) }, allKeyLegths);
            }
        }

        static void FindKeys_BFS(List<Vector3> map, List<FillState> currentStates, List<KeyLength> allKeyLegths)
        {
            var toCheck = new List<FillState>();
            foreach (var currentState in currentStates)
            {
                currentState.Path.Add(currentState.Postion);

                if (IsKey(currentState.Postion.Z) && !currentState.HasKey((char)currentState.Postion.Z))
                {
                    if (allKeyLegths.Any(c => c.Key == (char)currentState.Postion.Z))
                    {
                        var other = allKeyLegths.First(c => c.Key == (char)currentState.Postion.Z);
                        if (currentState.Length < other.Distance)
                        {
                            other.Distance = currentState.Length;
                            other.DoorsBlocking = currentState.DoorsBlocking;
                        }
                    }
                    else
                    {
                        var keyLenght = new KeyLength();
                        keyLenght.Key = (char)currentState.Postion.Z;
                        keyLenght.Distance = currentState.Length;
                        keyLenght.DoorsBlocking = currentState.DoorsBlocking;
                        allKeyLegths.Add(keyLenght);
                    }
                }

                if (IsDoor(currentState.Postion.Z))
                {
                    currentState.DoorsBlocking.Add((char)currentState.Postion.Z);
                }

                foreach (var movement in movements)
                {
                    var newPosition = map.First(m => m.X == currentState.Postion.X + movement.X
                                            && m.Y == currentState.Postion.Y + movement.Y);

                    if (newPosition.Z == (int)'#') continue;
                    if (currentState.Path.Contains(newPosition)) continue;

                    var state = currentState.Clone() as FillState;

                    state.Postion = newPosition;

                    state.Length++;

                    if (toCheck.Contains(state))
                    {
                        var otherState = toCheck.First(s => s.Equals(state));
                        if (otherState.Length > state.Length)
                        {
                            toCheck.Remove(otherState);
                            toCheck.Add(state);
                        }
                    }
                    else
                    {
                        toCheck.Add(state);
                    }

                }
            }

            if (toCheck.Any())
                FindKeys_BFS(map, toCheck, allKeyLegths);
        }

        static bool IsKey(float c)
        {
            return c >= 'a' && c <= 'z';
        }

        static bool IsDoor(float c)
        {
            return c >= 'A' && c <= 'Z';
        }

        //static void WritePaths(List<State> states)
        //{
        //    foreach(var state in states)
        //    {
        //        foreach (char c in state.CollectedKeys)
        //        {
        //            Console.Write((char)c + ",");
        //        }

        //        Console.Write((char)state.Postion.Z + ",");

        //        var length = state.Length;

        //        Console.Write(length + "\n");
        //    }
        //}

        static void PrintPaths()
        {
            foreach (var path in paths)
            {
                foreach (var length in path.Value)
                {
                    Console.WriteLine(string.Format("{0} -> {1}: {2} - BlockedBy: {3}",
                        path.Key, length.Key, length.Distance, new string(length.DoorsBlocking.ToArray())));
                }
            }
        }

        static void DrawMap(List<Vector3> map)
        {
            var maxX = map.Max(m => m.X);
            var maxY = map.Max(m => m.Y);

            for (int i = 0; i <= maxY; i++)
            {
                for (int n = 0; n <= maxX; n++)
                {
                    Console.Write((char)map.First(m => m.X == n && m.Y == i).Z);
                }
                Console.WriteLine();
            }
        }
    }

    public class KeyLength
    {
        public char Key { get; set; }
        public long Distance { get; set; }
        public List<char> DoorsBlocking { get; set; } = new List<char>();
    }
}