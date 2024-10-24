using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;

namespace EditorObject
{
    [CreateAssetMenu(menuName = "Enemy/Test AI", fileName = "New Test AI")]
    public class TestAi : ScriptableObject, IPoolableInstantiateData
    {
        private float health = 10;

        public float Health { get => health; }
    }
}