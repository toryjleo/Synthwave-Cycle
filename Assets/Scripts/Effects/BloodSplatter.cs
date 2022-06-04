using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void Init() 
    {
        bloodImage.enabled = false;
    }

    public void DisplayBlood() 
    {
        bloodImage.enabled = true;
    }
}
