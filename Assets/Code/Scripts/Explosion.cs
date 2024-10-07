using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] bool hasDelay = false;
    [SerializeField] private float delay = 1.0f;

    [SerializeField] private float radius = 5.0f;
    [SerializeField] private float force = 20000;
    [SerializeField] private bool explodeOnCollision = false;
    [SerializeField] private float damage = 25;

    private float delayTimer = 0.0f;

    #region Graphic
    private MeshRenderer meshRenderer = null;
    private float timeToShowGraphic = 1f;
    private float meshRendererTimer = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        delayTimer = 0.0f;

        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            Debug.LogError("Explosion needs attached spherical MeshRenderer for graphic");
        }
        else
        {
            meshRenderer.enabled = false;
        }
        meshRendererTimer = 0;

        transform.localScale = new Vector3(radius, radius, radius);
    }



    // Update is called once per frame
    public void Update()
    {
        DelayedExplosion(Time.deltaTime);

        UpdateEffects(Time.deltaTime);

#if DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoExplosion();
        }
#endif
    }

    public void DoExplosion()
    {
        Init();
        HandleEffects();
        HandleDestruction();
    }

    private void HandleEffects()
    {
        meshRendererTimer = 0;
        meshRenderer.enabled = true;
    }

    private void UpdateEffects(float deltaTime) 
    {
        if (meshRenderer.enabled)
        {
            meshRendererTimer += deltaTime;
            if (meshRendererTimer > timeToShowGraphic) 
            {
                meshRenderer.enabled = false;
                Destroy(this);
            }
        }
    }

    private void HandleDestruction() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            // Do not apply explosion force to projectiles
            if (collider.GetComponent<Projectile>() == null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                Health health = collider.GetComponent<Health>();

                if (rb)
                {
                    rb.AddExplosionForce(force, transform.position, radius);
                }
                if (health)
                {
                    // TODO: decide who is hurt
                    health.TakeDamage(damage);
                }
            }
        }
    }

    private void DelayedExplosion(float deltaTime) 
    {
        if (hasDelay)
        {
            delayTimer += deltaTime;

            if (delayTimer >= delay && !explodeOnCollision)
            {
                DoExplosion();
            }
        }
    }
}
