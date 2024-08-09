using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMaker : MonoBehaviour
{
    private string path = "BodiesBPs/";
    // Star
    [SerializeField] private int StarMinMass = 100000;
    [SerializeField] private int StarMaxMass = 100000;
    [SerializeField] private float StarMinRadius = 32.4f;//36 - 10%
    [SerializeField] private float StarMaxRadius = 39.6f;//36 + 10%
    [SerializeField] private float StarMaxAtmosphereFactor = 1.5f;

    // Gas planet
    [SerializeField] private int GasPlanetMinMass = 100;
    [SerializeField] private int GasPlanetMaxMass = 150;
    [SerializeField] private float GasPlanetMinRadius = 6.0f;
    [SerializeField] private float GasPlanetMaxRadius = 6.6f;//6 + 10%
    [SerializeField] private float GasPlanetMaxAtmosphereFactor = 1.3f;

    // Rocky planet
    [SerializeField] private int RockyPlanetMinMass = 50;
    [SerializeField] private int RockyPlanetMaxMass = 100;
    [SerializeField] private float RockyPlanetMinRadius = 5.4f;//6 - 10%
    [SerializeField] private float RockyPlanetMaxRadius = 6.0f;
    [SerializeField] private float RockyPlanetMaxAtmosphereFactor = 1.2f;

    // Moon
    [SerializeField] private int MoonMinMass = 1;
    [SerializeField] private int MoonMaxMass = 10;
    [SerializeField] private float MoonMinRadius = 1.5f;
    [SerializeField] private float MoonMaxRadius = 2.5f;
    [SerializeField] private float MoonMaxAtmosphereFactor = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CreateSystem();
        }
    }


    public void CreateSystem()
    {
        // TODO: binary system
        GameObject Star = CreateStar(new Vector3(0, 0, 0));

        float distanceFromStar = StarMaxRadius*3;

        // ensure at least one rocky planet
        // ---------------
        distanceFromStar += Random.Range(6*GasPlanetMaxRadius, 24*RockyPlanetMinRadius);
        GameObject RockyPlanet = CreateRockyPlanet(
            RandomDirection() * distanceFromStar,
            new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)), 
            Star
        );
        // ---------------

        // Create planets
        for (int i = 0; i < Random.Range(3, 9); i++)
        {
            //distanceFromStar += Random.Range(4*GasPlanetMaxRadius, 24*RockyPlanetMinRadius);//30a 10b
            distanceFromStar += Random.Range(6*GasPlanetMaxRadius, 24*RockyPlanetMinRadius);

            if (Random.Range(0, 2) == 0)
            {
                GameObject Planet = CreateRockyPlanet(
                    //new Vector3(distanceFromStar, 0, 0), 
                    RandomDirection() * distanceFromStar,
                    new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)), 
                    Star
                );
            }
            else
            {
                GameObject Planet = CreateGasPlanet(
                    //new Vector3(distanceFromStar, 0, 0), 
                    RandomDirection() * distanceFromStar,
                    new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)), 
                    Star
                );
            }

            // TODO: Create moons
        }
    }

    public GameObject CreateStar(Vector3 position, Vector2 velocity = default(Vector2), GameObject orbitSource = null)
    {
        int mass = Random.Range(StarMinMass, StarMaxMass);
        float radius = Random.Range(StarMinRadius, StarMaxRadius);
        float AtmosphereFactor = Random.Range(1.05f, StarMaxAtmosphereFactor);
        float atmosphereRadius = radius * AtmosphereFactor;

        GameObject celestialBody = CreateCelestialBody("StarBP", position, mass, radius, atmosphereRadius, Random.ColorHSV(0f, 0.17f, 0.8f, 1f, 0.5f, 1f));
        celestialBody.GetComponent<PhysicsProperty>().velocity = velocity;

        if (orbitSource != null)
        {
            celestialBody.GetComponent<PhysicsProperty>().SetOnOrbit(orbitSource);
        }

        return celestialBody;
    }

    public GameObject CreateGasPlanet(Vector3 position, Vector2 velocity = default(Vector2), GameObject orbitSource = null)
    {
        int mass = Random.Range(GasPlanetMinMass, GasPlanetMaxMass);
        float radius = Random.Range(GasPlanetMinRadius, GasPlanetMaxRadius);
        float AtmosphereFactor = Random.Range(1.2f, GasPlanetMaxAtmosphereFactor);
        float atmosphereRadius = radius * AtmosphereFactor;

        GameObject celestialBody = CreateCelestialBody("GasPlanetBP", position, mass, radius, atmosphereRadius);
        celestialBody.GetComponent<PhysicsProperty>().velocity = velocity;

        if (orbitSource != null)
        {
            celestialBody.GetComponent<PhysicsProperty>().SetOnOrbit(orbitSource);
        }

        return celestialBody;
    }

    public GameObject CreateRockyPlanet(Vector3 position, Vector2 velocity = default(Vector2), GameObject orbitSource = null)
    {
        int mass = Random.Range(RockyPlanetMinMass, RockyPlanetMaxMass);
        float radius = Random.Range(RockyPlanetMinRadius, RockyPlanetMaxRadius);
        float AtmosphereFactor = Random.Range(1.05f, RockyPlanetMaxAtmosphereFactor);
        float atmosphereRadius = radius * AtmosphereFactor;

        GameObject celestialBody = CreateCelestialBody("RockyPlanetBP", position, mass, radius, atmosphereRadius);
        celestialBody.GetComponent<PhysicsProperty>().velocity = velocity;

        if (orbitSource != null)
        {
            celestialBody.GetComponent<PhysicsProperty>().SetOnOrbit(orbitSource);
        }

        return celestialBody;
    }

    public GameObject CreateMoon(Vector3 position, Vector2 velocity = default(Vector2), GameObject orbitSource = null)
    {
        int mass = Random.Range(MoonMinMass, MoonMaxMass);
        float radius = Random.Range(MoonMinRadius, MoonMaxRadius);
        float AtmosphereFactor = Random.Range(1.05f, MoonMaxAtmosphereFactor);
        float atmosphereRadius = radius * AtmosphereFactor;

        GameObject celestialBody = CreateCelestialBody("MoonBP", position, mass, radius, atmosphereRadius);
        celestialBody.GetComponent<PhysicsProperty>().velocity = velocity;

        if (orbitSource != null)
        {
            celestialBody.GetComponent<PhysicsProperty>().SetOnOrbit(orbitSource);
        }

        return celestialBody;
    }

    public GameObject CreateCelestialBody(
        string type,
        Vector3 position,
        float mass,
        float radius,
        float atmosphereRadius,
        Color color = default(Color)
    )
    {
        GameObject celestialBody = Instantiate(Resources.Load(path + type, typeof(GameObject)), position, Quaternion.identity) as GameObject;

        celestialBody.GetComponent<PhysicsProperty>().Mass = mass;
        celestialBody.GetComponent<PhysicsProperty>().Radius = radius;
        celestialBody.GetComponent<PhysicsProperty>().AtmosphereRadius = atmosphereRadius;

        celestialBody.GetComponent<CelestialBody>().updateSpritesScale();

        // change colour
        if (color == default(Color)) color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        celestialBody.GetComponent<Renderer>().material.color = color;
        color.a = 0.4f;
        celestialBody.GetComponent<CelestialBody>().atmosphereObject.GetComponent<Renderer>().material.color = color;

        return celestialBody;
    }

    private Vector2 RandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return new Vector2(x, y);
    }
}
