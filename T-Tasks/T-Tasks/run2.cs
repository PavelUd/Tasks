using System;
using System.Collections.Generic;
using System.Linq;

namespace T_Tasks;

class Program
{
    private static Dictionary<string, HashSet<string>> _map = new();
    private static HashSet<string> _gateways = new();

    static void Main()
    {
        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            var tokens = line.Split('-', StringSplitOptions.TrimEntries);
            if (tokens.Length != 2) continue;

            string a = tokens[0];
            string b = tokens[1];
            
            if (!_map.TryGetValue(a, out var linksA))
                _map[a] = linksA = new HashSet<string>();
            linksA.Add(b);

            if (!_map.TryGetValue(b, out var linksB))
                _map[b] = linksB = new HashSet<string>();
            linksB.Add(a);
            
            if (!string.IsNullOrEmpty(a) && char.IsUpper(a[0])) _gateways.Add(a);
            if (!string.IsNullOrEmpty(b) && char.IsUpper(b[0])) _gateways.Add(b);
        }

        var actions = new List<string>(); 

        var path = FindNearestGateway("a", _map, _gateways);
        while (path is not null)
        {
            var pt = path.Value.Path;
            var gateway = path.Value.Gateway;

            if (pt.Count < 2) 
                break;

            var point = pt[^2];

            actions.Add($"{gateway}-{point}"); 
            
            _map[point].Remove(gateway);
            _map[gateway].Remove(point);

            var nextStart = pt.Count > 1 ? pt[1] : pt[0];
            path = FindNearestGateway(nextStart, _map, _gateways);
        }
        
        foreach (var action in actions)
        {
            Console.WriteLine(action);
        }
    }

    private static (string Gateway, List<string> Path)? FindNearestGateway(
        string start,
        Dictionary<string, HashSet<string>> graph,
        HashSet<string> gateways)
    {
        var queue = new Queue<(string Node, List<string> Path)>();
        var visited = new HashSet<string> { start };
        var nearest = new List<(string Gateway, List<string> Path)>();

        queue.Enqueue((start, new List<string> { start }));

        while (queue.Count > 0)
        {
            var (node, path) = queue.Dequeue();

            if (gateways.Contains(node))
            {
                nearest.Add((node, path));
                continue;
            }

            if (!graph.ContainsKey(node)) continue;

            foreach (var neighbor in graph[node].OrderBy(n => n))
            {
                if (visited.Add(neighbor))
                {
                    var newPath = new List<string>(path) { neighbor };
                    queue.Enqueue((newPath.Last(), newPath));
                }
            }
        }

        if (nearest.Count == 0) return null;

        return nearest
            .OrderBy(x => x.Path.Count)
            .ThenBy(x => x.Gateway)
            .First();
    }
}