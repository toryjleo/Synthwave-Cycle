using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float delay = 1.0f;
    [SerializeField] private float radius = 5.0f;
    [SerializeField] private float force = 20000;
    [SerializeField] private bool explodeOnCollision = false;

    private float delayTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        delayTimer = 0.0f;
    }



    // Update is called once per frame
    public  void Update()
    {

        delayTimer += Time.deltaTime;

        if (delayTimer >= delay && !explodeOnCollision) 
        {
            DoExplosion();
            Destroy(gameObject);
        }
    }

    private void DoExplosion() 
    {
        HandleEffects();
        HandleDestruction();
    }

    private void HandleEffects()
    {
        // TODO: flash our child sphere for time
    }

    private void HandleDestruction() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            if (rb) 
            {
                // TODO: make sure this force is 1D
                rb.AddExplosionForce(force, transform.position, radius);
                // TODO: deal damage as well
            }
        }
    }
}
