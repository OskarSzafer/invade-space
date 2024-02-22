using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : PhysicsSystem
{
    void Awake()
    {
        PhisicsObjects = new Dictionary<string, List<GameObject>>();

        foreach (string option in optionList)
        {
            PhisicsObjects.Add(option, new List<GameObject>());
        }      
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
                if (gravityDependences[j, i])
                {
                    ApplyGravity(target, source);
                }

                if (collisionDependences[j, i])
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
        // to be implemented
    }
}

// can be optimized by merging gravity and collision into additional method
// and not using GravityBetween
// then every component can be called only once