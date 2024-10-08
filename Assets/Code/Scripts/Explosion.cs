using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Generic.Poolable
{
    #region Countdown Explosion
    private float countDownTimer = 0.0f;

    [SerializeField] private MeshRenderer countDownMesh = null;
    #endregion

    private GunStats gunStats;

    #region Graphic
    [SerializeField] private MeshRenderer meshRenderer = null;
    private float timeToShowGraphic = 1f;
    private float meshRendererTimer = 0;
    #endregion


    public override void Init(IPoolableInstantiateData data)
    {
        GunStats gunStats = data as GunStats;
        if (!gunStats)
        {
            Debug.LogError("Explosion needs to get initialized by a GunStats object.");
        }
        this.gunStats = gunStats;

        Reset();
    }

    public override void Reset()
    {
        countDownTimer = 0.0f;

        meshRendererTimer = 0;

        transform.localScale = new Vector3(gunStats.Radius * 2, gunStats.Radius * 2, gunStats.Radius * 2);
        transform.rotation = Quaternion.identity;

        meshRenderer.enabled = false;
        countDownMesh.enabled = gunStats.IsCountDownExplosion;
    }



    // Update is called once per frame
    public void Update()
    {
        UpdateCountdownExplosion(Time.deltaTime);

        UpdateEffects(Time.deltaTime);
    }

    public void TriggerExplosiveAbility()
    {
        if (!gunStats.IsCountDownExplosion)
        {
            DoExplosion();
        }
        // Else case is handled in update with the countdown explosion
    }

    private void DoExplosion()
    {
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
                OnDespawn();
            }
        }
    }


    private void HandleDestruction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, gunStats.Radius);

        foreach (Collider collider in colliders)
        {
            // Do not apply explosion force to projectiles
            if (collider.GetComponent<Projectile>() == null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                Health health = collider.GetComponent<Health>();

                if (rb)
                {
                    rb.AddExplosionForce(gunStats.Force, transform.position, gunStats.Radius);
                }
                if (health)
                {
                    if ((health.gameObject.tag == "Enemy") ||
                        (health.gameObject.tag == "Player" && !gunStats.IsPlayerGun))
                    {
                        health.TakeDamage(gunStats.ExplosionDamage);
                    }
                }
            }
        }
    }


    private void UpdateCountdownExplosion(float deltaTime)
    {
        if (countDownMesh.enabled)
        {
            countDownTimer += deltaTime;

            if (countDownTimer >= gunStats.SecondsBeforeExplode)
            {
                countDownMesh.enabled = false;
                DoExplosion();
            }
        }
    }
}
