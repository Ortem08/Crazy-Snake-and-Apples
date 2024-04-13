using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Mazes
{
    internal class MazeBuilder: MonoBehaviour
    {
        public void Start()
        {
            var generator = new MazePatternGenerator();
            var maze = generator.RandomizedDFS(10, 10);
            using (var sw = new StreamWriter(@"C:\Users\Alex\Desktop\mazeGen.txt"))
            {
                sw.Write(generator.StringifyPattern(maze));
            }
            

            var curPos = transform.position;
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                curPos = transform.position + Vector3.right * i;
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == 0)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.transform.position = curPos;
                    }
                    curPos += Vector3.forward;
                }
            }
        }


    }
}
