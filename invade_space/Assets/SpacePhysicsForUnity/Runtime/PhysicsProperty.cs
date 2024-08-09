using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PhysicsController")]

public class PhysicsProperty : PhysicsSystem
{

    // TYPE
    [HideInInspector] public int bodyTypeIndex = 0;
    [HideInInspector] public string bodyType;
    
    // BODY PROPERTIES
    [SerializeField] protected bool physicsEnabled = true;
    [SerializeField] protected float mass;
    public float Mass { get { return mass; } set { mass = (value == 0) ? 1 : value; } }
    [SerializeField] protected float radius;
    public float Radius { get { return radius; } set { radius = (value == 0) ? transform.localScale.x / 2 : value; } }
    [SerializeField] protected float atmosphereRadius;
    public float AtmosphereRadius { get { return atmosphereRadius; } set { atmosphereRadius = (value < radius) ? radius : value; } }
    [SerializeField] public Vector2 velocity = Vector2.zero;
    
    // RUNTIME VARIABLES
    protected Vector2 netForce = Vector2.zero; // sum of all forces between updates
    protected Vector2 previousNetForce = Vector2.zero; // sum of all forces between last two updates
    // keep on orbit variables
    protected bool keptOnOrbit = false;
    protected float keptOnOrbitForceThreshold;
    protected GameObject OrbitSource;
    // Collision delegate
    public delegate void CollisionEventHandler(GameObject collidedObject);
    public event CollisionEventHandler OnCollisionDetected;

    // GETTERS
    public bool physicsEnabledStatus { get { return physicsEnabled; } }
    public bool isKeptOnOrbit { get { return keptOnOrbit; } }
    public GameObject GetOrbitSource { get { return OrbitSource; } }
    public Vector2 PreviousNetForce { get { return previousNetForce; } }


    void Awake()
    {
        mass = (mass == 0) ? 1 : mass;
        radius = (radius == 0) ? transform.localScale.x / 2 : radius;
        atmosphereRadius = (atmosphereRadius < radius) ? radius : atmosphereRadius;
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(EnablePhysicsBody());
    }

    // ensures that physicsController is awake
    private IEnumerator EnablePhysicsBody()
    {
        while (physicsController == null)
        {
            yield return null;
        }

        physicsController.changeBodyStatus(gameObject, true);
    }

