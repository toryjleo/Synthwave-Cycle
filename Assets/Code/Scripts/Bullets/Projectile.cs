using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{

    /// <summary>Class <c>Bullet</c> A Unity Component which moves a gameobject foreward.</summary>
    public abstract class Projectile : Generic.Poolable
    {
        public event BulletHitHandler notifyListenersHit;

        protected Vector3 shootDir;
        protected Vector3 initialVelocity;

        protected EditorObject.GunStats gunStats = null;

        internal List<GameObject> alreadyHit = new List<GameObject>();

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            Move();
        }

        public override void Init(IPoolableInstantiateData stats)
        {
            EditorObject.GunStats gunStats = stats as EditorObject.GunStats;
            if (gunStats != null)
            {
                this.gunStats = gunStats;
            }
            else 
            {
                Debug.LogError("Expects to be handed a GunStats reference.");
            }
        }

        /// <summary>Updates the object's location this frame.</summary>
        protected virtual void Move()
        {
            Vector3 distanceThisFrame = ((shootDir * gunStats.MuzzleVelocity) + initialVelocity) * Time.deltaTime;
            transform.position = transform.position + distanceThisFrame;
        }

        /// <summary>Initializes this bullet to start moving.</summary>
        /// <param name="curPosition">Location to start being shot from.</param>
        /// <param name="direction">Direction in which bullet will move.</param>
        /// <param name="initialVelocity">Velocity of the object shooting.</param>
        public virtual void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity)
        {
            this.transform.localScale = gunStats.ProjectileScale;
            transform.position = curPosition;
            direction.y = 0; // Do not travel vertically
            shootDir = direction.normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            this.initialVelocity = initialVelocity;
            this.timeInWorld = 0;
        }

        /// <summary>
        /// Resets bullet properties. This is used when a bullet is spawned from a pool to ensure it gets a fresh start
        /// </summary>
        public override void Reset()
        {
            initialVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        protected bool CanHitObject(Collider other) 
        {
            return (other.gameObject.tag == "Enemy" && gunStats.IsPlayerGun) ||
                   (other.gameObject.tag == "Player" && !gunStats.IsPlayerGun);
        }

        protected void NotifyListenersHit() 
        {
            notifyListenersHit?.Invoke(transform.position);
        }
    }
}