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
}