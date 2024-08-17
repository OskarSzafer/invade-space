using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Player Matter: " + PlayerShipResources.Instance.matter);
        Debug.Log("Player Fuel: " + PlayerShipResources.Instance.fuel);
        Debug.Log("Player Energy: " + PlayerShipResources.Instance.energy);       
    }
}
