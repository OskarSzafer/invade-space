using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    public List<List<GameObject>> PhisicsObjects;
    [SerializeField] private float gravitationalConstant = 1.0f;


    void Awake()
    {
        PhisicsObjects = new List<List<GameObject>>();

        for (int i = 0; i < 64; i++) // 64 is the number of layers, can be changed to number of body types
        {
            PhisicsObjects.Add(new List<GameObject>());
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
        Gravity(PhisicsObjects[1], PhisicsObjects[3]);
        Gravity(PhisicsObjects[1], PhisicsObjects[1]);
    }

    private void Gravity (List<GameObject> sources, List<GameObject> targets)
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

                target.GetComponent<PhysicsProperty>().ApplyForce(force * Time.deltaTime); // delta time not needed
            }
        }
    }
}

// TODO: 
// - colision
