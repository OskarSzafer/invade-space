using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : PhysicsSystem
{
    // temporal variables to overcome unity reseting static variables on play
    [HideInInspector] public float temporalGravitationalConstant;
    [HideInInspector] public float temporalAtmosphericDragConstant;

    void Awake()
    {
        // initialize static variables
        gravitationalConstant = temporalGravitationalConstant;
        atmosphericDragConstant = temporalAtmosphericDragConstant;
        physicsController = this;

        // initialize PhisicsObjects
        PhisicsObjects = new Dictionary<string, List<GameObject>>();

        foreach (string option in optionList)
        {
            PhisicsObjects.Add(option, new List<GameObject>());
        }
        Debug.Log("PhysicsController Awake");
        ControllerReady = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ForPairOfBodyTypes();
    }

    private void ForPairOfBodyTypes()
    {
        ControllerReady = false; // to avoid errors when objects are added or removed during iteration over PhisicsObjects

        for (int i = 0; i < optionList.Length; i++)
        {
            for (int j = 0; j < optionList.Length; j++)
            {
                // executs for each pair of physics body types
                ForPairOfBodies(PhisicsObjects[optionList[i]], PhisicsObjects[optionList[j]], i ,j);

            }
        }

        ControllerReady = true;
    }

    private void ForPairOfBodies(List<GameObject> targets, List<GameObject> sources, int i, int j)
    {
        foreach (GameObject source in sources)
        {
            foreach (GameObject target in targets)
            {
                // executs for each pair of physics body
                if (source == target) continue;

                if (gravityDependences[i, j])
                {
                    ApplyGravity(target, source);
                }

                if (collisionDependences[i, j])
                {
                    ApplyAtmosphericDragAndCollision (target, source);
                }

            }
        }
    }

    private void ApplyGravity (GameObject target, GameObject source)
    {
        Vector2 force = GravityBetween(target, source);
        target.GetComponent<PhysicsProperty>().ApplyForce(force * Time.fixedDeltaTime); // delta time not necessary
    }

    private void ApplyAtmosphericDragAndCollision (GameObject target, GameObject source)
    {
        // drag and collision merged for optimization
        
        // get physics property for entities
        PhysicsProperty targetPhysicsProperty = target.GetComponent<PhysicsProperty>();
        PhysicsProperty sourcePhysicsProperty = source.GetComponent<PhysicsProperty>();

        Vector2 targetPosition = target.transform.position;
        Vector2 sourcePosition = source.transform.position;
        float distance = Vector2.Distance(targetPosition, sourcePosition);
        float targetAtmosphereRadius = targetPhysicsProperty.AtmosphereRadius;
        float sourceAtmosphereRadius = sourcePhysicsProperty.AtmosphereRadius;
        float atmosphereRadiusSum = targetAtmosphereRadius + sourceAtmosphereRadius;

        if (distance >= atmosphereRadiusSum)
        {
            // no contact
            return;
        }

        // collision detection
        float radiusSum = targetPhysicsProperty.Radius + sourcePhysicsProperty.Radius;

        if (distance < radiusSum)
        {
            // collision occurs
            targetPhysicsProperty.CollisionDetected(source);
        }

        float contactScale = atmosphereRadiusSum - distance;
        Vector2 relativeVelocity = sourcePhysicsProperty.velocity - targetPhysicsProperty.velocity;
        Vector2 force = relativeVelocity * contactScale * atmosphericDragConstant * Time.fixedDeltaTime; // delta time not necessary

        targetPhysicsProperty.ApplyForce(force);
    }


    // used to set body as active or inactive and update body type
    public void changeBodyStatus(GameObject body, bool bodyIsActive)
    {
        StartCoroutine(EditPhisicsObjects(body, bodyIsActive));
    }

    // used to edit PhisicsObjects dictionary
    private IEnumerator EditPhisicsObjects(GameObject body, bool bodyIsActive)
    {
        // Wait until ControllerReady is true
        while (!ControllerReady)
        {
            yield return null;
        }

        // Remove object from PhisicsObjects, if it is present
        foreach (string type in optionList)
        {
            if (PhisicsObjects[type].Contains(body))
            {
                PhisicsObjects[type].Remove(body);
                Debug.Log("Removed " + body.name + " from " + type);
            }
        }

        // Add object to PhisicsObjects, if it is active
        if (bodyIsActive)
        {
            string bodyType = body.GetComponent<PhysicsProperty>().bodyType;
            PhisicsObjects[bodyType].Add(body);
            Debug.Log("Added " + body.name + " to " + bodyType);
        }
    }
}

// can be optimized by merging gravity and collision into additional method
// and not using GravityBetween
// then every component can be called only once

// can be optimize by not checking for each pair 2 times in ForPairOfBodies