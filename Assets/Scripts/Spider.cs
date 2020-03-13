using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : GameEntity
{
    public float speed = 0.001f;
    public float attackDamage = 10;
    public float attackCooldown = 2f;
    private House house;

    private float startTime;
    private float journeyLength;
    private Transform startMarker;
    private Vector3 endMarker;
    private Animator animator;
    private float health = 100;
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
    }
   

    // Update is called once per frame
    void Update()
    {
        if (house != null && !hasReachedHouse)
        {
            transform.LookAt(house.transform);
            if (journeyLength > 0)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startMarker.position, endMarker, fracJourney);

                animator.SetBool("IsMoving", true);
            }
        }
        
    }

    private void Damage(float damage)
    {
        health -= damage;
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

}
