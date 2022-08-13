using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRendererScript : MonoBehaviour
{
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null) 
        {
            Debug.LogError("CircleRendererScript cannot get reference to LineRenderer component!");
        }
        DrawCircle(40, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawCircle(int steps, float radius) 
    {
        lineRenderer.positionCount = steps + 1;

        for (int currentStep = 0; currentStep < steps + 1; currentStep++) 
        {
            float progressAroundPerimeter = (float)currentStep / (float)steps;

            float currentRadian = progressAroundPerimeter * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0);

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
