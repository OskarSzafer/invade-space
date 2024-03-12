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
    [SerializeField] protected bool physicsEnabeled = true;
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
    public delegate void CollisionEventHandler(GameObject collidedObject);
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
        physicsController.changeBodyStatus(gameObject, false);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {   
        if (physicsEnabeled)
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
            }
        }

        velocity += netForce / mass;
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
    // if no type is given, returns nearest gravity source affecting this object
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

    internal void CollisionDetected(GameObject collidedObject) // called from PhysicsController
    {
        if (!physicsEnabeled) return;

        OnCollisionDetected?.Invoke(collidedObject);
    }

    public void ApplyForce(Vector2 force)
    {
        // applyed force should be multiplied by time
        // if (!physicsEnabeled) return; dont needed because of check in FixedUpdate
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
        physicsEnabeled = false;
        OnDisable();
    }

    public void enablePhysics()
    {
        physicsEnabeled = true;
        OnEnable();
    }

    public void SetOnOrbit(GameObject target)
    {
        if (!physicsEnabeled) return;

        Vector2 forceDirection = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * target.GetComponent<PhysicsProperty>().Mass / distance);
        Vector2 orbitalVelocity = Vector2.Perpendicular(forceDirection).normalized * orbitalSpeed;

        if (Vector2.Dot(velocity, orbitalVelocity) < 0) orbitalVelocity *= -1;

        velocity = orbitalVelocity;
    }

    // body ignore forces other then gravity of target, until threshold is reached
    // to prevent it from slowly drifting from orbit
    public void KeepOnOrbit(
        GameObject source = null,
        float gravityFractionThreshold = 0.5f, 
        float accelerationThreshold = 0.0f, 
        float forceThreshold = 0.0f)
    {
        if (!physicsEnabeled) return;

        if (source == null) keptOnOrbit = false;
        else
        {   
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

            Vector2 GravitySourceForce = GravityBetween(gameObject, source) * Time.fixedDeltaTime;
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

    public GameObject GetOrbitSource()
    {
        return OrbitSource;
    }

    // merge two physics bodies
    public void Merge(GameObject target, string type = "", bool destroyTarget = false, bool changeRadius = true)
    {
        if (!physicsEnabeled) return;

        PhysicsProperty targetPhysicsProperty = target.GetComponent<PhysicsProperty>();

        float mergedMass = mass + targetPhysicsProperty.Mass;
        Vector2 mergedVelocity = (mass * velocity + targetPhysicsProperty.Mass * targetPhysicsProperty.velocity) / mergedMass;

        float mergedRadius = radius;
        float mergedAtmosphereRadius = atmosphereRadius;

        if (changeRadius)
        {
            mergedRadius = Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(targetPhysicsProperty.radius, 2));
            Debug.Log("Merged radius: " + mergedRadius);
            mergedAtmosphereRadius = Mathf.Sqrt(Mathf.Pow(atmosphereRadius, 2) + Mathf.Pow(targetPhysicsProperty.atmosphereRadius, 2));
            Debug.Log("Merged atmosphere radius: " + mergedAtmosphereRadius);
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
        radius = mergedRadius;
        atmosphereRadius = mergedAtmosphereRadius;
        velocity = mergedVelocity;
    }
}