using Environment;
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

        public int MazeSize_X = 10;

        public int MazeSize_Z = 10;
        
        public int CentralRoomSize_X = 4;

        public int CentralRoomSize_Z = 4;

        public Door Door;

        public void Start()
        {
            var patternGenerator = new MazePatternGenerator();
            // var maze = patternGenerator.RandomizedDFS(MazeSize_X, MazeSize_Z, IgnoreSeed ? null : Seed);
            var maze = patternGenerator.RandomizedLinear(MazeSize_X, MazeSize_Z, PlacementThreshold,
                IgnoreSeed ? null : Seed);
            
            patternGenerator.MakeCentralRoom(CentralRoomSize_X, CentralRoomSize_Z, maze);
            patternGenerator.MakeExit(maze, IgnoreSeed ? null : Seed);

            var wallBuilder = new WallBuilderSimple(WallThickness, PassageThickness, WallHeight);

            var mazeWallsObject = wallBuilder.BuildWalls(maze, 0);
            SetCentralRoomDoor(maze, mazeWallsObject);
            mazeWallsObject.transform.position = transform.position;

            Destroy(gameObject);    //remove mazeBuilder from scene
        }

        private void SetCentralRoomDoor(int[,] maze, GameObject mazeWallsObject)
        {
            var height = CentralRoomSize_X;
            var width = CentralRoomSize_Z;
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
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == 2)
                    {
                        Door.transform.parent = mazeWallsObject.transform;
                        Door.transform.localPosition = new Vector3(
                            ((WallThickness + PassageThickness) / 2.0f) * i,
                            WallHeight / 2.0f,
                            ((WallThickness + PassageThickness) / 2.0f) * j
                        );
                        
                        if (j == startCol)
                            Door.transform.Rotate(Vector3.up, -90);
                        else if (j == endCol)
                            Door.transform.Rotate(Vector3.up, 90);
                        else if (i == endRow)
                            Door.transform.Rotate(Vector3.up, 180);
                    }
                }
            }
        }
    }
}
