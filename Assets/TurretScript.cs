using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : Gun
{


    public Vector3 mouse;
    public Vector3 mouseWorld;
    public Vector3 forward;
    int steerRate = 2; //the rate at which turret turns. 
    public override void Init()
    {
        
    }

    public override void Shoot(Vector3 initialVelocity)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         mouse = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
         var angle = Mathf.Atan2(mouse.x, mouse.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        //transform.Rotate(Vector3.up, steerRate);

    }
}
