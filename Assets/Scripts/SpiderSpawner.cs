using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderSpawner : GameEntity
{
    public Spider SpiderPrefab;

    private const float START_SPAWN_COOLDOWN_MIN = 3;
    private const float START_SPAWN_COOLDOWN_MAX = 5;
    private const int START_MAX_SPIDER = 3;
    private const float START_SPIDER_DAMAGE = 10;
    private const float START_SPIDER_HEALTH = 100;

    private float spawnCooldownMin;
    private float spawnCooldownMax;
    private int maxSpider = 1;
    private float spiderHealth;
    public float SpiderHealth
    {
        get { return spiderHealth; }
        set
        {
            spiderHealth = value;
            SpiderPrefab.StartHealth = spiderHealth;
            SpiderPrefab.attackDamage = spiderDamage;
        }
    }
    private float spiderDamage;
    public float SpiderDamage
    {
        get { return spiderDamage; }
        set
        {
            spiderDamage = value;
            SpiderPrefab.attackDamage = spiderDamage;
        }
    }

    private GameHandler gameHandler;
    private int spawnCount = 0;
    private int deadSpider = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>();
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator WaitAndSpawnSpider()
    {
        yield return new WaitForSecondsRealtime(Random.Range(spawnCooldownMin, spawnCooldownMax));
        if (gameHandler.CurrentState == GameHandler.State.PLAYING)
        {
            SpawnSpider();
        }

        if (spawnCount < maxSpider) {
            yield return WaitAndSpawnSpider();
        }
    }

    private void SpawnSpider()
    {
        Spider spider = Instantiate(SpiderPrefab, transform);
        spider.Spawner = this;
        spawnCount++;
    }

    public void Restart()
    {
        spawnCooldownMin = START_SPAWN_COOLDOWN_MIN;
        spawnCooldownMax = START_SPAWN_COOLDOWN_MAX;
        maxSpider = START_MAX_SPIDER;
        SpiderHealth = START_SPIDER_HEALTH;
        SpiderDamage = START_SPIDER_DAMAGE;
    }

    public void NewWave()
    {
        spawnCount = 0;
        deadSpider = 0;
        maxSpider++;
        SpiderHealth *= 1.5f;
        SpiderDamage *= 1.5f;
        spawnCooldownMin -= 0.2f;
        spawnCooldownMax -= 0.2f;
        StartCoroutine(WaitAndSpawnSpider());
    }

    public void SpiderDied()
    {
        deadSpider++;
        if (deadSpider == maxSpider)
        {
            // End of wave
            StopAllCoroutines();
            gameHandler.SpawnerCleared();
        }
    }

}
