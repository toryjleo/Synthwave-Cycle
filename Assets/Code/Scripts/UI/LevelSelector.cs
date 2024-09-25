using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorObject;

/// <summary>
/// Persistent object that holds the level for the level manager to read in
/// </summary>
public class LevelSelector : MonoBehaviour
{
    private GameLevel selectedLevel;

    public GameLevel SelectedLevel { get { return selectedLevel; } }

    /// <summary>
    /// Start deletes this object if duplicate (returned to main menu again)
    /// </summary>
    void Start()
    {
        LevelSelector[] objs = FindObjectsOfType<LevelSelector>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Sets selected level to be found by the level manager in the game scene
    /// </summary>
    /// <param name="level"></param>
    public void SetSelectedLevel(GameLevel level)
    {
        selectedLevel = level;
        //Debug.Log("Level set to: " + selectedLevel.LevelName);
    }
}
