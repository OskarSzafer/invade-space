using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PhysicsController")]

public class PhysicsProperty : PhysicsSystem
{

    // CELESTIAL BODY PROPERTIES
    [HideInInspector] public int bodyTypeIndex = 0;
    [HideInInspector] public string bodyType;
    
    // BODY PROPERTIES
    [SerializeField] protected float mass;
    public float Mass { get { return mass; } set { mass = (mass == 0) ? 1 : mass; } }
    [SerializeField] protected float radius;
    public float Radius { get { return radius; } set { radius = (radius == 0) ? transform.localScale.x / 2 : radius; } }
    [SerializeField] protected float atmosphereRadius;
    public float AtmosphereRadius { get { return atmosphereRadius; } set { atmosphereRadius = (atmosphereRadius < radius) ? radius : atmosphereRadius; } }
    [SerializeField] public Vector2 velocity = Vector2.zero;
    
    // RUNTIME VARIABLES
    protected Vector2 netForce = Vector2.zero; // sum of all forces between updates
    // keep on orbit variables
    protected bool keptOnOrbit = false;
    protected float keptOnOrbitForceThreshold;
    protected GameObject OrbitSource;
    // Collision delegate
    public delegate void CollisionEventHandler();
    public event CollisionEventHandler OnCollisionDetected;


    void Awake()
    {
        mass = (mass == 0) ? 1 : mass;
        Radius = (Radius == 0) ? transform.localScale.x / 2 : Radius;
        AtmosphereRadius = (atmosphereRadius < radius) ? Radius : AtmosphereRadius;
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(WaitForControllerAndAdd());
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        PhisicsObjects[bodyType].Remove(gameObject);
    }

    private IEnumerator WaitForControllerAndAdd()
    {
        // Wait until ControllerReady is true
        while (!ControllerReady)
        {
            yield return null;
        }

        if(!PhisicsObjects[bodyType].Contains(gameObject))
        {
            PhisicsObjects[bodyType].Add(gameObject);
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        CalculateVelocity();
        MoveByVelocity();
    }

    private void CalculateVelocity()
    {
        if (keptOnOrbit)
        {
            Vector2 GravitySourceForce = GravityBetween(gameObject, OrbitSource) * Time.deltaTime;
            float drift = (GravitySourceForce - netForce).magnitude;
            if (drift > keptOnOrbitForceThreshold)
            {
                Debug.Log("Orbit lost");
                keptOnOrbit = false;
            }
            else
            {
                netForce = GravitySourceForce;
            }
        }

        velocity += netForce / mass;
        netForce = Vector2.zero;
    }

    private void MoveByVelocity()
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
                if (gravityDependences[bodyTypeIndex, i])
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

    private void NearestObject(string type, ref GameObject nearestObject, ref float nearestDistance) // for NearestGravitySource use
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

    internal void CollisionDetected()
    {
        OnCollisionDetected?.Invoke();
    }

    public void ApplyForce(Vector2 force)
    {
        // applyed force should be multiplied by time
        netForce += force;
    }

    public void SetOnOrbit(GameObject target)
    {
        Vector2 forceDirection = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * target.GetComponent<PhysicsProperty>().Mass / distance);
        Vector2 orbitalVelocity = Vector2.Perpendicular(forceDirection).normalized * orbitalSpeed;

        if (Vector2.Dot(velocity, orbitalVelocity) < 0) orbitalVelocity *= -1;

        velocity = orbitalVelocity;
    }

    public void KeepOnOrbit(GameObject source = null, float accelerationThreshold = 1.0f)
    {
        // body ignore forces other then gravity of target, until threshold is reached

        if (source == null) keptOnOrbit = false;
        else
        {   
            keptOnOrbitForceThreshold = accelerationThreshold * Mass;

            Vector2 GravitySourceForce = GravityBetween(gameObject, source) * Time.deltaTime;
            float drift = (GravitySourceForce - netForce).magnitude;
            if (drift > keptOnOrbitForceThreshold)
            {
                Debug.Log("Orbit impossible");
            }
            else
            {
                keptOnOrbit = true;
                OrbitSource = source;
            }
        }
    }
}