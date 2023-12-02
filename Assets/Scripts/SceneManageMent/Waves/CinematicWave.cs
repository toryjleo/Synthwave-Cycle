using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CinematicWaveType
{
  undefined, ReturnToMenu, PlayAudioLog  
};

/// <summary>
/// A wave that has additional controls
/// </summary>
[CreateAssetMenu(menuName = "Wave/CinematicWave", fileName = "New Cinematic Wave")]
public class CinematicWave : Wave
{
    [SerializeField]
    public CinematicWaveType waveType;

    internal override List<Enemy> GetWaveInfo()
    {
        switch (waveType)
        {
            case CinematicWaveType.ReturnToMenu:
                SceneManager.LoadScene("MainMenu");
                break;
        }
        return new List<Enemy>();
    }
}