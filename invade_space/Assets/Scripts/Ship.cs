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
    // ship particle system
    protected ParticleSystem explosionParticleSystem;

    // Start is called before the first frame update
    protected void Start()
    {
        physicsProperty = GetComponent<PhysicsProperty>();
        explosionParticleSystem = transform.Find("Explosion").gameObject.GetComponent<ParticleSystem>();

        // Collision delegate - functions called when collision detected
        physicsProperty.OnCollisionDetected += OnCollision;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollision(GameObject collidedObject) 
    {
        Debug.Log(gameObject.name + "collided with: " + collidedObject.name);
        explosionParticleSystem.Play();
        Destroy(gameObject, 0.5f);
    }
}