    void OnDisable()
    {
        if (physicsController != null)
        {
            physicsController.changeBodyStatus(gameObject, false);
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {   
        Vector3 velocityScaled = new Vector3(velocity.x * 10, velocity.y * 10, 0);
        Debug.DrawLine(transform.position, transform.position + velocityScaled, Color.blue, 0.1f);
        
        Vector3 accelerationScaled = new Vector3(netForce.x * 10 / mass, netForce.y * 10 / mass, 0);
        Debug.DrawLine(transform.position, transform.position + accelerationScaled, Color.yellow, 0.1f);

        Vector3 forceScaled = new Vector3(netForce.x * 10, netForce.y * 10, 0);
        Debug.DrawLine(transform.position, transform.position + forceScaled, Color.red, 0.1f);

        if (physicsEnabled)
        {
            CalculateVelocity();
        }
        MoveByVelocity();
    }

    private void CalculateVelocity()
    {
        if (keptOnOrbit)
        {
            Vector2 GravitySourceForce = GravityBetween(gameObject, OrbitSource) * Time.fixedDeltaTime;
            float drift = (GravitySourceForce - netForce).magnitude;
            if (drift > keptOnOrbitForceThreshold)
            {
                Debug.Log("Orbit lost");
                keptOnOrbit = false;
            }
            else
            {
                netForce = GravitySourceForce;
                netForce += (OrbitSource.GetComponent<PhysicsProperty>().PreviousNetForce / OrbitSource.GetComponent<PhysicsProperty>().Mass) * mass;
            }
        }

        velocity += netForce / mass;
        previousNetForce = netForce;
        netForce = Vector2.zero;
    }

    private void MoveByVelocity()
    {
        Vector2 position = transform.position;
        position.x = position.x + velocity.x * Time.fixedDeltaTime; // delta time not necessary
        position.y = position.y + velocity.y * Time.fixedDeltaTime; // delta time not necessary
        transform.position = position;
    }

    // returns nearest gravity source of given type
    // if no type is provided , returns nearest gravity source affecting this object
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

    // for use of NearestGravitySource
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

    // returns center of circular orbit of this object
    public Vector2 OrbitCenter()
    {
        Vector2 centripetalForceDirection = Vector2.Perpendicular(velocity).normalized;
        float centripetalForce = Vector2.Dot(previousNetForce / Time.fixedDeltaTime, centripetalForceDirection);
        
        if (centripetalForce == 0) return new Vector2(float.NaN, float.NaN);
        
        float circularOrbitRadius = Mathf.Pow(velocity.magnitude, 2) / (centripetalForce / mass); // we need only perpendicular component of net force
        Vector2 circularOrbitCenter = (Vector2)transform.position + circularOrbitRadius*centripetalForceDirection;
        return circularOrbitCenter;
    }

    // called by PhysicsController
    internal void CollisionDetected(GameObject collidedObject)
    {
        if (!physicsEnabled) return;

        OnCollisionDetected?.Invoke(collidedObject);
    }

    public void ApplyForce(Vector2 force)
    {
        // applyed force should be multiplied by time
        // if (!physicsEnabled) return; // don't needed because of check in FixedUpdate()
        netForce += force;
    }

    public void changeBodyType(string newType)
    {
        bodyType = newType;
        bodyTypeIndex = System.Array.IndexOf(optionList, bodyType);
        physicsController.changeBodyStatus(gameObject, true);
    }

    public void changeBodyType(int newTypeIndex)
    {
        bodyTypeIndex = newTypeIndex;
        bodyType = optionList[bodyTypeIndex];
        physicsController.changeBodyStatus(gameObject, true);
    }

    public void disablePhysics()
    {
        physicsEnabled = false;
        OnDisable();
    }

    public void enablePhysics()
    {
        physicsEnabled = true;
        OnEnable();
    }

    public void SetOnOrbit(GameObject target)
    {
        if (!physicsEnabled) return;

        Vector2 forceDirection = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * target.GetComponent<PhysicsProperty>().Mass / distance);
        Vector2 orbitalVelocity = Vector2.Perpendicular(forceDirection).normalized * orbitalSpeed;

        if (Vector2.Dot(velocity - target.GetComponent<PhysicsProperty>().velocity, orbitalVelocity) < 0) orbitalVelocity *= -1;

        Vector2 worldOrbitalVelocity = orbitalVelocity + target.GetComponent<PhysicsProperty>().velocity;
        velocity = worldOrbitalVelocity;
    }

    // Body ignores forces other than gravity of the target until the threshold is reached,
    // preventing it from slowly drifting from its orbit.
    public void KeepOnOrbit(
        GameObject source = null,
        float gravityFractionThreshold = 0.5f, 
        float accelerationThreshold = 0.0f, 
        float forceThreshold = 0.0f)
    {
        if (!physicsEnabled) return;

        if (source == null) keptOnOrbit = false;
        else
        {   
            keptOnOrbit = true;
            OrbitSource = source;

            if (forceThreshold != 0.0f)
            {
                keptOnOrbitForceThreshold = forceThreshold;
            }
            else if (accelerationThreshold != 0.0f)
            { 
                keptOnOrbitForceThreshold = accelerationThreshold * Mass;
            }
            else
            {
                keptOnOrbitForceThreshold = GravityBetween(gameObject, source).magnitude * Time.fixedDeltaTime * gravityFractionThreshold;
            }
        }
    }

    // merge two celestial bodies
    public void Merge(GameObject target, string type = "", bool destroyTarget = false, bool changeRadius = true, bool changePosition = true)
    {
        if (!physicsEnabled) return;

        PhysicsProperty targetPhysicsProperty = target.GetComponent<PhysicsProperty>();

        float mergedMass = mass + targetPhysicsProperty.Mass;
        Vector2 mergedVelocity = (mass * velocity + targetPhysicsProperty.Mass * targetPhysicsProperty.velocity) / mergedMass;

        float mergedRadius = radius; //tmp
        float mergedAtmosphereRadius = atmosphereRadius; // tmp

        Vector2 newPosition = transform.position;
        
        // change radius based on two previous radiuses
        // if (changeRadius)
        // {
        //     mergedRadius = Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(targetPhysicsProperty.radius, 2));
        //     Debug.Log("Merged radius: " + mergedRadius);
        //     mergedAtmosphereRadius = Mathf.Sqrt(Mathf.Pow(atmosphereRadius, 2) + Mathf.Pow(targetPhysicsProperty.atmosphereRadius, 2));
        //     Debug.Log("Merged atmosphere radius: " + mergedAtmosphereRadius);
        // }

        // change radius based on previous atmosphere radius and mass change
        // assuming 2D space
        if (changeRadius)
        {
            // R = sqrt(M * r^2 / m)
            mergedRadius = radius * Mathf.Sqrt(mergedMass / mass);
            Debug.Log("Merged radius: " + mergedRadius);
            mergedAtmosphereRadius = atmosphereRadius * Mathf.Sqrt(mergedMass / mass);
            Debug.Log("Merged atmosphere radius: " + mergedAtmosphereRadius);
        }

        if (changePosition)
        {
            newPosition = (transform.position * mass + target.transform.position * targetPhysicsProperty.Mass)/mergedMass;
        }

        if (destroyTarget)
        {
            Destroy(target);
        }

        if (type != "")
        {
            changeBodyType(type);         
        }

        mass = mergedMass;
        velocity = mergedVelocity;
        transform.position = newPosition;
        radius = mergedRadius;
        atmosphereRadius = mergedAtmosphereRadius;
    }
}