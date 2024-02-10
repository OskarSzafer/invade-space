using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProperty : MonoBehaviour
{

    // CELESTIAL BODY PROPERTIES
    [HideInInspector] public int bodyTypeIndex = 0; // maybe to be removed
    [HideInInspector] public string bodyType;
    [HideInInspector] public string[] optionList = new string[] { "Star", "Planet", "Moon", "Ship"};
    
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
        // add the object to the physics controller to take gravity into account
        GameObject physicsControllerObject = GameObject.Find("PhysicsControllerObject");
        PhysicsController physicsControllerScript = physicsControllerObject.GetComponent<PhysicsController>();

        physicsControllerScript.PhisicsObjects[bodyTypeIndex].Add(gameObject);
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

    public void ApplyForce(Vector2 force)
    {
        velocity += force / mass;
    }
}
