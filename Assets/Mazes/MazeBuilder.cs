using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Mazes
{
    internal class MazeBuilder: MonoBehaviour
    {
        public bool IgnoreSeed = true;

        public int Seed = 42;

        public float WallThickness = 1;

        public float PassageThickness = 1;

        public float TotalWallHeight = 1;

        public int MazeSize_X = 10;

        public int MazeSize_Z = 10;

        public void Start()
        {
            var generator = new MazePatternGenerator();
            var maze = generator.RandomizedDFS(MazeSize_X, MazeSize_Z, IgnoreSeed ? null : Seed);

            var mazeGameObject = new GameObject("The Maze");
            mazeGameObject.transform.position = transform.position;

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == 0)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        var xThickness = WallThickness;
                        if (i % 2 == 1)
                        {
                            xThickness = PassageThickness;
                        }

                        var zThickness = WallThickness;
                        if (j % 2 == 1)
                        {
                            zThickness = PassageThickness;
                        }

                        go.transform.localScale = new Vector3(xThickness, TotalWallHeight, zThickness);

                        var delta = new Vector3(
                                ((WallThickness + PassageThickness) / 2.0f) * i,
                                0,
                                ((WallThickness + PassageThickness) / 2.0f) * j
                            );

                        go.transform.position = mazeGameObject.transform.position + delta;// + new Vector3(i, 0, j);
                        go.transform.parent = mazeGameObject.transform;
                    }
                }
            }

            Destroy(gameObject);    //remove mazeBuilder from scene
        }
    }
}
