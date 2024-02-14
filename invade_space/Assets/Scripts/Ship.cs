using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    // PhysicsProperty
    protected PhysicsProperty physicsProperty;

    // Start is called before the first frame update
    protected void Start()
    {
        physicsProperty = GetComponent<PhysicsProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
