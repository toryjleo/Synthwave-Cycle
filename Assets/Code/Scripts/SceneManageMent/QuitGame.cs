using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>QuitGame</c> Component which will quit the game when "escape" is pressed if in a given scene with
/// this gameObject.</summary>
public class QuitGame : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


}
