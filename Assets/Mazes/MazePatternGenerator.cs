using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;


namespace Assets.Mazes
{
    public partial class MazePatternGenerator
    {
        public string StringifyPattern(int[,] maze)
        {
            var sb = new StringBuilder();

            var height = maze.GetLength(0);
            var width = maze.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (maze[i, j] == 0)
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public int[,] RandomizedDFS(int height, int width, int? seed = null)
        {
            var maze = new int[height * 2 + 1, width * 2 + 1];
            var stack = new Stack<(int, int)>();

            var random = new Random(seed is null ? (int)DateTime.Now.Ticks : (int)seed);
            var deltaVariants = new List<(int, int)>() { (0, 2), (2, 0), (0, -2), (-2, 0) };

            stack.Push((2 * random.Next(height) + 1, 2 * random.Next(width) + 1));

            while (stack.Count > 0)
            {
                var (curX, curY) = stack.Pop();
                maze[curX, curY] = 1;
                var nextPoints = GetNextPoints(curX, curY, maze, CanBePlacedOn, deltaVariants);
                if (nextPoints.Count == 0)
                {
                    continue;
                }
                stack.Push((curX, curY));

                var nextV = nextPoints[random.Next(nextPoints.Count)];
                maze[(curX + nextV.Item1) / 2, (curY + nextV.Item2) / 2] = 1;
                maze[nextV.Item1, nextV.Item2] = 1;

                stack.Push(nextV);
            }

            return maze;
        }

        private List<(int, int)> GetNextPoints(int x, int y, int[,] maze, Func<int, int, int[,], bool> predicate,
            List<(int, int)> deltaVariants)
        {
            return deltaVariants
                .Select(d => (d.Item1 + x, d.Item2 + y))
                .Where(p => predicate(p.Item1, p.Item2, maze))
                .ToList();
        }

        private bool IsOnField(int x, int y, int[,] maze)
        {
            return !(x < 0 || x >= maze.GetLength(0) || y < 0 || y >= maze.GetLength(1));
        }

        private bool CanBePlacedOn(int x, int y, int[,] maze)
        {
            return IsOnField(x, y, maze) && maze[x, y] == 0;
        }
    }

    public partial class MazePatternGenerator
    {
        public void MakeExit(int[,] maze, int? seed)
        {
            var height = maze.GetLength(0) / 2;
            var width = maze.GetLength(1) / 2;
            
            var exit = GetRandomPointOnBorder(height, width, seed);
            if (exit.x == 0 || exit.x == height)
                maze[2 * exit.x, 2 * exit.y + 1] = 1;
            else
                maze[2 * exit.x + 1, 2 * exit.y] = 1;
        }
        
        private (int x, int y) GetRandomPointOnBorder(int height, int width, int? seed)
        {
            var random = new Random(seed ?? (int)DateTime.Now.Ticks);
            int x, y;
            if (random.NextDouble() < 0.5)
            {
                x = random.Next(height);
                y = random.NextDouble() < 0.5 ? 0 : width;
            }
            else
            {
                x = random.NextDouble() < 0.5 ? 0 : height;
                y = random.Next(width);
            }

            return (x, y);
        }

        private void OpenUnreachableZones(int[,] maze)
        {
            var mazeHeight = maze.GetLength(0);
            var mazeWidth = maze.GetLength(1);
            var visited = new bool[mazeHeight, mazeWidth];
            DfsMaze(maze, visited, (1, 1));
            for (int i = 1; i < mazeHeight - 1; i++)
            {
                for (int j = 1 + (i % 2); j < mazeWidth - 1; j += 2)
                {
                    if ((i % 2 == 1 && !visited[i, j + 1])
                        || (i % 2 == 0 && !visited[i + 1, j]))
                    {
                        RemoveWall(i, j, maze, visited);
                    }
                }
            }
        }

        private void RemoveWall(int x, int y, int[,] maze, bool[,] visited)
        {
            maze[x, y] = 1;
            DfsMaze(maze, visited, x % 2 == 1 ? (x, y + 1) : (x + 1, y));
        }

        private void DfsMaze(int[,] maze, bool[,] visited, (int x, int y) start)
        {
            var q = new Queue<(int x, int y)>();
            var deltaVariants = new List<(int, int)>() { (0, 1), (1, 0), (0, -1), (-1, 0) };

            
            q.Enqueue(start);
            visited[start.x, start.y] = true;
        
            while (q.Count > 0)
            {
                var current = q.Dequeue();
                foreach (var next in GetNextPoints(current.x, current.y, maze, CanBeReached, deltaVariants))
                {
                    if (!visited[next.Item1, next.Item2])
                    {
                        visited[next.Item1, next.Item2] = true;
                        q.Enqueue(next);
                    }
                }
            }
        }
        
        private bool CanBeReached(int x, int y, int[,] maze)
        {
            return IsOnField(x, y, maze) && maze[x, y] == 1;
        }
    }
    
    public partial class MazePatternGenerator
    {
        public int[,] RandomizedLinear(int height, int width, float placementThreshold, int? seed = null)
        {
            var maze = new int[2 * height + 1, 2 * width + 1];

            var rowsCount = maze.GetLength(0) - 1;
            var colsCount = maze.GetLength(1) - 1;

            for (int i = 0; i <= rowsCount; i++)
            {
                for (int j = 0; j <= colsCount; j++)
                {
                    maze[i, j] = 1;
                }
            }

            var random = new Random(seed ?? (int)DateTime.Now.Ticks);

            for (int i = 0; i <= rowsCount; i++)
            {
                for (int j = 0; j <= colsCount; j++)
                {
                    if (i == 0 || j == 0 || i == rowsCount || j == colsCount)
                    {
                         maze[i, j] = 0;
                    }
                    else if (i % 2 == 0 && j % 2 == 0)
                    { 
                        if (random.NextDouble() > placementThreshold)
                        {
                            maze[i, j] = 0;
                            
                            var a = random.NextDouble() < .5 ? 0 : (random.NextDouble() < .5 ? -1 : 1);
                            var b = a != 0 ? 0 : (random.NextDouble() < .5 ? -1 : 1);
                            maze[i + a, j + b] = 0;
                            maze[i + 2 * a, j + 2 * b] = 0;
                        }
                    }
                }
            }
            
            OpenUnreachableZones(maze);
            return maze;
        }
    }
    
    public partial class MazePatternGenerator
    {
        public void MakeCentralRoom(int height, int width, int[,] maze, int? seed = null)
        {
            if (height == 0 || width == 0)
                return;
            var startRow = maze.GetLength(0) / 2 - height;
            var startCol = maze.GetLength(1) / 2 - width;
            if (startRow % 2 == 1)
                startRow--;
            if (startCol % 2 == 1)
                startCol--;
            var endRow = startRow + 2 * height;
            var endCol = startCol + 2 * width;
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    if (i == startRow || j == startCol || i == endRow || j == endCol)
                        maze[i, j] = 0;
                    else
                        maze[i, j] = 1;
                }
            }

            var door = GetRandomPointOnBorder(height, width, seed);
            if (door.x == 0 || door.x == height)
                maze[startRow + 2 * door.x, startCol + 2 * door.y + 1] = 2;
            else
                maze[startRow + 2 * door.x + 1, startCol + 2 * door.y] = 2;
            
            OpenUnreachableZones(maze);
        }
    }
}
