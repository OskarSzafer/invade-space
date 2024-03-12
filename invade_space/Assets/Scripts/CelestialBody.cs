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
        atmosphereObject = transform.Find("AtmosphereObject").gameObject;

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
        MergeBody(collidedObject);
    }

    public void DestroyBody()
    {
        explosionParticleSystem.Play();
        Destroy(gameObject, 1.0f);
    }

    public void MergeBody(GameObject collidedObject)
    {
        Debug.Log(gameObject.name + "merged with: " + collidedObject.name);
        physicsProperty.Merge(collidedObject);
        collidedObject.GetComponent<PhysicsProperty>().disablePhysics();

        float newScale = physicsProperty.Radius * 2;
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
        atmosphereObject.transform.localScale = new Vector3(physicsProperty.AtmosphereRadius/newScale, physicsProperty.AtmosphereRadius/newScale, 1);

        collidedObject.GetComponent<CelestialBody>().DestroyBody();
    }
}
