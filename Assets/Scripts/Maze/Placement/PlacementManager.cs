using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Mazes;
using Unity.AI.Navigation;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;
using Random = System.Random;

public class PlacementManager : MonoBehaviour
{
    public MazeBuilder MazeBuilder;

    [SerializeField] private bool IgnoredSeed;
    [SerializeField] private int Seed;
    [SerializeField] public List<GameObject> placementObjs = new ();
    [SerializeField] private List<GameObject> respawningMobs = new();
    [SerializeField] private float respawnDelay = 40;
    [SerializeField] private int deadBodyCountThresholdToRespawn = 10;

    private List<IPlacer> respawningMobPlacers;

    private int mazeHeight;
    private int mazeWidth;
    
    public int[,] PlacementValuesMap { get; private set; }

    public Random Rnd { get; private set; }

    private readonly List<Vector2Int> directions = new()
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up
    };

    private IEnumerator ProcessRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnDelay);
            var deadBodyObjs = GameObject.FindGameObjectsWithTag("DeadBody").ToList();
            if (deadBodyObjs.Count < deadBodyCountThresholdToRespawn)
                continue;
            foreach (var obj in deadBodyObjs)
            {
                Destroy(obj);
            }

            foreach (var placer in respawningMobPlacers)
            {
                placer.Place(this);
            }
        }
    }

    public void StartProcess()
    {
        Init();
        foreach (var obj in placementObjs)
        {
            obj.GetComponent<IPlacer>().Place(this);
        }

        respawningMobPlacers = respawningMobs.Select(obj => obj.GetComponent<IPlacer>()).ToList();
        foreach (var placer in respawningMobPlacers)
        {
            placer.Place(this);
        }
        MazeBuilder.Environment.GetComponent<NavMeshSurface>().BuildNavMesh();
        
        if (respawningMobs.Count > 0)
            StartCoroutine(ProcessRespawn());
        else
            Destroy(gameObject);
    }

    private void Init()
    {
        mazeHeight = MazeBuilder.Maze.GetLength(0) / 2;
        mazeWidth = MazeBuilder.Maze.GetLength(1) / 2;
        Rnd = new Random(IgnoredSeed ? (int)DateTime.Now.Ticks : Seed);
        PlacementValuesMap = new int[mazeHeight, mazeWidth];
    }
    
    public void AddOnPlacementMap(Vector2Int pos, int placementValue)
    {
        var x = pos.x / 2;
        var y = pos.y / 2;
        var startRow = Math.Max(0, x - placementValue);
        var endRow = Math.Min(mazeHeight, startRow + 2 * placementValue);
        var startCol = Math.Max(0, y - placementValue);
        var endCol = Math.Min(mazeWidth, startCol + 2 * placementValue);
        for (int i = startRow; i < endRow; i++)
        {
            for (int j = startCol; j < endCol; j++)
            {
                PlacementValuesMap[i, j] += placementValue - Math.Max(Math.Abs(x - i), Math.Abs(y - j));
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

    public Vector3 GetTransformPosition(Vector2Int pos)
    {
        var positionInMaze = new Vector3(
            ((MazeBuilder.WallThickness + MazeBuilder.PassageThickness) / 2.0f) * pos.x,
            0,
            ((MazeBuilder.WallThickness + MazeBuilder.PassageThickness) / 2.0f) * pos.y
        );
        return positionInMaze + MazeBuilder.transform.position;
    }
    
    public Vector3 GetShiftInsideCell()
    {
        var x = (int)(MazeBuilder.PassageThickness - 1) / 2;
        if (Rnd.NextDouble() < 0.5)
            x = -x;
        var z = (int)(MazeBuilder.PassageThickness - 1) / 2;
        if (Rnd.NextDouble() < 0.5)
            z = -z;
        return new Vector3(x, 0, z);
    }

    private Vector2Int GetPositionInMaze() => new(Rnd.Next(mazeHeight), Rnd.Next(mazeWidth));

    public void Spawn(List<Vector3> positions, GameObject prefab)
    {
        foreach (var position in positions)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    private Vector2Int BfsPosition(Vector2Int start, int maxPlacementValue)
    {
        if (PlacementValuesMap[start.x, start.y] <= maxPlacementValue)
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

                if (PlacementValuesMap[result.x, result.y] > PlacementValuesMap[next.x, next.y])
                {
                    result = next;
                    if (PlacementValuesMap[result.x, result.y] <= maxPlacementValue)
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

    private bool IsOnField(Vector2Int point, int height, int width)
    {
        return !(point.x < 0 || point.x >= height || point.y < 0 || point.y >= width);
    }
}