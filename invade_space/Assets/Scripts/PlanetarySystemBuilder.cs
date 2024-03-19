using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetarySystemBuilder : MonoBehaviour
{
    // prefabs
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject gasPlanetPrefab;
    [SerializeField] private GameObject rockyPlanetPrefab;
    //[SerializeField] private GameObject moonPrefab;
    //[SerializeField] private GameObject shipPrefab;

    GameObject celestialBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) //orbit nearest celestial body
        {
            CreateCelestialBody(starPrefab, new Vector3(0, 100, 0), 1000, 1, 2);
        }
    }

    public GameObject CreateCelestialBody(
        GameObject celestialBodyPrefab,
        Vector3 position,
        float mass,
        float radius,
        float atmosphereRadius
    )
    {
        celestialBody = Instantiate(celestialBodyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        celestialBody.transform.position = position;

        celestialBody.GetComponent<PhysicsProperty>().Mass = mass;
        celestialBody.GetComponent<PhysicsProperty>().Radius = radius;
        celestialBody.GetComponent<PhysicsProperty>().AtmosphereRadius = atmosphereRadius;

        celestialBody.GetComponent<CelestialBody>().updateSpritesScale();

        return celestialBody;
    }
}
