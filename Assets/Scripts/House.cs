using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : GameEntity
{
    
    public float StartHealth = 100;

    public HealthBar HealthBar;

    private float health;

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


    // Start is called before the first frame update
    void Start()
    {
        Health = StartHealth;
    }

    public void Damage(float damage)
    {
        Health -= damage;
    }
}
