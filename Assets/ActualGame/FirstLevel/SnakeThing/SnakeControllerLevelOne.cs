using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerLevelOne : SnakeController
{
    private float JumpAwayAcceleration { get; set; } = 3;

    private float JumpAwaySpeed { get; set; } = 5;

    private float JumpAwayTimeLeft { get; set; } = 10;

    private float maxBalloonRadius = 25f;

    private float BalloonIncreaseSpeed = 3f;

    [SerializeField]
    private GameObject Balloon;

    [SerializeField]
    private GameObject Rope;

    private SoundController soundController;

    protected override void Start()
    {
        base.Start();
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }

    protected override void AfterDefeatAction()
    {
        StartCoroutine(JumpVeryHigh());
    }

    private IEnumerator JumpVeryHigh()
    {
        if (TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }

        if (TryGetComponent<AINavigation>(out var aINavigation))
        {
            aINavigation.enabled = false;
        }

        soundController.PlaySound("Balloon", 0.5f, 1, transform.position, gameObject);
        soundController.PlaySound("Laugh", 1f, 1, transform.position);
        
        Balloon.SetActive(true);
        Rope.SetActive(true);

        while (JumpAwayTimeLeft > 0)
        {
            JumpAwaySpeed += JumpAwayAcceleration * Time.deltaTime;
            transform.position += Vector3.up * (JumpAwaySpeed * Time.deltaTime);

            if (Balloon.transform.localScale.x < maxBalloonRadius)
            {
                Balloon.transform.localScale += Vector3.one * (BalloonIncreaseSpeed * Time.deltaTime);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
