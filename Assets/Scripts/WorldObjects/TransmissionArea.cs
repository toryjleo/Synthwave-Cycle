using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionArea : MonoBehaviour
{
    // TODO: Figure out formatting to specify object needs prefab assigned
    [SerializeField] private HealthPool prefab_HealthPool;


    private float radius = 20;
    // In degrees
    private float spawnAngle = 0;

    [SerializeField] private HealthPool healthPool = null;

    private float Width 
    {
        get => radius * 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = new Vector3(radius, 0, 0);

        // Adjusts visual and capsule collider
        transform.localScale = new Vector3(Width, transform.localScale.y, Width);
        healthPool = Instantiate(prefab_HealthPool, startPos, Quaternion.identity);
        healthPool.onDespawnConditionMet += MoveHealthPool;


        healthPool.Init(0, 5, .2f);
        healthPool.transform.RotateAround(transform.position, Vector3.up, spawnAngle);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            MoveHealthPool();
        }
#endif

    }

    private void MoveHealthPool() 
    {
        float deltaAngle = 60;
        healthPool.transform.RotateAround(transform.position, Vector3.up, deltaAngle);

    }
}
