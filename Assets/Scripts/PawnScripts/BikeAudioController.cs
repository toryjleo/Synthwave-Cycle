using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BikeAudioController</c> Changes the noises the engine makes.</summary>
public class BikeAudioController : MonoBehaviour
{
    private AudioSource engineSound;
    private const float MIN_PITCH = .7f;
    private const float MAX_PITCH = 1.25f;
    private float currentPitch;

    private float pitchIncreasePerSecond = .1f;
    private float pitchDecreasePerSecond = .3f;


    // Start is called before the first frame update
    void Awake()
    {
        engineSound = GetComponent<AudioSource>();
        currentPitch = MIN_PITCH;
        InitEngineSound();
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            IncreasePitch(pitchIncreasePerSecond * Time.deltaTime);
        }
        else 
        {
            IncreasePitch(-pitchDecreasePerSecond * Time.deltaTime);
        }
    }

    /// <summary>Initializes the settings for the engine sound.</summary>
    private void InitEngineSound() 
    {
        engineSound.loop = true;
        engineSound.pitch = .7f;
        engineSound.Play();
    }

    /// <summary>Increases the pitch of the engine.</summary>
    /// <param name="amount">The amount by which to increase the pitch.</param>
    private void IncreasePitch(float amount) 
    {
        currentPitch = Mathf.Clamp(currentPitch + amount, MIN_PITCH, MAX_PITCH);
        engineSound.pitch = currentPitch;
    }

    /// <summary>Returns the number of units between the maximum and minimum pitch.</summary>
    /// <returns>MAX_PITCH - MIN_PITCH.</returns>
    private float PitchRange() 
    {
        return MAX_PITCH - MIN_PITCH;
    }
}
