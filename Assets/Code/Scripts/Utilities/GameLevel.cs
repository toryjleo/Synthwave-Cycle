using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds information for a specific level in the game, including the ground material and wave sequence
public class GameLevel : MonoBehaviour
{
    [SerializeField]
    public EditorObject.WaveSequence WaveSequence;
    [SerializeField]
    public Material GroundMat;
}
