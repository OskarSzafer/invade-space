using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // body physics property
    protected PhysicsProperty physicsProperty;
    // body particle system
    protected GameObject explosionObject;
    protected ParticleSystem explosionParticleSystem;
    // body atmosphere object
    protected GameObject atmosphereObject;

    // Start is called before the first frame update
    void Start()
    {
        physicsProperty = GetComponent<PhysicsProperty>();
        explosionObject = transform.Find("Explosion").gameObject;
        explosionParticleSystem = explosionObject.GetComponent<ParticleSystem>();
        atmosphereObject = transform.Find("Atmosphere").gameObject;

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
        DestroyBody();
    }

    public void DestroyBody()
    {
        explosionParticleSystem.Play();
        Destroy(gameObject, 1.0f);
    }

    public void MergeBody(GameObject collidedObject)
    {
        physicsProperty.Merge(collidedObject);
        gameObject.transform.localScale = new Vector3(physicsProperty.Radius*2, physicsProperty.Radius*2, 1);
        
    }
}
