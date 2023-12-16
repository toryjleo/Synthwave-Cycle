using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : EditorWindow
{
    Health playerhealth;
    Jukebox jukebox;
    bool godModeEnabled = false;
    bool forceSoundtrack = false;
    int soundtrackIndex = 0;

    [MenuItem("Window/GameManager")]
    public static void Init() 
    {
        EditorWindow.GetWindow<GameManager>("Game Manager Window");
    }

    private void Update()
    {
        // Make sure to keep a reference to player health
        if (playerhealth == null && GameStateController.GameIsPlaying())
        {
            playerhealth = GameObject.FindObjectOfType<BikeScript>().GetComponentInChildren<Health>();
            if (playerhealth == null) 
            {
                Debug.LogError("Player health not found in level!");
            }
        }
        if (jukebox == null && GameStateController.GameIsPlaying())
        {
            jukebox = GameObject.FindObjectOfType<Jukebox>();
            if (jukebox == null) 
            {
                Debug.LogError("Jukebox not found in level!");
            }
        }
    }

    private void OnEnable()
    {

    }


    private void OnGUI()
    {
        HandleGodMode();
    }
    private void HandleGodMode()
    {
        godModeEnabled = EditorGUILayout.Toggle("God Mode Enabled", godModeEnabled);
        if (godModeEnabled != playerhealth.isInvulnurable)
        {
            playerhealth.isInvulnurable = godModeEnabled;
        }
    }
}
