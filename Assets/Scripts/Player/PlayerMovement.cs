using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region InputManagerStrings
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    #endregion

    public Rigidbody rigidBody;

    private Vector3 start_position = new Vector3(0, 1, 0);
    private Vector3 inputDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = start_position;
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = GetInputDir();

    }
    private void FixedUpdate()
    {
        rigidBody.AddForce(inputDirection);
    }

    private Vector3 GetInputDir() 
    {
        // Note: using vertical axis to represent +x and horizontal axis to represent -z
        float horizontalAxis = Input.GetAxis(HORIZONTAL);
        float verticalAxis = Input.GetAxis(VERTICAL);

        Vector3 desiredDirection = new Vector3(verticalAxis, 0, -horizontalAxis);
        Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);
        float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);


        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        Debug.DrawLine(transform.position, transform.position + (desiredDirectionNormalized * magnitude), Color.magenta);

        return desiredDirectionNormalized * magnitude;
    }
}