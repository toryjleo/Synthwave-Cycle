using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>Class <c>CinematicCamera</c> A Unity Component that controls the camera.</summary>
/// Requires BikeScript in scene
public class CinematicCamera : MonoBehaviour
{

    public CinemachineVirtualCamera virtualCameraTop; // Please set in editor
    // public CinemachineVirtualCamera virtualCameraBrainclap; TODO
    private CinemachineTransposer topTransposer;
    private BikeScript playerBike;

    private const float BODY_MAX_Y_OFFSET = 140;
    private const float BODY_MIN_Y_OFFSET = 72;

    void Awake()
    {
        Init();

    }

    void Update()
    {
        // Testing feature
        /*
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            playerBike.FollowerHeight = BODY_MAX_Y_OFFSET;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerBike.FollowerHeight = BODY_MIN_Y_OFFSET;
        }
        */
    }


    public void Init() 
    {
        // Find object to follow
        bool playerBikeFound = GetPlayerBike();
        if (playerBikeFound)
        {
            InitTopCam();
        }
    }


    /// <summary>Gets the player bike from the scene.</summary>
    /// <returns>True if a player bike has been found in the scene.</returns>
    private bool GetPlayerBike() 
    {
        playerBike = FindObjectOfType<BikeScript>();
        if (playerBike == null)
        {
            Debug.LogError("Cannot find an object with 'BikeScript' component in scene!");
            return false;
        }
        return true;
    }

    /// <summary>Initializes variables for the top camera.</summary>
    private void InitTopCam() 
    {
        topTransposer = virtualCameraTop.GetCinemachineComponent<CinemachineTransposer>();

        virtualCameraTop.LookAt = playerBike.transform;
        virtualCameraTop.Follow = playerBike.CameraFollower;
        playerBike.FollowerHeight = BODY_MIN_Y_OFFSET;
    }
}
