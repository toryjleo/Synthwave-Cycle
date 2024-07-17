using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URP_Demo_Rot_Script : MonoBehaviour
{


    public static float RotationSpeed = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
    }
}
