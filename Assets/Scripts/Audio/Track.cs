using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A track is a container of all associated music clips.
/// </summary>
[CreateAssetMenu(menuName = "Music/AudioTrack", fileName = "New Track")]
public class Track : ScriptableObject
{
    // The audio clip that introduces this track
    public AudioClip            intro = null;
    // A list of variations. Each subsequent variation is expected to have an increasing danger level.
    public List<TrackVariation> variations;
}
