﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : GameEntity
{
  
    private Rigidbody rb;
    private float thrust = 0.5f;
    private bool launched = false;
    private float damage = 100;
    private Vector3 velocity;

    private const float MAX_LIFETIME = 5;
    private float currentLifeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= MAX_LIFETIME)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (launched)
            rb.velocity = velocity;
    }

    public void Launch(Vector3 direction)
    {
        this.velocity = direction.normalized * thrust;
        launched = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            (other.gameObject.GetComponent<Spider>()).Damage(damage);
        }

        if (other.tag != "Ground")
            Destroy(gameObject);
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
