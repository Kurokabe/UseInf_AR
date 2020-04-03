using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holder for a spider with movement and attack logic
/// </summary>
public class Spider : GameEntity
{
    public float speed = 0.001f;
    public float attackDamage = 10;
    public float attackCooldown = 2f;
    public SpiderSpawner Spawner;
    private House house;

    private float startTime;
    private float journeyLength;
    private Transform startMarker;
    private Vector3 endMarker;
    private Animator animator;
    public HealthBar HealthBar;
    public float StartHealth = 200;
    private float health;
    private bool hasReachedHouse = false;

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
        // Get and set required variables
        house = FindObjectOfType<House>();
        animator = GetComponentInChildren<Animator>();
        if (house != null)
        {
            startMarker = transform;
            endMarker = house.transform.position;
            startTime = Time.time;
            journeyLength = Vector3.Distance(transform.position, house.transform.position);
        }
        Health = StartHealth;
    }

    void Update()
    {
        if (house != null && !hasReachedHouse)
        {
            // Move the spider toward the house
            transform.LookAt(house.transform);
            if (journeyLength > 0 && Health > 0)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startMarker.position, endMarker, fracJourney);

                animator.SetBool("IsMoving", true);
            }
        }
        
    }

    /// <summary>
    /// When the spider receives some damage, reduce its health and play the animation accordingly (death or damage)
    /// </summary>
    /// <param name="damage">The amount of damage received</param>
    public void Damage(float damage)
    {
        Health -= damage;
        if (health <= 0)
            animator.SetTrigger("Death");
        else
            animator.SetTrigger("Damage");
    }

    /// <summary>
    /// When the spider attack, set its animation to attack
    /// </summary>
    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// When the spider collide with the house (has gotten inside the attack range), start attacking the house
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "House")
        {
            hasReachedHouse = true;
            animator.SetBool("IsMoving", false);
            InvokeRepeating("Attack", 0, attackCooldown);
        }
    }

    /// <summary>
    /// Called during the attack animation to attack the house
    /// </summary>
    public void DamageHouse()
    {
        house.Damage(attackDamage);
    }

    /// <summary>
    /// Called at the end of the die animation to destroy the spider
    /// </summary>
    public void Die()
    {
        Spawner.SpiderDied();
        Destroy(gameObject);
    }

}
