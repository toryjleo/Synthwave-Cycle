using UnityEngine;

namespace Assets.Scripts.Enemies.AiClasses
{
    internal class RammingCar : VehicleAI
    {
        public override Enemy GetEnemyType()
        {
            return Enemy.RamCar;
        }
        public override void Attack()
        {
            //Move to the firing target (player)
            this.SetMovementTarget(target);
        }
        public override void NewLife()
        {
            TIME_BY_TARGET_TO_ATTACK = Random.Range(3, 11);
            base.NewLife();
        }
    }
}
