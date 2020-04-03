using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fireball logic
/// </summary>
public class Fireball : GameEntity
{
  
    private Rigidbody rb;
    private float thrust = 0.5f;
    private bool launched = false;
    private float damage = 100;
    private Vector3 velocity;

    private const float MAX_LIFETIME = 5;
    private float currentLifeTime = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check to destroy the fireball after {MAX_LIFETIME} seconds
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= MAX_LIFETIME)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // Set constantly the projectile velocity so that it continue to that direction
        if (launched)
            rb.velocity = velocity;
    }

    /// <summary>
    /// Called after the projectile is created to send it towards a direction
    /// </summary>
    /// <param name="direction">The direction of the fireball</param>
    public void Launch(Vector3 direction)
    {
        this.velocity = direction.normalized * thrust;
        launched = true;
    }

    /// <summary>
    /// When the fireball has finished to collide with the ground, destroy it 
    /// The Exit function here is used to correct a problem when spider spawn under the ground
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
            Destroy(gameObject);
    }

    /// <summary>
    /// When the fireball collide with anything, destroy the fireball. If it collides with an enemy, apply the damage to the enemy
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            (other.gameObject.GetComponent<Spider>()).Damage(damage);
        }

        if (other.tag != "Ground")
            Destroy(gameObject);
    }
}
