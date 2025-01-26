using System;
using System.Collections;
using System.Collections.Generic;
using Overworld;
using UnityEngine;

public class OverWorldPathfinder
{
    public static List<OverWorldInnerLevel> FindPath(OverWorldInnerLevel start, OverWorldInnerLevel target)
    {
        Queue<List<OverWorldInnerLevel>> queue = new Queue<List<OverWorldInnerLevel>>();
        HashSet<OverWorldInnerLevel> visited = new HashSet<OverWorldInnerLevel>();

        queue.Enqueue(new List<OverWorldInnerLevel> { start });
        visited.Add(start);

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            var currentNode = currentPath[^1];

            if (currentNode == target)
            {
                return currentPath;
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (neighbor != null && neighbor.data.unlocked && !visited.Contains(neighbor))
                {
                    var newPath = new List<OverWorldInnerLevel>(currentPath) { neighbor };
                    queue.Enqueue(newPath);
                    visited.Add(neighbor);
                }
            }
        }

        return new List<OverWorldInnerLevel>();
    }

    private static IEnumerable<OverWorldInnerLevel> GetNeighbors(OverWorldInnerLevel node)
    {
        yield return node.data.northNeighbor;
        yield return node.data.southNeighbor;
        yield return node.data.eastNeighbor;
        yield return node.data.westNeighbor;
    }
}