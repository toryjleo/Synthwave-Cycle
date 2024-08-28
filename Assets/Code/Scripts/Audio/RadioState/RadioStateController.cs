using RadioState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class RadioStateController : MonoBehaviour
{
    #region Outside References
    private BoundsChecker boundsChecker;
    private Jukebox jukebox;
    #endregion



    private static RadioState.State state;

    public RadioState.RadioOff radioOff;
    public RadioState.RadioOn radioOn;
    public RadioState.InBounds inBounds;
    public RadioState.OutOfBounds outOfBounds;
    public RadioState.RadioPlaying radioPlaying;
    public RadioState.RadioNotPlaying radioNotPlaying;

    private static bool initialEnter = false;

    public static bool InitialEnter
    {
        get => initialEnter;
    }

    public static bool StateExists
    {
        get => state != null;
    }

    public bool IsInBounds
    {
        get => boundsChecker.IsInBounds;
    }

    #region MonoBehavior
    /// <summary>
    /// Object referencing a gamestate should hook up events in OnEnable()
    /// </summary>
    private void Awake()
    {
        // TODO: Initialize all state classes
        radioOff = new RadioOff();
        radioOn = new RadioOn();
        inBounds = new InBounds();
        outOfBounds = new OutOfBounds();
        radioPlaying = new RadioPlaying();
        radioNotPlaying = new RadioNotPlaying();

        state = radioOff;
        initialEnter = false;

    }

    private void Start()
    {
        boundsChecker = FindObjectOfType<BoundsChecker>();
        if (boundsChecker != null)
        {
            radioOn.notifyListenersEnter += boundsChecker.HandleRadioOnEvent;
            boundsChecker.onCrossedTransmissionBounds += HandleBoundsCrossedEvent;
        }
        else
        {
            Debug.LogWarning("RadioStateController needs a BoundsChecker in the scene but none was found");
        }

        jukebox = GetComponent<Jukebox>();
        if (jukebox != null)
        {
            inBounds.notifyListenersEnter += jukebox.HandleInBoundsEnter;
            jukebox.onRadioStatusUpdate += HandleRadioStatusUpdate;
        }
        else
        {
            Debug.LogWarning("Expected to be attached to object of type Jukebox but no Jukebox component was found");
        }

        if (GameStateController.StateExists)
        {
            GameStateController.playerDead.notifyListenersEnter += HandleDeath;
        }
    }

    private void Update()
    {
        if (!initialEnter)
        {
            initialEnter = true;
            state.Enter();
        }

        // Handle toggle radio state trigger
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            HandleTrigger(RadioState.StateTrigger.ToggleRadio);
        }
    }
    #endregion


    public void HandleTrigger(RadioState.StateTrigger trigger)
    {
        //Debug.Log("Received Trigger: " + trigger);
        State newState = state.HandleTrigger(this, trigger);
        if (newState != null)
        {
            //Debug.Log("Entering State: " + newState);
            state = newState;
            newState.Enter();
        }
    }

    private void HandleBoundsCrossedEvent(bool isWithinBounds)
    {
        if (isWithinBounds)
        {
            HandleTrigger(RadioState.StateTrigger.InBounds);
        }
        else
        {
            HandleTrigger(RadioState.StateTrigger.OutOfBounds);
        }
    }

    private void HandleRadioStatusUpdate(bool radioIsPlaying)
    {
        if (radioIsPlaying)
        {
            HandleTrigger(RadioState.StateTrigger.RadioIsPlaying);
        }
        else
        {
            HandleTrigger(RadioState.StateTrigger.RadioIsNotPlaying);
        }
    }

    private void HandleDeath()
    {
        if (state != radioOff)
        {
            HandleTrigger(RadioState.StateTrigger.ToggleRadio);
        }
    }
}
