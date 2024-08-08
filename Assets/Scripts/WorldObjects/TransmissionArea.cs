using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionArea : MonoBehaviour
{
    #region DefinedInPrefab
    // TODO: Figure out formatting to specify object needs prefab assigned
    [SerializeField] private HealthPool prefab_HealthPool;
    [SerializeField] private EditorObject.TransmissionArea transmissionArea;
    [SerializeField] private bool transmissionAreaIsViewable = false;
    #endregion

    /// <summary>
    /// Healthpool the transmission area creates
    /// </summary>
    private HealthPool healthPool = null;

    private float Width
    {
        get => transmissionArea.Radius * 2;
    }

    /// <summary>
    /// A percentage value. 1 is 100% clear and 0 is 0% clear
    /// TODO: Make a better alogrithm when ui and audio implemented for talking head
    /// </summary>
    public float TransmissionClarity(Vector3 point)
    {
        Vector3 centerToPoint = transform.position - point;
        if (centerToPoint.sqrMagnitude < (transmissionArea.Radius * transmissionArea.Radius)) 
        {
            return 1;
        }
        else
        {
            return 0;
        }
    
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = new Vector3(transmissionArea.Radius, 0, 0);

        // Adjusts visual and capsule collider
        transform.localScale = new Vector3(Width, transform.localScale.y, Width);
        healthPool = Instantiate(prefab_HealthPool, startPos, Quaternion.identity);
        healthPool.onDespawnConditionMet += MoveHealthPool;


        healthPool.Init(0, 5, .2f);
        healthPool.transform.RotateAround(transform.position, Vector3.up, transmissionArea.StartAngleDegrees);

#if UNITY_EDITOR
    GetComponent<MeshRenderer>().enabled = transmissionAreaIsViewable;
#else
    GetComponent<MeshRenderer>().enabled = false;
#endif
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
        healthPool.transform.RotateAround(transform.position, Vector3.up, transmissionArea.DeltaAngleDegrees);

    }
}
