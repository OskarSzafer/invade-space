using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProperty : PhysicsSystem
{

    // CELESTIAL BODY PROPERTIES
    [HideInInspector] public int bodyTypeIndex = 0;
    [HideInInspector] public string bodyType;
    
    // BODY PROPERTIES
    [SerializeField] private float mass;
    public float Mass { get { return mass; } set { mass = (mass == 0) ? 1 : mass; } }
    public float Radius;
    // [SerializeField] public float radius;
    [SerializeField] protected Vector2 velocity = Vector2.zero;
    
    // RUNTIME VARIABLES
    protected bool keptOnOrbit = false;


    void Awake()
    {
        mass = (mass == 0) ? 1 : mass;
        Radius = transform.localScale.x / 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhisicsObjects[bodyType].Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        MoveByVelocity();
    }


    protected void MoveByVelocity()
    {
        Vector2 position = transform.position;
        position.x = position.x + velocity.x * Time.deltaTime; // delta time not needed
        position.y = position.y + velocity.y * Time.deltaTime; // delta time not needed
        transform.position = position;
    }

    public GameObject NearestGravitySource(string[] types = null)
    {
        Vector3 position = gameObject.transform.position;
        float nearestDistance = Mathf.Infinity;
        GameObject nearestSource = null;

        if (types == null)
        {
            for (int i = 0; i < gravityDependences.GetLength(0); i++)
            {
                if (gravityDependences[i, bodyTypeIndex])
                {
                    NearestObject(optionList[i], ref nearestSource, ref nearestDistance);
                }
            }
        }
        else
        {
            foreach (string type in types)
            {
                NearestObject(type, ref nearestSource, ref nearestDistance);
            }
        }

        return nearestSource;
    }

    private void NearestObject(string type, ref GameObject nearestObject, ref float nearestDistance)
    {
        Vector3 position = gameObject.transform.position;

        foreach (GameObject obj in PhisicsObjects[type])
        {
            float distance = Vector3.Distance(obj.transform.position, position);
            if (distance < nearestDistance && obj != gameObject)
            {
                nearestDistance = distance;
                nearestObject = obj;
            }
        }
    }

    public void ApplyForce(Vector2 force)
    {
        velocity += force / mass;
    }

    public void SetOnOrbit(GameObject target)
    {
        Vector2 forceDirection = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * target.GetComponent<PhysicsProperty>().Mass / distance);
        Vector2 orbitalVelocity = Vector2.Perpendicular(forceDirection).normalized * orbitalSpeed;

        if (Vector2.Dot(velocity, orbitalVelocity) < 0)
        {
            orbitalVelocity *= -1;
        }

        velocity = orbitalVelocity;
    }

    public void KeepOnOrbit(GameObject target = null, bool active = true)
    {

    }
}

//TODO:
// - colision
// - delete object from list on destroy or disable
// - stmosferic drag