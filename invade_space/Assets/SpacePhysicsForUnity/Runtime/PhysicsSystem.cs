using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    [HideInInspector] public string[] optionList = new string[] { "Star", "Planet", "Moon", "Ship"};
    protected static Dictionary<string, List<GameObject>> PhisicsObjects;
    protected static bool ControllerReady = false;
    [HideInInspector] public static float gravitationalConstant = 0.1f;
    [HideInInspector] public static float atmosphericDragConstant = 1.0f;
    protected static PhysicsController physicsController;
    
    // Dependences
    // colomn - source
    // row - target
    public static bool[,] gravityDependences = new bool[4, 4]
    {
        {false, false, false, false},
        {true, true, false, false},
        {true, true, false, false},
        {true, true, true, false}
    };

    // collision dependences determine allso if atmospheric drag occurs
    public static bool[,] collisionDependences = new bool[4, 4]
    {
        {true, true, true, true},
        {true, true, true, true},
        {true, true, true, true},
        {true, true, true, true}
    }; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // calculate gravity force between two objects
    public Vector2 GravityBetween(GameObject target, GameObject source)
    {
        if (source == target) return Vector2.zero;
        
        Vector2 forceDirection = source.transform.position - target.transform.position;
        float distance = forceDirection.magnitude;
        forceDirection.Normalize();

        distance = Mathf.Max(distance, source.GetComponent<PhysicsProperty>().Radius + target.GetComponent<PhysicsProperty>().Radius);
        float forceValue = gravitationalConstant * (source.GetComponent<PhysicsProperty>().Mass * target.GetComponent<PhysicsProperty>().Mass) / (distance * distance);
        
        return forceDirection * forceValue;
    }

    //RAYCASTING
    // types - list of body types to check for collision
    // empty list means all types
    public GameObject Raycast(Vector2 origin, Vector2 direction, float distance, string[] types = null)
    {
        Vector2 end = origin + direction * distance;

        return Raycast(origin, end, types);
    }

    public GameObject Raycast(Vector2 beginning, Vector2 end, string[] types = null)
    {
        if (types == null)
        {
            types = optionList;
        }

        GameObject closestBody = null;
        float closestDistance = float.MaxValue;

        foreach (string type in types)
        {
            foreach (GameObject body in PhisicsObjects[type])
            {
                PhysicsProperty physicsProperty = body.GetComponent<PhysicsProperty>();
                Vector2 center = body.transform.position;
                float radius = physicsProperty.Radius;

                Vector2? intersection = GetClosestIntersection(beginning, end, center, radius);

                if (intersection != null)
                {
                    float distance = Vector2.Distance(beginning, intersection.Value);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBody = body;
                    }
                }
            }
        }

        return closestBody;
    }

    private Vector2? GetClosestIntersection(Vector2 p1, Vector2 p2, Vector2 center, float radius)
    {
        Vector2 d = p2 - p1;
        Vector2 f = p1 - center;

        float a = Vector2.Dot(d, d);
        float b = 2 * Vector2.Dot(f, d);
        float c = Vector2.Dot(f, f) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No intersection
            return null;
        }
        else
        {
            // Ray intersects circle
            discriminant = Mathf.Sqrt(discriminant);

            float t1 = (-b - discriminant) / (2 * a);
            float t2 = (-b + discriminant) / (2 * a);

            if (t1 >= 0 && t1 <= 1)
            {
                return p1 + t1 * d;
            }

            if (t2 >= 0 && t2 <= 1)
            {
                return p1 + t2 * d;
            }

            // No valid intersection
            return null;
        }
    }
}