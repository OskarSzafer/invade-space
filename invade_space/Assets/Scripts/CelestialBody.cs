using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // physics property
    [SerializeField] protected PhysicsProperty physicsProperty;
    // particle system object
    [SerializeField] protected GameObject explosionObject;
    [SerializeField] protected ParticleSystem explosionParticleSystem;
    // atmosphere object
    [SerializeField] public GameObject atmosphereObject;
    private float rotationSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Disabled due to Instantiate initialization order conflict.
        /*
        physicsProperty = GetComponent<PhysicsProperty>();
        explosionObject = transform.Find("Explosion").gameObject;
        explosionParticleSystem = explosionObject.GetComponent<ParticleSystem>();
        atmosphereObject = transform.Find("AtmosphereObject").gameObject;
        */

        // Collision delegate - functions called when collision detected
        physicsProperty.OnCollisionDetected += OnCollision;

        rotationSpeed = Random.Range(-rotationSpeed, rotationSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        atmosphereObject.transform.Rotate(0, 0, rotationAmount);
    }


    public void updateSpritesScale()
    {
        float newScale = physicsProperty.Radius * 2;
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
        float newAtmosphereScele = physicsProperty.AtmosphereRadius/physicsProperty.Radius;
        atmosphereObject.transform.localScale = new Vector3(newAtmosphereScele, newAtmosphereScele, 1);
    }

    protected void OnCollision(GameObject collidedObject) 
    {
        Debug.Log(gameObject.name + " collided with: " + collidedObject.name);
        MergeBody(collidedObject);
    }

    public void DestroyBody()
    {
        explosionParticleSystem.Play();
        Destroy(gameObject, 5.0f);
        Invoke("HideBody", 0.3f);
    }

    private void HideBody()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        atmosphereObject.GetComponent<Renderer>().enabled = false;
    }

    public void MergeBody(GameObject collidedObject)
    {
        PhysicsProperty collidedObjectPhysicsProperty = collidedObject.GetComponent<PhysicsProperty>();

        // respect hierarchy of celestial bodies
        if (physicsProperty.bodyTypeIndex > collidedObjectPhysicsProperty.bodyTypeIndex) return;
        if (collidedObjectPhysicsProperty.bodyType == "Ship") return;
        if (physicsProperty.bodyTypeIndex == collidedObjectPhysicsProperty.bodyTypeIndex && physicsProperty.Mass < collidedObjectPhysicsProperty.Mass) return;

        Debug.Log(gameObject.name + " merged with: " + collidedObject.name);

        physicsProperty.Merge(collidedObject);
        collidedObjectPhysicsProperty.disablePhysics();

        updateSpritesScale();

        collidedObject.GetComponent<CelestialBody>().DestroyBody();
    }
}
