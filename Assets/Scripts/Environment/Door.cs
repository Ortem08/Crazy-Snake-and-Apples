using System;
using UnityEngine;

namespace Environment
{
    public class Door : MonoBehaviour, IPlacementItem
    {
        private Animator animator;
        private BoxCollider collider;
        private bool isOpen;

        private void Start()
        {
            animator = GetComponent<Animator>();
            collider = GetComponent<BoxCollider>();
        }

        public void Interact()
        {
            if (enabled == false)
                return;
            
            if (isOpen)
                Close();
            else
                Open();
            isOpen = !isOpen;
        }

        private void Open()
        {
            collider.enabled = false;
            animator.SetBool("isOpen", true);
        }

        private void Close()
        {
            animator.SetBool("isOpen", false);
            collider.enabled = true;
        }

        public void Place(PlacementManager manager)
        {
            var position = FindPosition(manager.MazeBuilder.Maze);
            if (position == Vector2Int.zero)
                return;
            
            transform.position = manager.GetPositionInUnity(position);
            transform.parent = manager.MazeBuilder.Environment.transform;
            if (position.x == 0)
                transform.Rotate(Vector3.up, 90);
            else if (position.x == manager.MazeBuilder.Maze.GetLength(0) - 1)
                transform.Rotate(Vector3.up, -90);
            else if (position.y == manager.MazeBuilder.Maze.GetLength(1) - 1)
                transform.Rotate(Vector3.up, 180);
        }

        private Vector2Int FindPosition(MazeCell[,] maze)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == MazeCell.ExitDoor)
                        return new Vector2Int(i, j);
                }
            }

            return Vector2Int.zero;
        }
    }
}