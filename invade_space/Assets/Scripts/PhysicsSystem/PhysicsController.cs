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
        for (int i = 0; i < optionList.Length; i++)
        {
            for (int j = 0; j < optionList.Length; j++)
            {
                // executs for each pair of physics body types
                ForPairOfBodies(PhisicsObjects[optionList[i]], PhisicsObjects[optionList[j]], i ,j);

            }
        }
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
        target.GetComponent<PhysicsProperty>().ApplyForce(force * Time.deltaTime); // delta time not needed
    }

    private void ApplyAtmosphericDragAndCollision (GameObject target, GameObject source)
    {
        // drag and collision merged for optimization
        Vector2 targetPosition = target.transform.position;
        Vector2 sourcePosition = source.transform.position;
        float distance = Vector2.Distance(targetPosition, sourcePosition);
        float targetAtmosphereRadius = target.GetComponent<PhysicsProperty>().AtmosphereRadius;
        float sourceAtmosphereRadius = source.GetComponent<PhysicsProperty>().AtmosphereRadius;
        float atmosphereRadiusSum = targetAtmosphereRadius + sourceAtmosphereRadius;

        if (distance > atmosphereRadiusSum)
        {
            // no contact
            return;
        }

        // collision detection
        float radiusSum = target.GetComponent<PhysicsProperty>().Radius + source.GetComponent<PhysicsProperty>().Radius;

        if (distance < radiusSum)
        {
            // collision
            target.GetComponent<PhysicsProperty>().CollisionDetected();
        }

        // drag
        float contactScale = (distance - radiusSum) / (atmosphereRadiusSum - radiusSum);
        Vector2 relativeVelocity = target.GetComponent<PhysicsProperty>().velocity - source.GetComponent<PhysicsProperty>().velocity;
        Vector2 force = relativeVelocity * atmosphericDragConstant * contactScale * Time.deltaTime; // delta time not needed

        target.GetComponent<PhysicsProperty>().ApplyForce(force);
        
    }
}

// can be optimized by merging gravity and collision into additional method
// and not using GravityBetween
// then every component can be called only once

// can be optimize by not checking for each pair 2 times in ForPairOfBodies