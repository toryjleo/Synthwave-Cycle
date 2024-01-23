using UnityEngine;

namespace Assets.Scripts.Enemies.AiClasses
{
    internal class Trailer : VehicleAI
    {
        public override Enemy GetEnemyType()
        {
            return Enemy.TrailerCar;
        }
        public override void Attack()
        {
            if (myGun != null && myGun.CanShootAgain() && alive)
            {
                if (target.GetComponentInChildren<Rigidbody>() != null)
                {
                    Debug.Log("RB velocity: " + target.GetComponentInChildren<Rigidbody>().velocity);
                    this.myGun.PrimaryFire(rb.velocity);
                }
                else
                {
                    Debug.LogError("Target does not have RigidBody, cannot lead shot!");
                }
            }
        }
        public override void NewLife()
        {
            //KILL ON SIGHT
            TIME_BY_TARGET_TO_ATTACK = 0.1f;
            base.NewLife();
        }
    }
}
