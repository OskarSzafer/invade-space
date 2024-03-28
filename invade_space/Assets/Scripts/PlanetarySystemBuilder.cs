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
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject sun = MakeStar(new Vector3(0, 0, 0), 10000000, new Color(1.0f, 0.87f, 0.13f, 1.0f));

            GameObject mercury = MakeRockyPlanet(new Vector3(0, 300, 0), 100, new Color(0.76f, 0.70f, 0.50f, 1.0f));
            GameObject venus = MakeRockyPlanet(new Vector3(0, 370, 0), 100, new Color(0.80f, 0.50f, 0.20f, 1.0f));
            GameObject earth = MakeRockyPlanet(new Vector3(0, 440, 0), 100, new Color(0.20f, 0.60f, 0.80f, 1.0f));
            GameObject mars = MakeRockyPlanet(new Vector3(0, 510, 0), 100, new Color(0.80f, 0.20f, 0.20f, 1.0f));
            GameObject jupiter = MakeGasPlanet(new Vector3(0, 600, 0), 100, new Color(0.80f, 0.60f, 0.40f, 1.0f));
            GameObject saturn = MakeGasPlanet(new Vector3(0, 700, 0), 80, new Color(0.76f, 0.70f, 0.50f, 1.0f));
            GameObject uranus = MakeGasPlanet(new Vector3(0, 800, 0), 70, new Color(0.0f, 0.8f, 1.0f, 1.0f));
            GameObject neptune = MakeGasPlanet(new Vector3(0, 900, 0), 70, new Color(0.0f, 0.2f, 1.0f, 1.0f));

            mercury.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            venus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            earth.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            mars.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            jupiter.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            saturn.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            uranus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
            neptune.GetComponent<PhysicsProperty>().SetOnOrbit(sun);

        }
    }

    public GameObject CreateCelestialBody(
        GameObject celestialBodyPrefab,
        Vector3 position,
        float mass,
        float radius,
        float atmosphereRadius,
        Color color = default(Color)
    )
    {
        celestialBody = Instantiate(celestialBodyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        celestialBody.transform.position = position;

        celestialBody.GetComponent<PhysicsProperty>().Mass = mass;
        celestialBody.GetComponent<PhysicsProperty>().Radius = radius;
        celestialBody.GetComponent<PhysicsProperty>().AtmosphereRadius = atmosphereRadius;

        celestialBody.GetComponent<CelestialBody>().updateSpritesScale();

        // change colour
        if (color == default(Color)) color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        celestialBody.GetComponent<Renderer>().material.color = color;
        color.a = 0.4f;
        celestialBody.GetComponent<CelestialBody>().atmosphereObject.GetComponent<Renderer>().material.color = color;

        return celestialBody;
    }

    public GameObject MakeStar(Vector3 position, float mass, Color color)
    {
        float radiusFactor = 0.00001f;
        float radiusesDependency = 1.3f;

        float radius = mass * radiusFactor;
        float atmosphereRadius = radius * radiusesDependency;

        return CreateCelestialBody(starPrefab, position, mass, radius, atmosphereRadius, color);
    }

    public GameObject MakeGasPlanet(Vector3 position, float mass, Color color, float radiusesDependency = 1.5f)
    {
        float radiusFactor = 0.3f;

        float radius = mass * radiusFactor;
        float atmosphereRadius = radius * radiusesDependency;

        return CreateCelestialBody(gasPlanetPrefab, position, mass, radius, atmosphereRadius, color);
    }

    public GameObject MakeRockyPlanet(Vector3 position, float mass, Color color, float radiusesDependency = 1.3f)
    {
        float radiusFactor = 0.15f;

        float radius = mass * radiusFactor;
        float atmosphereRadius = radius * radiusesDependency;

        return CreateCelestialBody(rockyPlanetPrefab, position, mass, radius, atmosphereRadius, color);
    }
}
