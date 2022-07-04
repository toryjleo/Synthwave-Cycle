using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that manages the materials and renderers associated with a material
/// </summary>
public class BloodSplatter : MonoBehaviour
{
    // Set in editor
    public MeshRenderer bloodImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HideBlood() 
    {
        bloodImage.enabled = false;
    }

    /// <summary>
    /// Occludes the image
    /// </summary>
    public void DisplayBlood() 
    {
        bloodImage.enabled = true;
    }
}
