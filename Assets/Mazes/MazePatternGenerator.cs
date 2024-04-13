using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Mazes
{
    public class MazePatternGenerator
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

        public int[,] RandomizedDFS(int width, int height, int? seed = null)
        {
            var maze = new int[height * 2 + 1, width * 2 + 1];
            var stack = new Stack<(int, int)>();

            var random = new Random(seed is null ? (int)DateTime.Now.Ticks : (int)seed);

            stack.Push((2 * random.Next(height) + 1, 2 * random.Next(width) + 1));

            while (stack.Count > 0)
            {
                var (curX, curY) = stack.Pop();
                maze[curX, curY] = 1;
                var nextPoints = GetNextPoints(curX, curY, maze, CanBePlacedOn);
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

        private List<(int, int)> GetNextPoints(int x, int y, int[,] maze, Func<int, int, int[,], bool> predicate)
        {
            var deltaVariants = new List<(int, int)>() { (0, 2), (2, 0), (0, -2), (-2, 0) };
            return deltaVariants
                .Select(d => (d.Item1 + x, d.Item2 + y))
                .Where(p => CanBePlacedOn(p.Item1, p.Item2, maze))
                .ToList();
        }

        private bool IsOnField(int x, int y, int[,] maze)
        {
            return !(x < 0 || x >= maze.GetLength(0) || y < 0 || y >= maze.GetLength(1));
        }

        private bool CanBePlacedOn(int x, int y, int[,] maze)
        {
            /*if (x < 0 || x >= maze.GetLength(0) || y < 0 || y >= maze.GetLength(1))
            {
                return false;
            }*/

            return IsOnField(x, y, maze) && maze[x, y] == 0;
        }
    }
}
