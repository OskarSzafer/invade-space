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
    protected Transform thrusterTransform;
    // protected Vector2 thrustDirection;
    protected ParticleSystem thrusterParticleSystem;


    // Awake is called before the first frame update
    void Awake()
    {
        thruster = GameObject.Find("Thruster");
        thrusterTransform = thruster.GetComponent<Transform>();
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
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        inputThrust = input.magnitude;
        input.Normalize();

        ThrustControll();
        
        if (inputThrust > 0)
        {
            Thrust();
        }

        // INPUT
        if (Input.GetKeyDown(KeyCode.F)) //orbit nearest celestial body
        {
            GameObject source = physicsProperty.NearestGravitySource();
            physicsProperty.SetOnOrbit(source);
            physicsProperty.KeepOnOrbit(source);
        }
    }


    protected void ThrustControll()
    {
        float rotationAmount = rotationRate * Time.deltaTime;

        // turn the thruster on or off
        if (inputThrust > 0)
        {
            var emission = thrusterParticleSystem.emission;
            emission.enabled = true;

            inputRotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg + 90.0f;
            float currentRotation = thrusterTransform.eulerAngles.z;

            float rotation = Mathf.DeltaAngle(currentRotation, inputRotation);
            rotation = Mathf.Clamp(rotation, -rotationAmount, rotationAmount);

            thrusterTransform.rotation = Quaternion.Euler(0, 0, currentRotation + rotation);
        }
        else
        {
            var emission = thrusterParticleSystem.emission;
            emission.enabled = false;
        }
    }

    protected void Thrust()
    {
        Vector2 force = thrusterTransform.rotation * Vector2.down * thrustForce * Time.deltaTime;
        physicsProperty.ApplyForce(force);
    }
}

// TODO:
// - add manual rotation
// - set on orbit works only for if alredy in aproximate velocity