using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            playerBike.FollowerHeight = BODY_MAX_Y_OFFSET;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerBike.FollowerHeight = BODY_MIN_Y_OFFSET;
        }
    }


    public void Init() 
    {
        // Find object to follow
        playerBike = FindObjectOfType<BikeScript>();
        if (playerBike == null)
        {
            Debug.LogError("Cannot find an object with 'BikeScript' component in scene!");
        }

        InitTopCam();
    }


    private void InitTopCam() 
    {
        topTransposer = virtualCameraTop.GetCinemachineComponent<CinemachineTransposer>();

        virtualCameraTop.LookAt = playerBike.transform;
        virtualCameraTop.Follow = playerBike.CameraFollower;
        playerBike.FollowerHeight = BODY_MIN_Y_OFFSET;
    }
}
