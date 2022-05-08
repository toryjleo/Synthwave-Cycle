using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A version of a given track
/// </summary>
[CreateAssetMenu(menuName = "Music/AudioTrackVariation", fileName = "New Track Variation")]
public class TrackVariation : ScriptableObject
{
    // The given clip associated with this variation
    public AudioClip variation;
    // The heat level at which this track should start playing
    public int dangerLevelStart = 0;
}
