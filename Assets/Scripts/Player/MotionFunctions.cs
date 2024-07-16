using UnityEngine;

/// <summary>
/// Used for defining the forward motion of the player
/// </summary>
public interface MotionFunctions
{

    public float GettFromVelocity(float y);

    public float Velocity(float t);

    public float Acceleration(float t);
}

public class LinearVelocity : MotionFunctions
{
    public float Acceleration(float t)
    {
        return 5;
    }

    public float Velocity(float t)
    {
        return t * 5.0f;
    }


    float MotionFunctions.GettFromVelocity(float y)
    {
        return y / 5.0f;
    }
}

public class Sigmoid1 : MotionFunctions
{
    private float xScale = 4.0f;

    public float Acceleration(float t)
    {
        float cosht = Unity.Mathematics.math.cosh(xScale * t);
        return 1/(cosht * cosht);
    }

    public float GettFromVelocity(float y)
    {
        // Need to check this
        return Mathf.Clamp((.5f * Mathf.Log((1.0f + y) / (1.0f - y))) / xScale, -1.0f, 1.0f);
    }

    public float Velocity(float t)
    {
        return Unity.Mathematics.math.tanh(xScale * t);
    }
}