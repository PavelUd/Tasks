using System; 
using System.Collections.Generic;
using System.Linq; 

namespace T_Tasks;

public class Program
{
    public static void Main()
    {
        var graph = new Dictionary<string, HashSet<string>>();
        
        var gateways = new SortedDictionary<string, SortedSet<string>>();

        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            var tokens = line.Split('-');
            string u = tokens[0], v = tokens[1];

            if (!graph.ContainsKey(u)) graph[u] = new HashSet<string>();
            if (!graph.ContainsKey(v)) graph[v] = new HashSet<string>();

            graph[u].Add(v);
            graph[v].Add(u);

            if (char.IsUpper(v[0]))
            {
                if (gateways.ContainsKey(v))
                {
                    gateways[v].Add(u);
                }
                else
                {
                    gateways[v] = new SortedSet<string>(){u};
                }
            }

            if (char.IsUpper(u[0]))
            {
                if (gateways.ContainsKey(u))
                {
                    gateways[u].Add(v);
                }
                else
                {
                    gateways[v] = new SortedSet<string>(){v};
                }
            }
        }

        var start = "a";
        var actions = new List<string>();

        while (true)
        {
            var path = FindNearestGateway(start, graph, gateways.Keys.ToHashSet());
            if (path == null) break;

            var pt = path.Value.Path;
            var gateway = path.Value.Gateway;

            if (pt.Count == 1) break;

            string point;
            if (pt.Count == 2)
            {
                point = pt[^2];
                gateways[gateway].Remove(point);
                if (!gateways[gateway].Any())
                {
                    gateways.Remove(gateway);
                }
                actions.Add($"{gateway}-{point}");
            }
            else
            {
                var t = gateways.First();
                gateway = t.Key;
                point = t.Value.First();
                t.Value.Remove(point);
                if (t.Value.Count == 0)
                {
                    gateways.Remove(gateway);
                }
                
                actions.Add($"{gateway}-{point}");
            }

            graph[point].Remove(gateway);
            graph[gateway].Remove(point);

            var pathfff = FindNearestGateway(start, graph, gateways.Keys.ToHashSet());
            start = pathfff == null ? start : pathfff.Value.Path[1];
        }

        foreach (var act in actions)
        {
            Console.WriteLine(act);
        }
    }

    private static (string Gateway, List<string> Path)? FindNearestGateway(
        string start,
        Dictionary<string, HashSet<string>> graph,
        HashSet<string> gateways)
    {
        var queue = new Queue<(string Node, List<string> Path)>();
        var visited = new HashSet<string> { start };

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

            foreach (var neighbor in graph[node].OrderBy(n => n, StringComparer.Ordinal))
                if (visited.Add(neighbor))
                {
                    var newPath = new List<string>(path) { neighbor };
                    queue.Enqueue((neighbor, newPath));
                }
        }

        if (nearest.Count == 0) return null;
        
        var result = nearest
            .OrderBy(x => x.Path.Count)
            .ThenBy(x => x.Gateway)
            .ThenBy(p => string.Join(",", p.Path), StringComparer.Ordinal).First();
        

        return result;
    }
}