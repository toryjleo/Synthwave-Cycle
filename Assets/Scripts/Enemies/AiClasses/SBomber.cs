using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemies.AiClasses
{
    internal class SBomber : VehicleAI
    {
        public override Enemy GetEnemyType()
        {
            return Enemy.SBomber;
        }
    }
}
