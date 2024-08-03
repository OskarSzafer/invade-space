using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public float maxCameraOrthographicSize = 15.0f;
    public float distanceBetweenMarkers = 8.0f;

    // vertical markers
    [SerializeField] private GameObject verticalMarkerPrefab;
    private GameObject verticalMarker;
    private LineRenderer verticalLine;

    // horizontal markers
    [SerializeField] private GameObject horizontalMarkerPrefab;
    private GameObject horizontalMarker;
    private LineRenderer horizontalLine;


    // Start is called before the first frame update
    void Start()
    {
        maxCameraOrthographicSize *= 2;
        // vertical markers
        verticalMarker = Instantiate(verticalMarkerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        verticalLine = verticalMarker.GetComponent<LineRenderer>();
        // horizontal markers
        horizontalMarker = Instantiate(horizontalMarkerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        horizontalLine = horizontalMarker.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Floor(mainCamera.transform.position.x/distanceBetweenMarkers)*distanceBetweenMarkers;
        float y = Mathf.Floor(mainCamera.transform.position.y/distanceBetweenMarkers)*distanceBetweenMarkers;
        drawVerticalMarkers(x - (maxCameraOrthographicSize*1.5f), distanceBetweenMarkers);
        drawHorizontalMarkers(y + (maxCameraOrthographicSize*1.5f), distanceBetweenMarkers);
    }

    void drawVerticalMarkers(float left, float distance)
    {
        distance /= 2;
        int quantity = Mathf.CeilToInt(maxCameraOrthographicSize*1.5f / distance);
        verticalLine.positionCount = 2*quantity;

        float top = mainCamera.transform.position.y + maxCameraOrthographicSize*1.5f;
        float bot = mainCamera.transform.position.y - maxCameraOrthographicSize*1.5f;

        for (int i = 0; i < 2*quantity - 1; i += 2)
        {
            verticalLine.SetPosition(i, new Vector3(left + i * distance, top, 0));
            verticalLine.SetPosition(i + 1, new Vector3(left + i * distance, bot, 0));
            float tmp = top;
            top = bot;
            bot = tmp;
        }
    }

    void drawHorizontalMarkers(float top, float distance)
    {
        distance /= 2;
        int quantity = Mathf.CeilToInt(maxCameraOrthographicSize*1.5f / distance);
        horizontalLine.positionCount = 2*quantity;

        float left = mainCamera.transform.position.x - maxCameraOrthographicSize*1.5f;
        float right = mainCamera.transform.position.x + maxCameraOrthographicSize*1.5f;

        for (int i = 0; i < 2*quantity - 1; i += 2)
        {
            horizontalLine.SetPosition(i, new Vector3(left, top - i * distance, 0));
            horizontalLine.SetPosition(i + 1, new Vector3(right, top - i * distance, 0));
            float tmp = left;
            left = right;
            right = tmp;
        }
    }
}
