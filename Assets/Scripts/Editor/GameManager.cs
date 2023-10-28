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
        if (playerhealth == null)
        {
            playerhealth = GameObject.FindObjectOfType<BikeScript>().GetComponentInChildren<Health>();
            if (playerhealth == null) 
            {
                Debug.LogError("Player health not found in level!");
            }
        }
        if (jukebox == null)
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
        HandleForceSoundtrack();
    }
    private void HandleGodMode()
    {
        godModeEnabled = EditorGUILayout.Toggle("God Mode Enabled", godModeEnabled);
        if (godModeEnabled != playerhealth.isInvulnurable)
        {
            playerhealth.isInvulnurable = godModeEnabled;
        }
    }
    private void HandleForceSoundtrack()
    {
        forceSoundtrack = EditorGUILayout.Toggle("Force Soundtrack?", forceSoundtrack);
        if (forceSoundtrack)
        {
            soundtrackIndex = EditorGUILayout.IntField(soundtrackIndex);

            if (jukebox.sequence != jukebox.soundTracks[soundtrackIndex])
            {
                jukebox.sequence = jukebox.soundTracks[soundtrackIndex];
                jukebox.ResetGameObject();
            }
        }
    }
}
