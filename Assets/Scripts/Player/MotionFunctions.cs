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
    public float xScale = 1.0f;

    public float Acceleration(float x)
    {
        float coshX = Unity.Mathematics.math.cosh(xScale * x);
        return Mathf.Clamp(1 /(coshX * coshX), -1.0f, 1.0f);
    }

    public float GetXFromVelocity(float y)
    {
        float yClamped = Mathf.Clamp(y, -1.0f, 1.0f);
        return 1/xScale * .5f * Mathf.Log((1.0f + yClamped) / (1.0f - yClamped));
    }

    public float Velocity(float x)
    {
        return Mathf.Clamp(Unity.Mathematics.math.tanh(xScale * x), -1.0f, 1.0f);
    }
}