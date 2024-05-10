using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Environment;
using Object = System.Object;

public class WallBuilderSimple
{
    private float WallThickness { get; }
    private float PassageThickness { get; }
    private float WallHeight { get; }

    private Material material;


    public WallBuilderSimple(Material material, float wallThickness = 1, float passageThickness = 1, float wallHeight = 1)
    {
        CheckParameter(wallThickness, nameof(wallThickness));
        CheckParameter(passageThickness, nameof(passageThickness));
        CheckParameter(wallHeight, nameof(wallHeight));

        WallThickness = wallThickness;
        PassageThickness = passageThickness;
        WallHeight = wallHeight;
        this.material = material;
    }

    private void CheckParameter(float parameter, string name)
    {
        if (parameter <= 0)
        {
            throw new ArgumentException($"{name} must be > 0");
        }
    }

    public GameObject BuildWalls(MazeCell[,] maze)
    {
        var mazeWallsGameObject = new GameObject("TheMaze");

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int z = 0; z < maze.GetLength(1); z++)
            {
                if (maze[x, z] == MazeCell.Wall)
                {
                    BuildWallPart(x, z, mazeWallsGameObject);
                }
            }
        }

        return mazeWallsGameObject;
    }

    private GameObject BuildWallPart(int x, int z, GameObject mazeWallsGameObject)
    {
        var wallPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallPart.GetComponent<Renderer>().material = material;

        wallPart.transform.parent = mazeWallsGameObject.transform;

        var xThickness = WallThickness;
        if (x % 2 == 1)
        {
            xThickness = PassageThickness;
        }

        var zThickness = WallThickness;
        if (z % 2 == 1)
        {
            zThickness = PassageThickness;
        }

        wallPart.transform.localScale = new Vector3(xThickness, WallHeight, zThickness);

        wallPart.transform.localPosition = new Vector3(
                        ((WallThickness + PassageThickness) / 2.0f) * x,
                        WallHeight / 2.0f,
                        ((WallThickness + PassageThickness) / 2.0f) * z
                    );
        return wallPart;
    }

    public void BuildExitDoor(MazeCell[,] maze, GameObject doorPrefab, GameObject mazeWallsObject)
    {
        var rowsCount = maze.GetLength(0);
        var colsCount = maze.GetLength(1);
        for (int i = 0; i < colsCount; i++)
        {
            for (int j = 0; j < rowsCount; j++)
            {
                if (maze[i, j] == MazeCell.ExitDoor)
                {
                    // var door = UnityEngine.Object.Instantiate(doorPrefab, mazeWallsObject.transform);
                    var door = doorPrefab;
                    door.transform.parent = mazeWallsObject.transform;
                    
                    door.transform.localPosition = new Vector3(
                        ((WallThickness + PassageThickness) / 2.0f) * i,
                        WallHeight / 2.0f,
                        ((WallThickness + PassageThickness) / 2.0f) * j
                    );
                        
                    if (j == 0)
                        door.transform.Rotate(Vector3.up, -90);
                    else if (j == colsCount - 1)
                        door.transform.Rotate(Vector3.up, 90);
                    else if (i == rowsCount - 1)
                        door.transform.Rotate(Vector3.up, 180);
                }
            }
        }
    }
}
