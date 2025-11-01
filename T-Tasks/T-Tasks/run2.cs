using System;
using System.Collections.Generic;
using System.Linq;

namespace T_Tasks;

public static class Program
{
    public static void Main()
    {
        var graph = new Dictionary<string, HashSet<string>>();
        var gateways = new HashSet<string>();

        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            var tokens = line.Split('-');
            string u = tokens[0], v = tokens[1];

            if (!graph.ContainsKey(u)) graph[u] = new HashSet<string>();
            if (!graph.ContainsKey(v)) graph[v] = new HashSet<string>();

            graph[u].Add(v);
            graph[v].Add(u);

            if (char.IsUpper(v[0])) gateways.Add(v);
            if (char.IsUpper(u[0])) gateways.Add(u);
        }

        var start = "a";
        var path = FindNearestGateway(start, graph, gateways);
        var actions = new List<string>();
        var flag = false;
        while (true)
        {
            if (path == null) break;
            var pt = path.Value.Path;
            var gateway = path.Value.Gateway;
            start = path.Value.Point;

            if (pt.Count == 1)
            {
                break;
            }
            var point = path.Value.Path[^2];
            actions.Add($"{gateway}-{point}");
            graph[point].Remove(gateway);
            graph[gateway].Remove(point);
            path = FindNearestGateway(start, graph, gateways);
        }

        foreach (var act in actions.Select(x=>x.Split('-')).OrderBy(x=>x[0]).ThenBy(x=>x[1]))
        {
            Console.WriteLine(act);
        }
    }

    private static (string Gateway,string Point, List<string> Path)? FindNearestGateway(
        string start,
        Dictionary<string, HashSet<string>> graph,
        HashSet<string> gateways)
    {
        var queue = new Queue<(string Node, List<string> Path)>();
        var visited = new HashSet<string> { start };

        var shh = new Dictionary<string,(string node, int length)>();
        queue.Enqueue((start, new List<string> { start }));

        var nearest = new List<(string Gateway, List<string> Path)>();

        while (queue.Count > 0)
        {
            var (node, path) = queue.Dequeue();

            if (gateways.Contains(node))
            {
                nearest.Add((node, path));
                continue;
            }

            foreach (var neighbor in graph[node])
            {
                if (gateways.Contains(neighbor) || visited.Add(neighbor))
                {
                    var newPath = new List<string>(path) { neighbor };
                    queue.Enqueue((neighbor, newPath));
                }
                
            }
        }

        if (nearest.Count == 0) return null;

        var r = nearest
            .OrderBy(x => x.Path.Count)
            .ThenBy(x => x.Gateway);
        var greatPt = r.ThenBy(x => x.Path[^2]).First(); 
        nearest.Remove(greatPt);
        if (nearest.Count == 0)
        {
            return (greatPt.Gateway, greatPt.Path[0], greatPt.Path);
        }
        var virusStart = r.ThenBy(x => x.Path[1]).First();
        return (greatPt.Gateway, virusStart.Path[1], greatPt.Path);
    }
}