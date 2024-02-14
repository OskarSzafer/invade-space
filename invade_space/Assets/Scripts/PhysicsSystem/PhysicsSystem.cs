using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    [HideInInspector] public string[] optionList = new string[] { "Star", "Planet", "Moon", "Ship"};
    public static Dictionary<string, List<GameObject>> PhisicsObjects;
    protected float gravitationalConstant = 1.0f;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
