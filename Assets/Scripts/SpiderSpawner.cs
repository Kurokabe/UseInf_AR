using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSpawner : GameEntity
{
    public Spider SpiderPrefab;

    public int spawnCooldown = 3;

    private GameHandler gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameHandler.CurrentState == GameHandler.State.PLAYING)
        {
            StartCoroutine(WaitAndSpawnSpider());
        }
    }

    private IEnumerator WaitAndSpawnSpider()
    {
        yield return new WaitForSeconds(spawnCooldown);
        if (gameHandler.CurrentState == GameHandler.State.PLAYING)
            SpawnSpider();
    }

    private void SpawnSpider()
    {
        Instantiate(SpiderPrefab, transform);
    }
    
}
