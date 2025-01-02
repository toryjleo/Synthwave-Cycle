using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "AI Stats", fileName = "New AI Stats")]
    public class AiStats : ScriptableObject, IPoolableInstantiateData
    {
        /// <summary>
        /// The health points the enemy starts with
        /// </summary>
        [SerializeField] private float health = 10;

        /// <summary>
        /// The movement group an enemy belongs to. Value of 1, 2, or 3 (relative to player speed gears)
        /// </summary>
        [SerializeField] private float movementGroup = 1;

        /// <summary>
        /// Multiplier of the movementGroup top speed
        /// </summary>
        [SerializeField] private float gearModifier = 0.8f;

        /// <summary>
        /// The maximum force determines how quickly an enemy groups and separates
        /// </summary>

        /// <summary>
        /// The maximum chase force determines how much priority chasing has
        /// </summary>
        [SerializeField] private float maxChaseForce = 1;

        /// <summary>
        /// The distance away from the player that an enemy will follow when Chasing, measured in meters
        /// </summary>
        [SerializeField] private float chaseRange = 10;

        /// <summary>
        /// The maximum grouping force determines how much priority grouping has
        /// </summary>
        [SerializeField] private float maxGroupingForce = 1;

        /// <summary>
        /// The distance between other units of the same type to stay between, measured in meters
        /// </summary>
        [SerializeField] private float groupingRange = 10;

        /// <summary>
        /// The maximum separate force determines how much priority separating has
        /// </summary>
        [SerializeField] private float maxSeparateForce = 1;

        /// <summary>
        /// The distance away from other units of the same type to keep from, measured in meters
        /// </summary>
        [SerializeField] private float separateRange = 10;

        /// <summary>
        /// This float will determine how fast an enemy slows down when they've arrived near the player
        /// </summary>
        [SerializeField] private float arrivalWeight = 0.0f;

        /// <summary>
        /// The amount of danger level score an enemy gives upon death
        /// </summary>
        [SerializeField] private int dlScore = 10;

        /// <summary>
        /// The distance from the target an enemy has to be to start counting down to attack, measured in meters
        /// </summary>
        [SerializeField] private float attackRange = 15;

        /// <summary>
        /// The amount of time (seconds) it takes while an enemy is in range to attack
        /// </summary>
        [SerializeField] private float timeToAttack = 2.0f;

        [SerializeField] private float timeToDie = 3.0f;

        /// <summary>
        /// Determines whether the enemy can rotate and aim separately from moving
        /// </summary>
        [SerializeField] private bool canAim = false;

        /// <summary>
        /// The type of enemy this is, used in spawn pooling
        /// </summary>
        [SerializeField] private Enemy enemyType = Enemy.Rifleman;

        [SerializeField] private GunStats gunStats = null;

        public float Health { get => health; }
        public float MovementGroup { get => movementGroup; }
        public float GearModifier { get => gearModifier; }
        public float MaxChaseForce { get => maxChaseForce; }
        public float ChaseRange { get => chaseRange; }
        public float MaxGroupingForce { get => maxGroupingForce; }
        public float GroupingRange { get => groupingRange; }
        public float MaxSeparateForce { get => maxSeparateForce; }
        public float SeparateRange { get => separateRange; }
        public float ArrivalWeight { get => arrivalWeight; }
        public int DlScore { get => dlScore; }
        public float AttackRange { get => attackRange; }
        public float TimeToAttack { get => timeToAttack; }
        public float TimeToDie { get => timeToDie; }
        public bool CanAim { get => canAim; }
        public Enemy EnemyType { get => enemyType; }
        public GunStats GunStats
        {
            get
            {
                if (gunStats == null)
                {
                    Debug.LogError("No gunStats on this " + EnemyType.ToString());
                    return null;
                }
                else
                {
                    return gunStats;
                }
            }
        }
    }
}