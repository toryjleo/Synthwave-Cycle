using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "Enemy/AI Stats", fileName = "New AI Stats")]
    public class AiStats : ScriptableObject, IPoolableInstantiateData
    {
        // TODO: Enemy type

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
        [SerializeField] private float maxForce = 1;

        /// <summary>
        /// The amount of danger level score an enemy gives upon death
        /// </summary>
        [SerializeField] private float dlScore = 10;

        /// <summary>
        /// The distance from the target an enemy has to be to start counting down to attack
        /// </summary>
        [SerializeField] private float attackRange = 15;

        /// <summary>
        /// The distance away from the player that an enemy will follow when Chasing
        /// </summary>
        [SerializeField] private float followRange = 10;

        /// <summary>
        /// The amount of time (seconds) it takes while an enemy is in range to attack
        /// </summary>
        [SerializeField] private float timeToAttack = 2.0f;

        /// <summary>
        /// Determines whether the enemy can rotate and aim separately from moving
        /// </summary>
        [SerializeField] private bool canAim = false;

        /// <summary>
        /// The type of enemy this is, used in spawn pooling
        /// </summary>
        [SerializeField] private Enemy enemyType = Enemy.Rifleman;

        public float Health { get => health; }
        public float MovementGroup { get => movementGroup; }
        public float GearModifier { get => gearModifier; }
        public float MaxForce { get => maxForce; }
        public float DlScore { get => dlScore; }
        public float AttackRange { get => attackRange; }
        public float FollowRange { get => followRange; }
        public float TimeToAttack { get => timeToAttack; }
        public bool CanAim { get => canAim; }
        public Enemy EnemyType { get => enemyType; }
    }
}