using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionArea : MonoBehaviour
{
    [SerializeField] private HealthPool prefab_HealthPool;


    private float radius = 20;
    private Vector3 startPos = new Vector3 (20, 0, 0);

    [SerializeField] private HealthPool healthPool = null;

    private float Width 
    {
        get => radius * 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Adjusts visual and capsule collider
        transform.localScale = new Vector3(Width, transform.localScale.y, Width);
        healthPool = Instantiate(prefab_HealthPool, startPos, Quaternion.identity);
        healthPool.Init(0, 1, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
