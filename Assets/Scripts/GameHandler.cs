using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public House house;
    public SpiderSpawner spiderSpawner;

    public Text GameInfo;
    public Button BtnStartGame;

    public enum State
    {
        CONFIGURING,
        PLAYING,
        ENDED
    }

    public State CurrentState = State.CONFIGURING;

    // Start is called before the first frame update
    void Start()
    {
        GameInfo.gameObject.SetActive(true);
        BtnStartGame.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameObjectSpawned(GameEntity entity)
    {
        if (entity is House)
            house = entity as House;
        else if (entity is SpiderSpawner)
            spiderSpawner = entity as SpiderSpawner;

        if (house != null && spiderSpawner != null)
        {
            GameInfo.gameObject.SetActive(false);
            BtnStartGame.gameObject.SetActive(true);
        }

    }

    public void StartGame()
    {
        CurrentState = State.PLAYING;
        BtnStartGame.gameObject.SetActive(false);
    }
}
