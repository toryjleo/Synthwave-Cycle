using RadioState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// A class to manage and track radio state and notifies front end
/// Requires:
/// - BoundsChecker
/// - Jukebox
/// </summary>
public class RadioStateController : MonoBehaviour
{
    #region Outside References
    private BoundsChecker boundsChecker;
    private Jukebox jukebox;
    #endregion

    private static RadioState.State state;

    #region Managed States
    public RadioState.RadioOff radioOff;
    public RadioState.RadioOn radioOn;
    public RadioState.InBounds inBounds;
    public RadioState.OutOfBounds outOfBounds;
    public RadioState.RadioPlaying radioPlaying;
    public RadioState.RadioNotPlaying radioNotPlaying;
    #endregion

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
            radioOn.notifyListenersEnter += HandleEnterRadioOn;
        }
        else
        {
            Debug.LogWarning("RadioStateController needs a BoundsChecker in the scene but none was found");
        }

        jukebox = GetComponent<Jukebox>();
        if (jukebox != null)
        {
            inBounds.notifyListenersEnter += HandleEnterInBounds;
        }
        else
        {
            Debug.LogWarning("Expected to be attached to object of type Jukebox but no Jukebox component was found");
        }

        if (GameStateController.StateExists)
        {
            GameStateController.playerDead.notifyListenersEnter += HandleEnterDeath;
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

    /// <summary>
    /// Passes triggers to states to trigger state transisions
    /// </summary>
    /// <param name="trigger">Trigger for current state to handle</param>
    public void HandleTrigger(RadioState.StateTrigger trigger)
    {
        State newState = state.HandleTrigger(this, trigger);
        if (newState != null)
        {
            state = newState;
            newState.Enter();
        }
    }

    /// <summary>
    /// Sends either an InBounds or OutOfBounds trigger
    /// </summary>
    private void HandleEnterRadioOn() 
    {
        if (boundsChecker.IsInBounds)
        {
            HandleTrigger(RadioState.StateTrigger.InBounds);
        }
        else
        {
            HandleTrigger(RadioState.StateTrigger.OutOfBounds);
        }
    }

    /// <summary>
    /// Sends either an RadioIsPlaying or RadioIsNotPlaying trigger
    /// </summary>
    private void HandleEnterInBounds() 
    {
        if (jukebox.RadioClipIsPlaying)
        {
            HandleTrigger(RadioState.StateTrigger.RadioIsPlaying);
        }
        else
        {
            HandleTrigger(RadioState.StateTrigger.RadioIsNotPlaying);
        }
    }

    /// <summary>
    /// Makes sure the radio is off on death
    /// </summary>
    private void HandleEnterDeath()
    {
        if (state != radioOff)
        {
            HandleTrigger(RadioState.StateTrigger.ToggleRadio);
        }
    }
}
