using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler of the game logic
/// </summary>
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
    
    void Start()
    {
        TxtGameInfo.gameObject.SetActive(true);
        BtnStartGame.gameObject.SetActive(false);
    }
       
    /// <summary>
    /// Called whenever a game object has spawned (from the ARController)
    /// </summary>
    /// <param name="entity">The entity that has spawned</param>
    public void GameObjectSpawned(GameEntity entity)
    {
        if (entity is House)
            house = entity as House;
        else if (entity is SpiderSpawner)
            spiderSpawners.Add(entity as SpiderSpawner);

        // When the house and at least one spider spawner have spawned, enable the game to start
        if (house != null && spiderSpawners.Count != 0)
        {
            TxtGameInfo.gameObject.SetActive(false);
            BtnStartGame.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// When the start button has been pressed
    /// </summary>
    public void StartGame()
    {
        // Reset the game state and start the game
        currentWave = 0;
        DestroyAllEnemiesAndProjectiles();
        CurrentState = State.PLAYING;
        BtnStartGame.gameObject.SetActive(false);
        spiderSpawners.ForEach( e => e.Restart());
        house.Restart();
        NewWave();
    }

    /// <summary>
    /// When the game has ended, display the start button
    /// </summary>
    public void EndGame()
    {
        BtnStartGame.gameObject.SetActive(true);
    }

    /// <summary>
    /// When a new wave has started, show the wave number text and increase the spider strength
    /// </summary>
    public void NewWave()
    {
        currentWave++;
        StartCoroutine(DisplayWaveText());
    }

    // Coroutine to display the text and start the new wave of the spawners
    private IEnumerator DisplayWaveText()
    {
        TxtCurrentWave.text = "Wave " + currentWave;
        TxtCurrentWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveDisplayTime);
        TxtCurrentWave.gameObject.SetActive(false);
        spiderSpawners.ForEach(e => e.NewWave());
    }

    /// <summary>
    /// When the house has been destroyed, end the game
    /// </summary>
    public void HouseDestroyed()
    {
        CurrentState = State.ENDED;
        StartCoroutine(DisplayEndGameText());

    }

    /// <summary>
    /// Display the end of game text (the number of wave survived)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayEndGameText()
    {
        DestroyAllEnemiesAndProjectiles();
        TxtCurrentWave.text = $"You survived {currentWave} waves ";
        TxtCurrentWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(endGameDisplayTime);
        TxtCurrentWave.gameObject.SetActive(false);
        BtnStartGame.gameObject.SetActive(true);
    }

    /// <summary>
    /// As the name indicates, destroy all enemies and projectiles
    /// </summary>
    private void DestroyAllEnemiesAndProjectiles()
    {
        Array.ForEach(GameObject.FindGameObjectsWithTag("Enemy"), element => Destroy(element));
        Array.ForEach(GameObject.FindGameObjectsWithTag("Fireball"), element => Destroy(element));
    }

    /// <summary>
    /// When all enemies from a spawner has been cleared, start a new wave
    /// </summary>
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
