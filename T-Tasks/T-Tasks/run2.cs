namespace T_Tasks;

class Program
{
    private static Dictionary<string, HashSet<string>> _map = new();
    private static HashSet<string> gateways = new();
    static void Main()
    {
        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            var tokens =  line.Split("-").ToArray();
            if (_map.TryGetValue(tokens[0], out var value))
            {
                value.Add(tokens[1]);
            }
            else
            {
                _map.Add(tokens[0], [tokens[1]]);
            }
            if (_map.TryGetValue(tokens[1], out var value2))
            {
                value2.Add(tokens[0]);
            }
            else
            {
                _map.Add(tokens[1], [tokens[0]]);
            }

            if (char.IsUpper(char.Parse(tokens[1])))
            {
                gateways.Add(tokens[1]);
            }
            if (char.IsUpper(char.Parse(tokens[0])))
            {
                gateways.Add(tokens[0]);
            }
        }

        var path = FindNearestGateway("a", _map, gateways);
        while (path is not null)
        {
            var pt = path.Value.Path;
            var gateway = path.Value.Gateway;
            if (pt.Count < 2)
            {
                break;
            }
            var point = pt[^2];
            Console.WriteLine($"{gateway}-{point}");
            _map[point].Remove(gateway);
            _map[gateway].Remove(point);
            path = FindNearestGateway(pt[1], _map, gateways);
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
            
            foreach (var neighbor in graph[node].OrderBy(n => n))
            {
                if (visited.Add(neighbor))
                {
                    var newPath = new List<string>(path) { neighbor };
                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        if (nearest.Count == 0)
            return null;
        
        var result = nearest
            .OrderBy(x => x.Path.Count)
            .ThenBy(x => x.Gateway)
            .First();

        return result;
    }
}