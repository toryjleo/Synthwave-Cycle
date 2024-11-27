using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVehicleAttack : MonoBehaviour
{
    [SerializeField] public GameObject attackTarget;
    [SerializeField] public GameObject movementTarget;

    // Update is called once per frame
    void Update()
    {
        CalculateAttackMovement();
        Debug.DrawLine(transform.position, attackTarget.transform.position, Color.red);
        Debug.DrawLine(transform.position, movementTarget.transform.position, Color.blue);
    }

    private void CalculateAttackMovement()
    {
        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        movementTarget.transform.position = (20 * direction) + attackTarget.transform.position;
    }
}
