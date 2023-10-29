using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CinematicWaveType
{
  undefined, ReturnToMenu, PlayAudioLog  
};

/// <summary>
/// A wave that has additional controls
/// </summary>
[CreateAssetMenu(menuName = "Wave/Wave", fileName = "New Cinematic Wave")]
public class CinematicWave : Wave
{
    [SerializeField]
    public CinematicWaveType waveType;

    internal override List<Enemy> GetWaveInfo()
    {
        switch (waveType)
        {
            case CinematicWaveType.ReturnToMenu:
                //TODO add return to main menu code here
                break;
        }
        return new List<Enemy>();
    }
}