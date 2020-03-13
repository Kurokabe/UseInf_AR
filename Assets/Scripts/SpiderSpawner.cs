using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderSpawner : GameEntity
{
    public Spider SpiderPrefab;

    private int spawnCooldown = 3;
    private int spawnTotal = 1;

    private GameHandler gameHandler;
    private int spawnCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>();
        StartCoroutine(WaitAndSpawnSpider());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator WaitAndSpawnSpider()
    {
        yield return new WaitForSecondsRealtime(spawnCooldown);
        if (gameHandler.CurrentState == GameHandler.State.PLAYING)
        {
            SpawnSpider();
        }

        if (spawnCount < spawnTotal) {
            yield return WaitAndSpawnSpider();
        }
    }

    private void SpawnSpider()
    {
        Instantiate(SpiderPrefab, transform);
        spawnCount++;
    }
   

}
