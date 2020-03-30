using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public House house;
    public List<SpiderSpawner> spiderSpawners = new List<SpiderSpawner>();

    public Text TxtGameInfo;
    public Text TxtCurrentWave;
    public Button BtnStartGame;
    private int currentWave = 1;
    private int waveDisplayTime = 2;
    private int endGameDisplayTime = 4;
    private int spawnerCleared = 0;

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
        TxtGameInfo.gameObject.SetActive(true);
        BtnStartGame.gameObject.SetActive(false);
    }
       

    public void GameObjectSpawned(GameEntity entity)
    {
        if (entity is House)
            house = entity as House;
        else if (entity is SpiderSpawner)
            spiderSpawners.Add(entity as SpiderSpawner);

        if (house != null && spiderSpawners.Count != 0)
        {
            TxtGameInfo.gameObject.SetActive(false);
            BtnStartGame.gameObject.SetActive(true);
        }

    }

    public void StartGame()
    {
        currentWave = 0;
        DestroyAllEnemiesAndProjectiles();
        CurrentState = State.PLAYING;
        BtnStartGame.gameObject.SetActive(false);
        spiderSpawners.ForEach( e => e.Restart());
        house.Restart();
        NewWave();
    }

    public void EndGame()
    {
        BtnStartGame.gameObject.SetActive(true);
    }

    public void NewWave()
    {
        currentWave++;
        StartCoroutine(DisplayWaveText());
    }

    private IEnumerator DisplayWaveText()
    {
        TxtCurrentWave.text = "Wave " + currentWave;
        TxtCurrentWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveDisplayTime);
        TxtCurrentWave.gameObject.SetActive(false);
        spiderSpawners.ForEach(e => e.NewWave());
    }

    public void HouseDestroyed()
    {
        CurrentState = State.ENDED;
        StartCoroutine(DisplayEndGameText());

    }

    private IEnumerator DisplayEndGameText()
    {
        DestroyAllEnemiesAndProjectiles();
        TxtCurrentWave.text = $"You survived {currentWave} waves ";
        TxtCurrentWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(endGameDisplayTime);
        TxtCurrentWave.gameObject.SetActive(false);
        BtnStartGame.gameObject.SetActive(true);
    }

    private void DestroyAllEnemiesAndProjectiles()
    {
        Array.ForEach(GameObject.FindGameObjectsWithTag("Enemy"), element => Destroy(element));
        Array.ForEach(GameObject.FindGameObjectsWithTag("Fireball"), element => Destroy(element));
    }

    public void SpawnerCleared()
    {
        spawnerCleared++;
        if (spawnerCleared >= spiderSpawners.Count)
        {
            spawnerCleared = 0;
            NewWave();
        }
    }
}
