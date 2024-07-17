using UnityEngine;

/// <summary>
/// Used for defining the forward motion of the player
/// </summary>
public interface MotionFunctions
{

    public float GetXFromVelocity(float y);

    public float Velocity(float x);

    public float Acceleration(float x);
}

public class LinearVelocity : MotionFunctions
{
    public float Acceleration(float x)
    {
        return 5;
    }

    public float Velocity(float x)
    {
        return x * 5.0f;
    }


    float MotionFunctions.GetXFromVelocity(float y)
    {
        return y / 5.0f;
    }
}

public class Sigmoid1 : MotionFunctions
{
    public float xScale = 4.0f;

    public float Acceleration(float x)
    {
        float coshX = Unity.Mathematics.math.cosh(xScale * x);
        return 1/(coshX * coshX);
    }

    public float GetXFromVelocity(float y)
    {
        return Mathf.Clamp((.5f * Mathf.Log((1.0f + y) / (1.0f - y))), -1.0f, 1.0f);
    }

    public float Velocity(float x)
    {
        return Unity.Mathematics.math.tanh(xScale * x);
    }
}