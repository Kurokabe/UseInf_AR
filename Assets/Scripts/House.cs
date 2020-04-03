using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holder for the house
/// Stops the game when its lifepoints go below 0
/// </summary>
public class House : GameEntity
{
    public float StartHealth = 100;

    public HealthBar HealthBar;

    private GameHandler gameHandler;

    private float health;
    /// <summary>
    /// Whenever the health change, change the health bar
    /// </summary>
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (HealthBar != null)
            {
                HealthBar.SetSize(value / StartHealth);
            }
        }
    }

    void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>();
    }

    /// <summary>
    /// When the game restart, reset the current health
    /// </summary>
    public void Restart()
    {
        Health = StartHealth;
    }

    /// <summary>
    /// Damage the house by some amount
    /// </summary>
    /// <param name="damage">The amount of damage</param>
    public void Damage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            gameHandler.HouseDestroyed();
        }
    }
    

}
