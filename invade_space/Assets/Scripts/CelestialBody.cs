using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // physics property
    protected PhysicsProperty physicsProperty;
    // particle system object
    protected GameObject explosionObject;
    protected ParticleSystem explosionParticleSystem;
    // atmosphere object
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
        Debug.Log(gameObject.name + " collided with: " + collidedObject.name);
        MergeBody(collidedObject);
    }

    public void DestroyBody()
    {
        explosionParticleSystem.Play();
        Destroy(gameObject, 1.0f);
    }

    public void MergeBody(GameObject collidedObject)
    {
        PhysicsProperty collidedObjectPhysicsProperty = collidedObject.GetComponent<PhysicsProperty>();

        // respect hierarchy of celestial bodies
        if (physicsProperty.bodyTypeIndex > collidedObjectPhysicsProperty.bodyTypeIndex) return;
        if (collidedObjectPhysicsProperty.bodyType == "Ship") return;

        Debug.Log(gameObject.name + " merged with: " + collidedObject.name);
        physicsProperty.Merge(collidedObject);
        collidedObjectPhysicsProperty.disablePhysics();

        float newScale = physicsProperty.Radius * 2;
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
        float newAtmosphereScele = physicsProperty.AtmosphereRadius/physicsProperty.Radius;
        atmosphereObject.transform.localScale = new Vector3(newAtmosphereScele, newAtmosphereScele, 1);

        collidedObject.GetComponent<CelestialBody>().DestroyBody();
    }
}
