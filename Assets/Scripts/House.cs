using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : GameEntity
{
    
    public float StartHealth = 100;

    public HealthBar HealthBar;

    private float health;
    private GameHandler gameHandler;

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

    public void Restart()
    {
        Health = StartHealth;
    }

    public void Damage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            gameHandler.HouseDestroyed();
        }
    }
    

}
