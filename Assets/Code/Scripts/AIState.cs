using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIState 
{
    public enum StateTrigger 
    {
        Spawning,
        AiKilled,
        ArrivedAtLocation,
        HasTarget,
        InRange,
        OutOfRange,
        CountownToAttackComplete,
        FollowAgain,
        Despawned,
        PlayerDead,
    }
}
