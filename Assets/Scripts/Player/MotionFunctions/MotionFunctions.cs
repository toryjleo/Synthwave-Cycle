using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return t * 5;
    }


    float MotionFunctions.GettFromVelocity(float y)
    {
        return y / 5;
    }
}
