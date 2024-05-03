﻿using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Mazes
{
    internal class MazeBuilder: MonoBehaviour
    {
        public bool IgnoreSeed = true;

        public int Seed = 42;

        public float PlacementThreshold = 0.5f;

        public float WallThickness = 1;

        public float PassageThickness = 1;

        public float WallHeight = 1;

        public Material WallMaterial;

        public int MazeSize_X = 10;

        public int MazeSize_Z = 10;
        
        public int CentralRoomSize_X = 4;

        public int CentralRoomSize_Z = 4;

        public GameObject Door;

        public GameObject Environment;

        public void Start()
        {
            var maze = GenerateMaze();

            var mazeWallsObject = Build(maze);

            if (Environment != null)
                BuildNavMeshForSnake(mazeWallsObject);

            Destroy(gameObject);    //remove mazeBuilder from scene
        }

        private int[,] GenerateMaze()
        {
            var patternGenerator = new MazePatternGenerator();
            // var maze = patternGenerator.RandomizedDFS(MazeSize_X, MazeSize_Z, IgnoreSeed ? null : Seed);
            var maze = patternGenerator.RandomizedLinear(MazeSize_X, MazeSize_Z, PlacementThreshold,
                IgnoreSeed ? null : Seed);
            
            patternGenerator.MakeCentralRoom(CentralRoomSize_X, CentralRoomSize_Z, maze);
            patternGenerator.MakeExit(maze, IgnoreSeed ? null : Seed);

            return maze;
        }

        private GameObject Build(int[,] maze)
        {
            var wallBuilder = new WallBuilderSimple(WallMaterial, WallThickness, PassageThickness, WallHeight);

            var mazeWallsObject = wallBuilder.BuildWalls(maze, 0);
            wallBuilder.BuildOuterDoors(maze, Door, mazeWallsObject);
            mazeWallsObject.transform.position = transform.position;

            return mazeWallsObject;
        }

        private void BuildNavMeshForSnake(GameObject mazeWallsObject)
        {
            mazeWallsObject.transform.parent = Environment.transform;
            Environment.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
