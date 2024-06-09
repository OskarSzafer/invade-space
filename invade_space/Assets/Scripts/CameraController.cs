using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    
    // Camera zoom
    private float elapsedTime;
    [SerializeField] private float zoomIn = 4.0f;
    [SerializeField] private float zoomOut = 10.0f;
    [SerializeField] private float zoomTime = 1.0f;
    [SerializeField] private AnimationCurve zoomCurve;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        
        transform.position = player.transform.position + new Vector3(0, 0, -10);

        // Camera zoom
        elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, zoomTime);
        float procetage = elapsedTime / zoomTime;
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(zoomOut, zoomIn, zoomCurve.Evaluate(procetage));

        if (player.GetComponent<PhysicsProperty>().KeptOnOrbit)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime -= Time.deltaTime;
        }
    }
}
