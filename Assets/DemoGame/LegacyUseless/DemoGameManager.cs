using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject levelPrefab;

    [SerializeField]
    private Transform deathRoomCenter;

    [SerializeField]
    private RestartTrigger restartTrigger;

    private PlayerComponent player;

    void Start()
    {
        if (levelPrefab == null)
        {
            throw new System.Exception("level prefab not set");
        }
        if (deathRoomCenter == null)
        {
            throw new System.Exception("death room center not set");
        }
        if (restartTrigger == null)
        {
            throw new System.Exception("restartTrigger not set");
        }


        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
        player.OnPlayerDead += OnPlayerDeath;

        StartCoroutine(PrepareGameAndStart());
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PrepareGameAndStart()
    {
        yield return null;
        Instantiate(levelPrefab);
        TransportPlayerIntoLabyrinth();
    }

    public void TransportPlayerIntoLabyrinth()
    {
        player.gameObject.transform.position = new Vector3(-10, 7, -48);
    }

    public void TransportPlayerToDeathRoom()
    {

    }

    private void OnPlayerDeath()
    {
        //player.Resurrect();
    }
}
