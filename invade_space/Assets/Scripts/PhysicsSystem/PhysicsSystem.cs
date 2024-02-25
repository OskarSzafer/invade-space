using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    [HideInInspector] public string[] optionList = new string[] { "Star", "Planet", "Moon", "Ship"};
    public static Dictionary<string, List<GameObject>> PhisicsObjects;
    [HideInInspector] public static float gravitationalConstant = 1.0f; // overlaps with inspector value, to fix
    [HideInInspector] public static float atmosphericDragConstant = 10.0f; // overlaps with inspector value, to fix

    // Gravity dependences
    // colomn - target
    // row - source
    public bool[,] gravityDependences = new bool[4, 4]
    {
        {false, true, true, true},
        {false, false, true, true},
        {false, false, false, true},
        {false, false, false, false}
    };

    public bool[,] collisionDependences = new bool[4, 4]
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

    protected Vector2 GravityBetween(GameObject target, GameObject source)
    {
        // calculate gravity force between two objects, even if disabled
        if (source == target) return Vector2.zero;
        
        Vector2 forceDirection = source.transform.position - target.transform.position;
        float distance = forceDirection.magnitude;
        forceDirection.Normalize();

        distance = Mathf.Max(distance, source.GetComponent<PhysicsProperty>().Radius + target.GetComponent<PhysicsProperty>().Radius);
        float forceValue = gravitationalConstant * (source.GetComponent<PhysicsProperty>().Mass * target.GetComponent<PhysicsProperty>().Mass) / (distance * distance);
        
        return forceDirection * forceValue;
    }
}

//TODO:
// - delete object from list on destroy or disable
//
// - switch colomn and row in Dependences matrix