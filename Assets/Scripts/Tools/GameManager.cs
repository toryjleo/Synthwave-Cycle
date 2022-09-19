using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : EditorWindow
{
    BoxCollider bikeHealth;
    bool godModeEnabled = false;

    [MenuItem("Window/GameManager")]
    public static void Init() 
    {
        EditorWindow.GetWindow<GameManager>("Game Manager Window");
    }

    private void Update()
    {
        // Make sure to keep a reference to player health
        if (bikeHealth == null)
        {
            bikeHealth = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<BoxCollider>();
            if (bikeHealth == null) 
            {
                Debug.LogError("Player health not found in level!");
            }
        }
    }

    private void OnEnable()
    {

    }


    private void OnGUI()
    {
        godModeEnabled = EditorGUILayout.Toggle("God Mode Enabled", godModeEnabled);
        if (godModeEnabled != bikeHealth.enabled) 
        {
            bikeHealth.enabled = !godModeEnabled;
        }


    }
}
