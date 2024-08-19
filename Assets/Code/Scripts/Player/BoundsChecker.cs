using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    /// <summary>
    /// Reference to the transmissionArea
    /// </summary>
    private TransmissionArea transmissionArea;

    /// <summary>
    /// Linearly scales the "in bounds" area
    /// </summary>
    private float outOfBoundsScale = 2.0f;

    private const float MAX_TIME = 10.0f;

    private Timer timer;



    private class Timer
    {
        private float timeElapsed = 0;

        private float maxTime = 10.0f;

        private bool timerOn = false;

        public bool HasStarted 
        {
            get => timerOn;
        }

        public float TimeLeft 
        {
            get { return maxTime - timeElapsed; }
        }

        public Timer(float maxTime)
        {
            this.maxTime = maxTime;
            this.timeElapsed = 0;
            this.timerOn = false;
        }

        public void Tick(float deltaTime) 
        {
            if (timerOn) 
            {
                timeElapsed += deltaTime;
                Debug.Log("timer at " + timeElapsed + " seconds");

                if (timeElapsed > maxTime) 
                {
                    // TODO: trigger event to kill player and end game
                    TimerReset();
                }
            }
        }

        public void TimerStart()
        {
            if (GameStateController.CanRunGameplay) 
            {
                timerOn = true;
                // TODO: trigger event to start game ui timer
            }
        }

        public void TimerReset()
        {
            timeElapsed = 0;
            timerOn = false;
            // TODO: trigger event to disable game ui timer
        } 
    }

    /// <summary>
    /// Returns true if the player has made it outside the playable bounds
    /// </summary>
    private bool PlayerIsOutsideBounds
    {
        get {
            if (transmissionArea == null) 
            {
                Debug.LogError("Trying to find if player outside bounds when there is no TransmissionArea found");
                return true;
            }

            float distanceSqrToTransmissionAreaCenter = (transform.position - transmissionArea.transform.position).sqrMagnitude;
            float maxDistanceSqrFromTransmissionAreaCenter = transmissionArea.Width * transmissionArea.Width;

            return distanceSqrToTransmissionAreaCenter >
            (outOfBoundsScale * outOfBoundsScale * maxDistanceSqrFromTransmissionAreaCenter);
        }
    }

    /// <summary>
    /// Returns how much time is left on the timer
    /// </summary>
    public float TimeLeft 
    {
        get 
        { 
            if (timer == null) { return -1; }
            else { return timer.TimeLeft; } 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find TransmissionArea, disable if not found
        if (!CanFindTransmissionArea()) 
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning("BoundsChecker did not find a TransmissionArea in scene. Disabling bounds");
        }

        InitTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.GameIsPlaying()) 
        {
            UpdateTimer(Time.deltaTime);
        }
    }

    /// <summary>
    /// Finds transmission area in scene and returns true if it is found.
    /// </summary>
    /// <returns>True if it can find a TransmissionArea in scene. Else false</returns>
    private bool CanFindTransmissionArea() 
    {
        transmissionArea = FindObjectOfType<TransmissionArea>();
        return transmissionArea == null;
    }

    /// <summary>
    /// Set timer to its initial state
    /// </summary>
    private void InitTimer() 
    {
        timer = new Timer(MAX_TIME);
    }

    /// <summary>
    /// Handles timer logic during gametime
    /// </summary>
    /// <param name="deltaTime">The amound of time since last frame</param>
    private void UpdateTimer(float deltaTime) 
    {
        if (timer.HasStarted)
        {
            if (!PlayerIsOutsideBounds)
            {
                // in bounds
                Debug.Log("In bounds");
                timer.TimerReset();
            }
            else
            {
                // out of bounds
                timer.Tick(deltaTime);
            }
        }
        else if (PlayerIsOutsideBounds)
        {
            // outside of bounds
            Debug.Log("Out of bounds");
            timer.TimerStart();
        }
    }
}
