using System;
using System.Collections;
using UnityEngine;

namespace Environment
{
    public class Door : MonoBehaviour, IPlacer
    {
        private Animator animator;
        private bool isOpen;

        [SerializeField]
        private NotificationManager notificationManager;

        private string OnPlayerInteractWithoutKeyMsg = "Заперто. Надо попробывать найти ключ";

        private SoundController soundController;

        private void Start()
        {
            animator = GetComponent<Animator>();

            soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        }

        public void Interact()
        {
            if (enabled == false)
            {
                notificationManager?.Notify(OnPlayerInteractWithoutKeyMsg);
                return;
            }

            if (isOpen)
                Close();
            else
                Open();
            isOpen = !isOpen;
        }

        private void Open()
        {
            var length = soundController.PlaySound("Key", 0.5f, 1, transform.position, gameObject);
            StartCoroutine(OpenWithDelay(length));
        }

        private IEnumerator OpenWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            soundController.PlaySound("DoorOpen", 0.5f, 1, transform.position, gameObject);
            animator.SetBool("isOpen", true);
        }

        private void Close()
        {
            animator.SetBool("isOpen", false);
        }

        public void Place(PlacementManager manager)
        {
            Debug.Log("door placer called");
            var position = FindPosition(manager.MazeBuilder.Maze);
            if (position == Vector2Int.zero)
                return;

            Debug.Log("door found");
            Debug.Log(manager.GetTransformPosition(position));

            transform.position = manager.GetTransformPosition(position);
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