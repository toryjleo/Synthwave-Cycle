using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFieldManager : MonoBehaviour
{
    
    [SerializeField] private Health health; // (Player health)
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    [SerializeField] private float effectTime = 2.0f; // Time for the effect to complete in seconds

    private float scanlinePos = 0;
    private bool playingEffect = false;

    #region MatPropNames

    const string _ScanlinePos = "ScanlinePos";

    #endregion


    private void Awake()
    {
        GetSingleInstanceOfMat();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (health != null)
        {
            health.healEvent += PlayEffect;
        }
        DisableEffect();
    }

    // Update is called once per frame
    void Update()
    {

        if (playingEffect) 
        {
            scanlinePos += (Time.deltaTime / effectTime);
            material.SetFloat(_ScanlinePos, scanlinePos);

            if (scanlinePos > 1.0f) 
            {
                DisableEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            PlayEffect();
        }
    }

    /// <summary>
    /// Gets the material and makes a copy.
    /// </summary>
    private void GetSingleInstanceOfMat()
    {
        if (meshRenderer.enabled)
        {
            Material attachedMaterialToDuplicate = meshRenderer.material;
            material = new Material(attachedMaterialToDuplicate);
            meshRenderer.sharedMaterial = material;
        }
    }


    private void PlayEffect() 
    {
        scanlinePos = 0;
        playingEffect = true;
        meshRenderer.gameObject.SetActive(true);
    }

    private void DisableEffect() 
    {
        scanlinePos = 0;
        playingEffect = false;
        meshRenderer.gameObject.SetActive(false);
    }
}
