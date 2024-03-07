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
    public delegate void CollisionEventHandler(GameObject collidedObject);
    public event CollisionEventHandler OnCollisionDetected;
    // Controller synchronization
    private Coroutine WaitForControllerCoroutine;
    private bool IsWaitForControllerRunning = false;


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
        if (!IsWaitForControllerRunning){
            WaitForControllerCoroutine = StartCoroutine(WaitForController());
        }
    }

    void OnDisable()
    {
        if (IsWaitForControllerRunning)
        {
            StopCoroutine(WaitForControllerCoroutine);
            IsWaitForControllerRunning = false;
        }
        PhisicsObjects[bodyType].Remove(gameObject);
    }

    private IEnumerator WaitForController()
    {
        IsWaitForControllerRunning = true;

        // Wait until ControllerReady is true
        while (!ControllerReady)
        {
            yield return null;
        }

        if(!PhisicsObjects[bodyType].Contains(gameObject))
        {
            PhisicsObjects[bodyType].Add(gameObject);
        }
        IsWaitForControllerRunning = false;
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
        OnCollisionDetected?.Invoke(collidedObject);
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

    public void KeepOnOrbit(
        GameObject source = null,
        float gravityFractionThreshold = 0.5f, 
        float accelerationThreshold = 0.0f, 
        float forceThreshold = 0.0f)
    {
        // body ignore forces other then gravity of target, until threshold is reached
        // to prevent it from slowly drifting from orbit

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
}