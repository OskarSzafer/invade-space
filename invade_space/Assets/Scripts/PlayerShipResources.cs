using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipResources : MonoBehaviour
{
    public static PlayerShipResources Instance {get; private set;}
    public int matter = 0;
    public int fuel = 0;
    public int energy = 0;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
