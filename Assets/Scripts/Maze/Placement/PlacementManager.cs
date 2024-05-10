using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mazes;
using UnityEngine;
using Random = System.Random;

public class PlacementManager : MonoBehaviour
{
    public MazeBuilder MazeBuilder;

    private List<IPlacementItem> placementItems;

    private int mazeHeight;
    private int mazeWidth;
    
    private int[,] placementMap;

    private Random rnd;

    private readonly List<Vector2Int> directions = new()
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up
    };

    public void StartProcess()
    {
        Init();
        foreach (var item in placementItems)
        {
            item.Place(this);
        }
    }

    private void Init()
    {
        mazeHeight = MazeBuilder.Maze.GetLength(0) / 2;
        mazeWidth = MazeBuilder.Maze.GetLength(1) / 2;
        rnd = new Random();
        placementMap = new int[mazeHeight, mazeWidth];
    }
    
    public void PlaceOnPlacementMap(Vector2Int pos, int placementValue)
    {
        var startRow = Math.Max(0, pos.x - placementValue);
        var endRow = Math.Min(mazeHeight, startRow + 2 * placementValue);
        var startCol = Math.Max(0, pos.y - placementValue);
        var endCol = Math.Min(mazeWidth, startCol + 2 * placementValue);
        for (int i = startRow; i < endRow; i++)
        {
            for (int j = startCol; j < endCol; j++)
            {
                placementMap[i, j] += placementValue - Math.Max(Math.Abs(pos.x - i), Math.Abs(pos.y - j));
            }
        }
    }

    public Vector2Int GetPosition()
    {
        var pos = GetPositionInMaze();
        return new Vector2Int(2 * pos.x + 1, 2 * pos.y + 1);
    }
    
    public Vector2Int GetPosition(int maxPlacementValue)
    {
        var result = BfsPosition(GetPositionInMaze(), maxPlacementValue);
        return new Vector2Int(2 * result.x + 1, 2 * result.y + 1);
    }

    public Vector3 GetPositionInUnity(Vector2Int pos)
    {
        var positionInMaze = new Vector3(
            ((MazeBuilder.WallThickness + MazeBuilder.PassageThickness) / 2.0f) * pos.x,
            0,
            ((MazeBuilder.WallThickness + MazeBuilder.PassageThickness) / 2.0f) * pos.y
        );
        return positionInMaze + MazeBuilder.transform.position;
    }

    private Vector2Int GetPositionInMaze() => new(rnd.Next(mazeHeight), rnd.Next(mazeWidth));

    private Vector2Int BfsPosition(Vector2Int start, int maxPlacementValue)
    {
        if (placementMap[start.x, start.y] <= maxPlacementValue)
            return start;
        
        var q = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();
        q.Enqueue(start);
        visited.Add(start);
        
        var result = start;

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            foreach (var next in GetNextPoints(current, visited))
            {
                q.Enqueue(next);
                visited.Add(next);

                if (placementMap[result.x, result.y] > placementMap[next.x, next.y])
                {
                    result = next;
                    if (placementMap[result.x, result.y] <= maxPlacementValue)
                        return result;
                }
            }
        }

        return result;
    }

    private IEnumerable<Vector2Int> GetNextPoints(Vector2Int current, HashSet<Vector2Int> visited)
    {
        return directions
            .Select(dir => current + dir)
            .Where(next => IsOnField(next, mazeHeight, mazeWidth) && !visited.Contains(next));
    }

    public List<Vector2Int> GetNextPoints(
        Vector2Int current,
        List<Vector2Int> deltaVariants,
        MazeCell[,] maze,
        Func<Vector2Int, MazeCell[,], bool> predicate)
    {
        return deltaVariants
            .Select(d => d + current)
            .Where(p => predicate(p, maze))
            .ToList();
    }

    public bool IsOnField(Vector2Int point, int height, int width)
    {
        return !(point.x < 0 || point.x >= height || point.y < 0 || point.y >= width);
    }
}