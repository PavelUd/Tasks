using System;
using System.Collections.Generic;
using System.Linq;

class Programgg
{
    static void VMain()
    {
        var lines = new List<string>();
        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            lines.Add(line);
        }

        if (lines.Count < 3)
        {
            return;
        }

        int rsize;
        switch (lines.Count)
        {
            case 5:
                rsize = 2;
                break;
            case 7:
                rsize = 4;
                break;
            default:
                return;
        }

        var state = Parser.ParseState(lines, rsize);
        Console.WriteLine(DijkstraSolver.Solve(state, rsize));
    }
}

public static class Parser
{
    public static string ParseState(List<string> inputLines, int roomSize)
    {
        var hallway = ExtractHallway(inputLines);
        var roomLines = ExtractRoomLines(inputLines, roomSize);
        return BuildStateString(roomLines, hallway, roomSize);
    }
    
    private static string ExtractHallway(List<string> inputLines)
    {
        return inputLines[1].TrimEnd().Replace("#", "");
    }
    
    private static string[] ExtractRoomLines(List<string> inputLines, int roomSize)
    {
        var rooms = new string[roomSize];
        for (var i = 0; i < roomSize; i++)
        {
            rooms[i] = inputLines[2 + i].Trim().Replace("#", "");
        }
        return rooms;
    }
    
    private static string BuildStateString(string[] roomLines, string hallway, int roomSize)
    {
        var stateChars = new List<char>();
        var roomWidth = roomLines[0].Length;

        for (var col = 0; col < roomWidth; col++)
        {
            for (var row = 0; row < roomSize; row++)
            {
                stateChars.Add(roomLines[row][col]);
            }
        }

        stateChars.AddRange(hallway);
        return new string(stateChars.ToArray());
    }
}


public static class DijkstraSolver
{
    private const string AmphipodTypes = "ABCD";
    
    public static int Solve(string initialState, int roomSize)
    {
        var priorityQueue = new PriorityQueue<string, int>();
        var seenStates = new Dictionary<string, int> { [initialState] = 0 };

        var finalState = string.Concat(AmphipodTypes.Select(t => new string(t, roomSize))) + new string('.', 11);
        priorityQueue.Enqueue(initialState, 0);

        while (priorityQueue.Count > 0)
        {
            priorityQueue.TryDequeue(out var currentState, out var currentEnergy);

            if (currentState == finalState)
                return currentEnergy;

            foreach (var move in GenerateMoves(currentState, roomSize))
            {
                var newState = SwapPositions(currentState, move.SourceIndex, move.DestinationIndex);
                var newEnergy = currentEnergy + move.Distance * (int)Math.Pow(10, AmphipodTypes.IndexOf(currentState[move.SourceIndex]));

                if (!seenStates.TryGetValue(newState, out var prevEnergy) || newEnergy < prevEnergy)
                {
                    seenStates[newState] = newEnergy;
                    priorityQueue.Enqueue(newState, newEnergy);
                }
            }
        }

        throw new InvalidOperationException("No solution found.");
    }

    private static string SwapPositions(string state, int i, int j)
    {
        if (i > j) (i, j) = (j, i);
        var chars = state.ToCharArray();
        (chars[i], chars[j]) = (chars[j], chars[i]);
        return new string(chars);
    }

    private static IEnumerable<int> WalkPath(int from, int to)
    {
        var step = from > to ? -1 : 1;
        for (var i = from + step; i != to + step; i += step)
            yield return i;
    }

    private static IEnumerable<HallwayMove> MovesIntoHallway(string state, int doorIndex, int roomSize)
    {
        var firstDoor = 4 * roomSize + 2;
        var doorsBlocked = new HashSet<int>();
        for (var i = 0; i <= 7; i += 2)
            doorsBlocked.Add(firstDoor + i);

        foreach (var end in new[] { 4 * roomSize, state.Length - 1 })
        {
            var distance = 0;
            foreach (var pos in WalkPath(doorIndex, end))
            {
                distance++;
                if (state[pos] != '.') break;
                if (!doorsBlocked.Contains(pos))
                    yield return new HallwayMove { Distance = distance, HallwayIndex = pos };
            }
        }
    }

    private static IEnumerable<Move> GenerateMoves(string state, int roomSize)
    {
        for (var roomStart = 0; roomStart < 4 * roomSize; roomStart += roomSize)
        {
            var expectedType = AmphipodTypes[roomStart / roomSize];
            var doorIndex = 4 * roomSize + 2 * (roomStart / roomSize + 1);

            for (var depth = 0; depth < roomSize; depth++)
            {
                var currentChar = state[roomStart + depth];
                if (currentChar != '.')
                {
                    var isWrong = currentChar != expectedType || Enumerable.Range(depth + 1, roomSize - depth - 1)
                        .Any(d => state[roomStart + d] != expectedType);

                    if (isWrong)
                    {
                        foreach (var hallwayMove in MovesIntoHallway(state, doorIndex, roomSize))
                        {
                            yield return new Move
                            {
                                Distance = hallwayMove.Distance + depth + 1,
                                SourceIndex = roomStart + depth,
                                DestinationIndex = hallwayMove.HallwayIndex
                            };
                        }
                    }
                    break;
                }
            }
        }
        
        for (var hallIndex = 4 * roomSize; hallIndex < state.Length; hallIndex++)
        {
            var amphipod = state[hallIndex];
            if (amphipod == '.') continue;

            var roomStart = AmphipodTypes.IndexOf(amphipod) * roomSize;
            var doorIndex = 4 * roomSize + 2 * (roomStart / roomSize + 1);

            if (WalkPath(hallIndex, doorIndex).All(i => state[i] == '.'))
            {
                var doorDistance = Math.Abs(hallIndex - doorIndex);
                for (var depth = roomSize - 1; depth >= 0; depth--)
                {
                    var cell = state[roomStart + depth];
                    if (cell == '.')
                        yield return new Move
                        {
                            Distance = doorDistance + depth + 1,
                            SourceIndex = hallIndex,
                            DestinationIndex = roomStart + depth
                        };
                    else if (cell != amphipod)
                        break;
                }
            }
        }
    }

    private struct Move
    {
        public int Distance;
        public int SourceIndex;
        public int DestinationIndex;
    }

    private struct HallwayMove
    {
        public int Distance;
        public int HallwayIndex;
    }
}