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
        Gravity();
    }

    private void Gravity()
    {
        for (int i = 0; i < optionList.Length; i++)
        {
            for (int j = 0; j < optionList.Length; j++)
            {
                if (gravityDependences[i, j])
                {
                    GravityBetween(PhisicsObjects[optionList[j]], PhisicsObjects[optionList[i]]);
                }
            }
        }
    }

    private void GravityBetween (List<GameObject> targets, List<GameObject> sources)
    {
        foreach (GameObject source in sources)
        {
            foreach (GameObject target in targets)
            {
                if (source == target) continue;

                Vector2 forceDirection = source.transform.position - target.transform.position;
                float distance = forceDirection.magnitude;
                forceDirection.Normalize();

                distance = Mathf.Max(distance, source.GetComponent<PhysicsProperty>().Radius + target.GetComponent<PhysicsProperty>().Radius);

                float forceValue = gravitationalConstant * (source.GetComponent<PhysicsProperty>().Mass * target.GetComponent<PhysicsProperty>().Mass) / (distance * distance);
                Vector2 force = forceDirection * forceValue;

                target.GetComponent<PhysicsProperty>().ApplyForce(force * Time.deltaTime, source, true); // delta time not needed
            }
        }
    }
}
