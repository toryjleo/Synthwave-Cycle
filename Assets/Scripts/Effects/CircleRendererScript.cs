using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders a circle. Expects a connected lineRenderer.
/// </summary>
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
        
    }

    /// <summary>
    /// Draws a circle using a lineRenderer
    /// </summary>
    /// <param name="center"> The center of the circle to be drawn. </param>
    /// <param name="steps"> How many sections the circle will be broken up in to. </param>
    /// <param name="radius"> The radius of the circle to be drawn. </param>
    public void DrawCircle(Vector3 center, int steps, float radius) 
    {
        lineRenderer.positionCount = steps + 1;

        for (int currentStep = 0; currentStep < steps + 1; currentStep++) 
        {
            float progressAroundPerimeter = (float)currentStep / (float)steps;

            float currentRadian = progressAroundPerimeter * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float zScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float z = zScaled * radius;

            Vector3 currentPosition = new Vector3(x, 0, z) + center;

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
