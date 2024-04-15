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

        public float WallHeight = 1;

        public int MazeSize_X = 10;

        public int MazeSize_Z = 10;

        public void Start()
        {
            var patternGenerator = new MazePatternGenerator();
            var maze = patternGenerator.RandomizedDFS(MazeSize_X, MazeSize_Z, IgnoreSeed ? null : Seed);

            var wallBuilder = new WallBuilderSimple(WallThickness, PassageThickness, WallHeight);

            var mazeWallsObject = wallBuilder.BuildWalls(maze, 0);
            mazeWallsObject.transform.position = transform.position;

            Destroy(gameObject);    //remove mazeBuilder from scene
        }
    }
}
