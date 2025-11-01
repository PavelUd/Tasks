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
            start = pt.Count > 2 ? pt[1] : pt[0];

            if (pt.Count == 1)
            {
                break;
            }
            var point = path.Value.Point;
            actions.Add($"{gateway}-{point}");
            graph[point].Remove(gateway);
            graph[gateway].Remove(point);
            path = FindNearestGateway(start, graph, gateways);
        }

        foreach (var act in actions)
        {
            Console.WriteLine(act);
        }
    }

    private static (string Gateway, string Point, List<string> Path)? FindNearestGateway(
    string start,
    Dictionary<string, HashSet<string>> graph,
    HashSet<string> gateways)
{
    var queue = new Queue<(string Node, List<string> Path)>();
    var visited = new HashSet<string> { start };

    // Для каждого шлюза храним узел перед ним и длину пути
    var shh = new Dictionary<string, (string NodeBefore, int Length)>();
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

        if (!graph.ContainsKey(node)) 
            continue;
        foreach (var neighbor in graph[node].OrderBy(n => n))
        {
            if (gateways.Contains(neighbor))
            {
                if (!shh.ContainsKey(neighbor))
                {
                    shh[neighbor] = (node, path.Count);
                }
                else
                {
                    var info = shh[neighbor];
                    if (info.Length > path.Count || (info.Length == path.Count && string.Compare(info.NodeBefore, node, StringComparison.Ordinal) > 0))
                    {
                        shh[neighbor] = (node, path.Count);
                    }
                }
            }
            if (visited.Add(neighbor))
            {
                var newPath = new List<string>(path) { neighbor };
                queue.Enqueue((neighbor, newPath));
            }
        }
    }

    if (nearest.Count == 0) 
        return null;
    
    var chosen = nearest
        .OrderBy(x => x.Path.Count)
        .ThenBy(x => x.Gateway, StringComparer.Ordinal)
        .First();

    return (chosen.Gateway, shh[chosen.Gateway].NodeBefore, chosen.Path);
}
}