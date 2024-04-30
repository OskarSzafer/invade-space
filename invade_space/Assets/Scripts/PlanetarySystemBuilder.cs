using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetarySystemBuilder : MonoBehaviour
{
    // prefabs
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject gasPlanetPrefab;
    [SerializeField] private GameObject rockyPlanetPrefab;
    [SerializeField] private GameObject moonPrefab;
    //[SerializeField] private GameObject shipPrefab;

    GameObject celestialBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //MakeSolarSystem();
            StartCoroutine(MakeSolarSystemAnimeted());
            Invoke("RainOfDebris", 8.6f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            //MakeSolarSystem();
            StartCoroutine(MakeSolarSystemAnimeted());
            Invoke("RoguePlanet", 5.3f);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            //MakeSolarSystem();
            StartCoroutine(MakeSolarSystemAnimeted());
            Invoke("SystemsCollision", 8.5f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RainOfDebris();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            RoguePlanet();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SystemsCollision();
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

    public GameObject MakeGasPlanet(Vector3 position, float mass, Color color = default(Color), float radiusesDependency = 1.4f)
    {
        float radiusFactor = 0.003f; //0.02f //calculated for saturn

        float radius = mass * radiusFactor;
        float atmosphereRadius = radius * radiusesDependency;

        return CreateCelestialBody(gasPlanetPrefab, position, mass, radius, atmosphereRadius, color);
    }

    public GameObject MakeRockyPlanet(Vector3 position, float mass, Color color = default(Color), float radiusesDependency = 1.2f)
    {
        float radiusFactor = 0.2f;

        float radius = mass * radiusFactor;
        float atmosphereRadius = radius * radiusesDependency;

        return CreateCelestialBody(rockyPlanetPrefab, position, mass, radius, atmosphereRadius, color);
    }

    // ready to use 
    public void MakeSolarSystem(Vector3 sunPosition, Vector3 velocity)
    {
        // 1 mass unit = 1e24 kg
        GameObject sun = MakeStar(sunPosition, 2000000, new Color(1.0f, 0.87f, 0.13f, 1.0f));
        sun.GetComponent<PhysicsProperty>().velocity = velocity;

        GameObject mercury = CreateCelestialBody(rockyPlanetPrefab, new Vector3(0, -45, 0) + sunPosition, 0.3f, 1.4f, 1.4f*1.1f, new Color(0.76f, 0.70f, 0.50f, 1.0f));
        GameObject venus = CreateCelestialBody(rockyPlanetPrefab, new Vector3(50, 0, 0) + sunPosition, 4.8f, 2.0f, 2.0f*1.3f, new Color(0.80f, 0.50f, 0.20f, 1.0f));
        GameObject earth = CreateCelestialBody(rockyPlanetPrefab, new Vector3(0, 55, 0) + sunPosition, 6f, 1.5f, 1.5f*1.3f, new Color(0.20f, 0.60f, 0.80f, 1.0f));
        GameObject mars = CreateCelestialBody(rockyPlanetPrefab, new Vector3(60, 0, 0) + sunPosition, 0.65f, 1.0f, 1.0f*1.1f, new Color(0.80f, 0.20f, 0.20f, 1.0f));
        GameObject jupiter = CreateCelestialBody(gasPlanetPrefab, new Vector3(0, 80, 0) + sunPosition, 190.0f, 5.0f, 5.0f*1.4f, new Color(0.80f, 0.60f, 0.40f, 1.0f)); // mass 10 times smaller
        GameObject saturn = CreateCelestialBody(gasPlanetPrefab, new Vector3(0, -100, 0) + sunPosition, 57.0f, 4.0f, 4.0f*1.3f, new Color(0.76f, 0.70f, 0.50f, 1.0f)); // mass 10 times smaller
        GameObject uranus = CreateCelestialBody(gasPlanetPrefab, new Vector3(-110, 0, 0) + sunPosition, 8.6f, 2.6f, 2.6f*1.3f, new Color(0.0f, 0.8f, 1.0f, 1.0f)); // mass 10 times smaller
        GameObject neptune = CreateCelestialBody(gasPlanetPrefab, new Vector3(85, 85, 0) + sunPosition, 10.2f, 2.4f, 2.4f*1.25f, new Color(0.0f, 0.2f, 1.0f, 1.0f));

        mercury.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        venus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        earth.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        mars.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        jupiter.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        saturn.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        uranus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        neptune.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
    }

    public IEnumerator MakeSolarSystemAnimeted()
    {
        // 1 mass unit = 1e24 kg
        GameObject sun = MakeStar(new Vector3(0, 0, 0), 2000000, new Color(1.0f, 0.87f, 0.13f, 1.0f));
        yield return new WaitForSeconds(0.5f);

        GameObject mercury = CreateCelestialBody(rockyPlanetPrefab, new Vector3(0, -45, 0), 0.3f, 1.4f, 1.4f*1.1f, new Color(0.76f, 0.70f, 0.50f, 1.0f));
        mercury.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject venus = CreateCelestialBody(rockyPlanetPrefab, new Vector3(50, 0, 0), 4.8f, 2.0f, 2.0f*1.3f, new Color(0.80f, 0.50f, 0.20f, 1.0f));
        venus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject earth = CreateCelestialBody(rockyPlanetPrefab, new Vector3(0, 55, 0), 6f, 1.5f, 1.5f*1.3f, new Color(0.20f, 0.60f, 0.80f, 1.0f));
        earth.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject mars = CreateCelestialBody(rockyPlanetPrefab, new Vector3(60, 0, 0), 0.65f, 1.0f, 1.0f*1.1f, new Color(0.80f, 0.20f, 0.20f, 1.0f));
        mars.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject jupiter = CreateCelestialBody(gasPlanetPrefab, new Vector3(0, 80, 0), 190.0f, 5.0f, 5.0f*1.4f, new Color(0.80f, 0.60f, 0.40f, 1.0f)); // mass 10 times smaller
        jupiter.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject saturn = CreateCelestialBody(gasPlanetPrefab, new Vector3(0, -100, 0), 57.0f, 4.0f, 4.0f*1.3f, new Color(0.76f, 0.70f, 0.50f, 1.0f)); // mass 10 times smaller
        saturn.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject uranus = CreateCelestialBody(gasPlanetPrefab, new Vector3(-110, 0, 0), 8.6f, 2.6f, 2.6f*1.3f, new Color(0.0f, 0.8f, 1.0f, 1.0f)); // mass 10 times smaller
        uranus.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
        yield return new WaitForSeconds(0.2f);
        GameObject neptune = CreateCelestialBody(gasPlanetPrefab, new Vector3(85, 85, 0), 10.2f, 2.4f, 2.4f*1.25f, new Color(0.0f, 0.2f, 1.0f, 1.0f));
        neptune.GetComponent<PhysicsProperty>().SetOnOrbit(sun);
    }


    public void RainOfDebris()
    {
        float mass = 0.1f;
        float radius = 0.4f;
        float atmosphereRadius = radius;

        for (int i = 0; i < 80; i++)
        {
            Vector3 position = new Vector3(Random.Range(-100, 100), Random.Range(-230, -330), 0);
            
            GameObject debris = CreateCelestialBody(moonPrefab, position, mass, radius, atmosphereRadius, new Color(0.85f, 0.20f, 0.20f, 1.0f));
            debris.GetComponent<PhysicsProperty>().velocity = new Vector2(0, Random.Range(10.0f, 15.0f));
        }
    }

    public void RoguePlanet()
    {
        GameObject planet = CreateCelestialBody(gasPlanetPrefab, new Vector3(300, 80, 0), 15.0f, 2.6f, 2.6f*1.3f, new Color(0.2f, 0.5f, 0.4f, 1.0f));
        planet.GetComponent<PhysicsProperty>().velocity = new Vector2(-70.0f, 0.0f);
    }

    public void SystemsCollision()
    {
        MakeSolarSystem(new Vector3(300, 0, 0), new Vector2(-40.0f, 0.0f));
    }
}
