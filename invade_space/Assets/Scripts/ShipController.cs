using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    // ship speed
    public float force = 10.0f;
    // ship rotation speed
    protected float rotationRate = 100.0f;

    // input
    protected Vector2 input = Vector2.zero;
    protected float inputRotation = 0.0f;
    protected float inputThrust = 0.0f;

    // thruster
    protected GameObject thruster;
    protected Transform thrusterTransform;
    protected Vector2 thrustDirection;
    protected ParticleSystem thrusterParticleSystem;


    // Awake is called before the first frame update
    void Awake()
    {
        // Set the ship's position to the starting position
        thruster = GameObject.Find("Thruster");
        thrusterTransform = thruster.GetComponent<Transform>();
        thrusterParticleSystem = thruster.GetComponentInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        inputThrust = input.magnitude;
        input.Normalize();

        ThrustControll();
    }

    // FUNCTIONS

    protected void ThrustControll()
    {
        float rotationAmount = rotationRate * Time.deltaTime;
        Debug.Log(rotationAmount);

        // turn the thruster on or off
        if (inputThrust > 0)
        {
            //thrusterParticleSystem.emission.enabled = true;
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
            //thrusterParticleSystem.emission.enabled = false;
            var emission = thrusterParticleSystem.emission;
            emission.enabled = false;
        }
    }

    protected void Thrust()
    {
        //thrustDirection = new Vector2(Mathf.Cos(thrusterTransform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(thrusterTransform.eulerAngles.z * Mathf.Deg2Rad));
        //GetComponent<Rigidbody2D>().AddForce(thrustDirection * force);
    }
}
