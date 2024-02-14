using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProperty : PhysicsSystem
{

    // CELESTIAL BODY PROPERTIES
    [HideInInspector] public int bodyTypeIndex = 0; // maybe to be removed
    [HideInInspector] public string bodyType;
    
    // BODY PROPERTIES
    [SerializeField] private float mass;
    public float Mass { get { return mass; } set { mass = (mass == 0) ? 1 : mass; } }
    public float Radius;
    // [SerializeField] public float radius;
    [SerializeField] protected Vector2 velocity = Vector2.zero;


    void Awake()
    {
        mass = (mass == 0) ? 1 : mass;

        if (bodyType == "Ship")
        {
            Radius = 0.5f; //transform.localScale.x / 2;
        }
        else
        {
            Radius = transform.localScale.x / 2;
        }
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

    public GameObject NearestGravitySource() // setting sources as parameter
    {
        Vector3 position = gameObject.transform.position;
        float nearestDistance = Mathf.Infinity;
        GameObject nearestSource = null;

        foreach (GameObject source in PhisicsObjects["Planet"])
        {
            float distance = Vector3.Distance(source.transform.position, position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSource = source;
            }
        }

        return nearestSource;
    }

    public void ApplyForce(Vector2 force)
    {
        velocity += force / mass;
    }

    public void SetOnOrbit(GameObject target)
    {
        // TODO:
        // - auto pick direction base on previus velocity
        // - smooth transition

        Vector2 forceDirection = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * target.GetComponent<PhysicsProperty>().Mass / distance);
        Vector2 orbitalVelocity = Vector2.Perpendicular(forceDirection).normalized * orbitalSpeed;

        velocity = orbitalVelocity;
    }
}

//TODO:
// - colision
// - delete object from list on destroy or disable