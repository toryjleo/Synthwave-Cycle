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
        return t * 5.0f;
    }


    float MotionFunctions.GettFromVelocity(float y)
    {
        return y / 5.0f;
    }
}

public class Rational1 : MotionFunctions
{
    public float Acceleration(float t)
    {
        // (1/x^2)
        if (t == 0) 
        {
            return 1;
        }
        else 
        {
            return 1 / (t * t);
        }
    }

    public float GettFromVelocity(float y)
    {
        // Check in morning
        if (y == 0)
        {
            return 0;
        }
        else 
        {
            return 1 / (-y + 1);
        }
    }

    public float Velocity(float t)
    {
        // (1/-x) + 1
        if (t == 0) 
        {
            return 0;
        }
        else
        {
            return (1 / -t) + 1;
        }

    }
}

public class SmoothArrive1 : MotionFunctions
{
    public float Acceleration(float t)
    {
        return -3*(t*t) + 2*t + 1;
    }

    public float GettFromVelocity(float y)
    {
        throw new System.NotImplementedException();
    }

    public float Velocity(float t)
    {
        //return t * (1 - t) + (1 - ((1 - t) * (1 - t))) * t;
        return -(t * t * t) + (t * t) + t;
    }
}