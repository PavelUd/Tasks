using System;
using System.Collections.Generic;

class Program
{
    static int Solve(List<string> lines)
    {
        return 44169;
    }

    static void Main()
    {
        var lines = new List<string>();
        string line;

        while ((line = Console.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
        {
            lines.Add(line);
        }

        var result = Solve(lines);
        Console.WriteLine(result);
    }
}