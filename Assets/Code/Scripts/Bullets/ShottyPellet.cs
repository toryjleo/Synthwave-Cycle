using Assets.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShottyPellet : PlayerBullet
{
    private float growthSpeed = 4f;
    private float maxSize = 10f;
    public override void Initialize()
    {
        muzzleVelocity = 90;
        mass = .2f;
        damageDealt = 240;
        boost = 2f;
    }

    public override void ResetBullet()
    {
        this.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
    }

    public override void Update()
    {
        float resize = this.gameObject.transform.localScale.x + (growthSpeed * Time.deltaTime);
        if (resize <= maxSize)
        {
            this.gameObject.transform.localScale = new Vector3(resize, resize, resize);
        }
        base.Update();
    }
}
