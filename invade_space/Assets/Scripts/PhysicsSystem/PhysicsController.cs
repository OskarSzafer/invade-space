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
        ForGravityDependencesTrue();
    }

    private void ForGravityDependencesTrue()
    {
        for (int i = 0; i < optionList.Length; i++)
        {
            for (int j = 0; j < optionList.Length; j++)
            {
                if (gravityDependences[i, j])
                {
                    ApplyGravity(PhisicsObjects[optionList[j]], PhisicsObjects[optionList[i]]);
                }
            }
        }
    }

    private void ApplyGravity (List<GameObject> targets, List<GameObject> sources)
    {
        foreach (GameObject source in sources)
        {
            foreach (GameObject target in targets)
            {
                Vector2 force = GravityBetween(target, source);
                target.GetComponent<PhysicsProperty>().ApplyForce(force * Time.deltaTime); // delta time not needed
            }
        }
    }
}
