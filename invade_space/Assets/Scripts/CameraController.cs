using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    
    // Camera zoom
    private float elapsedTime;
    private float zoomOut = 15.0f;
    private float scrollZoomSpeed = 1.0f;
    [SerializeField] private float zoomIn = 6.0f;
    [SerializeField] private float zoomOutMin = 10.0f;
    [SerializeField] private float zoomOutMax = 20.0f;
    [SerializeField] private float zoomTime = 1.0f;
    [SerializeField] private AnimationCurve zoomCurve;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // following player
        if (player == null) return;
        
        transform.position = player.transform.position + new Vector3(0, 0, -10);

        // zoom on planet
        elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, zoomTime);
        float procetage = elapsedTime / zoomTime;
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(zoomOut, zoomIn, zoomCurve.Evaluate(procetage));

        if (player.GetComponent<PhysicsProperty>().isKeptOnOrbit)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime -= Time.deltaTime;
            scrollZoom();
        }
    }

    // scroll zoom
    void scrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.0f)
        {
            zoomOut -= scrollZoomSpeed;
        }
        else if (scroll < 0.0f)
        {
            zoomOut += scrollZoomSpeed;
        }
        zoomOut = Mathf.Clamp(zoomOut, zoomOutMin, zoomOutMax);
    }
}
