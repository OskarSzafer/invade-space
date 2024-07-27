using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetUI2 : MonoBehaviour
{
    [SerializeField] private GameObject orbitMarkerPrefab;
    private GameObject orbitMarker;
    private LineRenderer orbitLine;
    protected PhysicsProperty physicsProperty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable() {
        physicsProperty = GetComponent<PhysicsProperty>();
        orbitMarker = Instantiate(orbitMarkerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        orbitLine = orbitMarker.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 orbitCenter = physicsProperty.OrbitCenter();
        float orbitRadius = (orbitCenter - (Vector2)transform.position).magnitude;
        if (float.IsNaN(orbitRadius))
        {
            orbitLine.positionCount = 0;
        }
        else
        {
            drowCircle(orbitCenter, orbitRadius, 360);
        }
    }

    void drowCircle(Vector2 center, float radius, int segments)
    {
        float angle = 0f;

        orbitLine.positionCount = segments + 1;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            orbitLine.SetPosition(i, new Vector3(x + center.x, y + center.y, 0));

            angle += 360f / segments;
        }
    }

    void OnDisable() {
        Destroy(orbitMarker);
    }
}
