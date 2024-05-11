using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField]
    GameSceneManager gameSceneManager;

    private float LaunchImpulse = 25;

    private float LaunchTime = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerComponent>(out _))
        {
            StartCoroutine(LaunchPlayerUp(other.gameObject));
        }
    }

    private IEnumerator LaunchPlayerUp(GameObject player)
    {
        if (player.TryGetComponent<QuakeCPMPlayerMovement>(out var qComp))
        {
            while (LaunchTime > 0)
            {
                LaunchTime -= Time.deltaTime;
                qComp.AddVelocity(Vector3.up * LaunchImpulse);
                yield return null;
            }
        }

        gameSceneManager.OnPlayerWin();
    }
}
