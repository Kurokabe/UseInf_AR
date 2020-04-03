using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the spider spawner logic
/// </summary>
public class SpiderSpawner : GameEntity
{
    public Spider SpiderPrefab;

    // Constants for the start and restart of the game
    private const float START_SPAWN_COOLDOWN_MIN = 3;
    private const float START_SPAWN_COOLDOWN_MAX = 5;
    private const int START_MAX_SPIDER = 3;
    private const float START_SPIDER_DAMAGE = 6;
    private const float START_SPIDER_HEALTH = 66;

    // Variables that changes every waves
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

    /// <summary>
    /// Wait a random time between {spawnCooldownMin} and {spawnCooldownMax} and then spawn a spider
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Create a spider at the spawner location
    /// </summary>
    private void SpawnSpider()
    {
        Spider spider = Instantiate(SpiderPrefab, transform);
        spider.Spawner = this;
        spawnCount++;
    }

    /// <summary>
    /// When the game restarts, reset the spider values
    /// </summary>
    public void Restart()
    {
        spawnCooldownMin = START_SPAWN_COOLDOWN_MIN;
        spawnCooldownMax = START_SPAWN_COOLDOWN_MAX;
        maxSpider = START_MAX_SPIDER;
        SpiderHealth = START_SPIDER_HEALTH;
        SpiderDamage = START_SPIDER_DAMAGE;
    }

    /// <summary>
    /// When a new wave starts, reset some variables and increase the spider frequency and strength
    /// </summary>
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

    /// <summary>
    /// When a spider dies check if all spiders from this spawner died, if so, notify the game handler
    /// </summary>
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
