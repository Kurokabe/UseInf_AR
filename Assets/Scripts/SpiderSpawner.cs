using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderSpawner : GameEntity
{
    public Spider SpiderPrefab;

    public int spawnCooldown = 3;
    public int spawnTotal = 100;

    private GameHandler gameHandler;
    private int spawnCount = 0;
    private float startSpawnTime;

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
        startSpawnTime = Time.time;
        yield return new WaitForSecondsRealtime(spawnCooldown);
        if (gameHandler.CurrentState == GameHandler.State.PLAYING)
            SpawnSpider();
        else
            yield return WaitAndSpawnSpider();
        if (this.spawnCount++ < this.spawnTotal) {
            yield return WaitAndSpawnSpider();
        }
    }

    private void SpawnSpider()
    {
        
        Instantiate(SpiderPrefab, transform);
        float time = Time.time - startSpawnTime;
        _ShowAndroidToastMessage($"Spawned after {time}");
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
