using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    // ship speed
    [SerializeField] protected float thrustForce = 10.0f;
    // ship rotation speed
    [SerializeField] protected float rotationRate;
    // ship physics property
    protected PhysicsProperty physicsProperty;

    // Start is called before the first frame update
    protected void Start()
    {
        physicsProperty = GetComponent<PhysicsProperty>();

        // Collision delegate - functions called when collision detected
        physicsProperty.OnCollisionDetected += OnCollision;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollision(GameObject collidedObject) 
    {
        Debug.Log("BOOM! " + collidedObject.name);
    }
}
