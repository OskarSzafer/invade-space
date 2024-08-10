using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : Ship
{
    // INPUT
    protected Vector2 input = Vector2.zero;
    protected float inputRotation = 0.0f;
    protected float inputThrust = 0.0f;

    // THRUSTER
    protected GameObject thruster;
    // protected Vector2 thrustDirection;
    protected ParticleSystem thrusterParticleSystem;


    // Awake is called before the first frame update
    void Awake()
    {
        thruster = transform.Find("Thruster").gameObject;
        thrusterParticleSystem = thruster.GetComponentInChildren<ParticleSystem>();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Steering
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        inputThrust = input.magnitude;

        if (inputThrust > 0.5f){
            inputRotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg - 90.0f;
        }
        else
        {
            inputRotation = transform.eulerAngles.z;
        }

        // Thrust control
        Thrust(Input.GetKey("space"));
        RotationControll();

        //orbit nearest celestial body
        if (Input.GetKeyDown(KeyCode.F))
        {
            Orbit();
        }

        // make structure
        if (physicsProperty.isKeptOnOrbit && physicsProperty.GetOrbitSource.GetComponent<StructureMaker>() != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                List<string> structuresTypes = physicsProperty.GetOrbitSource.GetComponent<StructureMaker>().structuresTypes;
                physicsProperty.GetOrbitSource.GetComponent<StructureMaker>().makeStructure(structuresTypes[0], transform.position);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                List<string> structuresTypes = physicsProperty.GetOrbitSource.GetComponent<StructureMaker>().structuresTypes;
                physicsProperty.GetOrbitSource.GetComponent<StructureMaker>().makeStructure(structuresTypes[1], transform.position);
            }
        }

        // test
        GameObject raycasted = physicsProperty.Raycast((Vector2)transform.position, new Vector2(0, 0), new string[] {"Star", "Planet", "Moon"});
        if (raycasted != null)
        {
            Debug.Log(raycasted.name);
        }else{
            Debug.Log("No object");
        }
    }


    // rotate gameObject to inputRotation, with speed of rotationRate
    protected void RotationControll()
    {
        float rotationAmount = rotationRate * Time.deltaTime;
        float currentRotation = transform.eulerAngles.z;

        float rotation = Mathf.DeltaAngle(currentRotation, inputRotation);
        rotation = Mathf.Clamp(rotation, -rotationAmount, rotationAmount);

        transform.rotation = Quaternion.Euler(0, 0, currentRotation + rotation);
    }

    // apply force and handle thruster particle system
    protected void Thrust(bool thrusterActice)
    {
        if (thrusterActice)
        {
            var emission = thrusterParticleSystem.emission;
            emission.enabled = true;

            Vector2 force = transform.rotation * Vector2.down * thrustForce * Time.deltaTime * -1;
            physicsProperty.ApplyForce(force);
        }
        else
        {
            var emission = thrusterParticleSystem.emission;
            emission.enabled = false;
        }
    }

    protected void Orbit()
    {
        GameObject source = physicsProperty.NearestGravitySource();
        Debug.Log(source.name);
        physicsProperty.KeepOnOrbit(source, 7.0f);
        Invoke("SetOrbitVelocity", 0.2f);
    }

    private void SetOrbitVelocity()
    {
        if (!physicsProperty.isKeptOnOrbit) return;
        physicsProperty.SetOnOrbit(physicsProperty.GetOrbitSource);
    }
}