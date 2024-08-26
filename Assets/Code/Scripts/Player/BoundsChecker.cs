using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks and triggers logic related to the player going out of bounds
/// Attach to a gameobject underneath player
/// Requires a TransmissionArea in scene
/// </summary>
public class BoundsChecker : MonoBehaviour, IResettable
{
    #region Members
    /// <summary>
    /// Reference to the transmissionArea
    /// </summary>
    private TransmissionArea transmissionArea;

    /// <summary>
    /// Maximum amount of time (in seconds) you can remain out of bounds before game over
    /// </summary>
    private const float MAX_TIME = 10.0f;

    /// <summary>
    /// Reference to timer object
    /// </summary>
    private Timer timer;

    public NotifyCrossedTransmissionAreaBounds onCrossedTransmissionBounds;

    private bool wasLastWithinBounds = true;
    #endregion

    #region Type Definitions

    public delegate void NotifyCrossedTransmissionAreaBounds(bool isWithinBounds);

    public delegate void NotifyTimerStateChange(bool timerIsOn);

    /// <summary>
    /// Class that counts up to an elapsed time
    /// </summary>
    private class Timer
    {
        private float timeElapsed = 0;

        private float maxTime = 10.0f;

        private bool timerOn = false;

        public NotifyTimerStateChange ontimerStateChanged;
        public NotifyTimerStateChange onTimerComplete;

        /// <summary>
        /// If the timer has started runnung
        /// </summary>
        public bool HasStarted
        {
            get => timerOn;
        }

        /// <summary>
        /// How much time until timer finishes
        /// </summary>
        public float TimeLeft
        {
            get { return maxTime - timeElapsed; }
        }

        /// <summary>
        /// Percentage of the full time used up
        /// </summary>
        public float TimePercentage
        {
            get { return timeElapsed / maxTime; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxTime">The span of time the timer is tracking</param>
        public Timer(float maxTime)
        {
            this.maxTime = maxTime;
            this.timeElapsed = 0;
            this.timerOn = false;
        }

        /// <summary>
        /// Called every update
        /// </summary>
        /// <param name="deltaTime">Amount of time since last tick</param>
        public void Tick(float deltaTime)
        {
            if (timerOn)
            {
                timeElapsed += deltaTime;

                if (timeElapsed > maxTime)
                {
                    onTimerComplete?.Invoke(false);
                    TimerReset();
                }
            }
        }

        /// <summary>
        /// Starts the timer from 0 seconds
        /// </summary>
        public void TimerStart()
        {
            if (GameStateController.CanRunGameplay)
            {
                timerOn = true;
                ontimerStateChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Resets the timer's values to run again
        /// </summary>
        public void TimerReset()
        {
            timeElapsed = 0;
            timerOn = false;
            ontimerStateChanged?.Invoke(false);
        }
    }

    #endregion

    #region Properties
    /// <summary>
    /// Returns true if the player has made it outside the playable bounds
    /// </summary>
    private bool PlayerIsOutsideBounds
    {
        get
        {
            if (transmissionArea == null)
            {
                Debug.LogError("Trying to find if player outside bounds when there is no TransmissionArea found");
                return true;
            }

            float distanceSqrToTransmissionAreaCenter = (transform.position - transmissionArea.transform.position).sqrMagnitude;

            return distanceSqrToTransmissionAreaCenter > transmissionArea.MaxBoundsFromTransmissionAreaSqr;
        }
    }
    private bool PlayerIsInsideTransmissionArea 
    {
        get 
        {
            if (transmissionArea == null)
            {
                Debug.LogError("Trying to find if player outside bounds when there is no TransmissionArea found");
                return false;
            }
            float distanceSqrToTransmissionAreaCenter = (transform.position - transmissionArea.transform.position).sqrMagnitude;

            return distanceSqrToTransmissionAreaCenter < transmissionArea.RadiusSqr;
        }
    }

    public float TransmissionClarity
    {
        get 
        {
            if (transmissionArea == null)
            {
                Debug.LogError("Trying to get transmission clarity when there is no TransmissionArea found");
                return -1;
            }
            else 
            {
                return transmissionArea.TransmissionClarity(transform.position);
            }
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

    /// <summary>
    /// Rising Percentage until timer is completed
    /// </summary>
    public float TimePercentage
    {
        get
        {
            if (timer == null) { return -1; }
            else { return timer.TimePercentage; }
        }
    }

    /// <summary>
    /// Notifies the listeners that a timer event has triggered.
    /// Timer events trigger when the player goes out of bounds or enters the bounds again.
    /// </summary>
    public NotifyTimerStateChange OnTimerStateChanged
    {
        get
        {
            if (timer == null)
            {
                Debug.LogError("Hooking up an event to a null object");
                return null;
            }
            else
            {
                return timer.ontimerStateChanged;
            }
        }
        set { timer.ontimerStateChanged = value; }
    }
    #endregion

    void Awake()
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
        if (GameStateController.CanRunGameplay)
        {
            UpdateTimer(Time.deltaTime);

            CheckIfCrossedBounds();
        }
    }

    #region Transmission Area

    /// <summary>
    /// Finds transmission area in scene and returns true if it is found.
    /// </summary>
    /// <returns>True if it can find a TransmissionArea in scene. Else false</returns>
    private bool CanFindTransmissionArea()
    {
        transmissionArea = FindObjectOfType<TransmissionArea>();
        return transmissionArea != null;
    }

    /// <summary>
    /// Checks if the player crossed the TransmissionArea bounds this frame
    /// </summary>
    private void CheckIfCrossedBounds() 
    {
        if (wasLastWithinBounds != PlayerIsInsideTransmissionArea) 
        {
            wasLastWithinBounds = PlayerIsInsideTransmissionArea;
            onCrossedTransmissionBounds?.Invoke(PlayerIsInsideTransmissionArea);
        }
    }

    #endregion

    #region Timer

    /// <summary>
    /// Set timer to its initial state
    /// </summary>
    private void InitTimer()
    {
        timer = new Timer(MAX_TIME);

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null) { timer.onTimerComplete += playerHealth.KillPlayer; }
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
            timer.TimerStart();
        }
    }

    public void ResetGameObject()
    {
        wasLastWithinBounds = true;
    }
    #endregion
}
