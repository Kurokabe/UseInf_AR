using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : GameEntity
{
    public float speed = 0.001f;
    private House house;

    private float startTime;
    private float journeyLength;
    private Transform startMarker;
    private Vector3 endMarker;

    // Start is called before the first frame update
    void Start()
    {
        house = FindObjectOfType<House>();
        startMarker = transform;
        endMarker = house.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, house.transform.position);
    }
   

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(house.transform);
        if (journeyLength > 0)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker.position, endMarker, fracJourney);
        }
    }
    


}
