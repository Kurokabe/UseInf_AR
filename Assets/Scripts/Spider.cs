using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool hasReachedHouse = false;


    // Start is called before the first frame update
    void Start()
    {
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
   

    // Update is called once per frame
    void Update()
    {
        if (house != null && !hasReachedHouse)
        {
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

    public void Damage(float damage)
    {
        Health -= damage;
        if (health <= 0)
            animator.SetTrigger("Death");
        else
            animator.SetTrigger("Damage");
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "House")
        {
            hasReachedHouse = true;
            animator.SetBool("IsMoving", false);
            InvokeRepeating("Attack", 0, attackCooldown);
        }
    }

    public void DamageHouse()
    {
        house.Damage(attackDamage);
    }

    public void Die()
    {
        Spawner.SpiderDied();
        Destroy(gameObject);
    }

}
