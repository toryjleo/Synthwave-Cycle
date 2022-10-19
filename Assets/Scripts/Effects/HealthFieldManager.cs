using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages effect for the player where a sphere will envelop the player upon picking up health.
/// </summary>
public class HealthFieldManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Health health; // (Player health)
    [SerializeField] private MeshRenderer meshRenderer; // Mesh renderer of the field effect
    [SerializeField] private Material material; // Material for the effect to use

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


    /// <summary>
    /// Initiates the effect.
    /// </summary>
    private void PlayEffect() 
    {
        scanlinePos = 0;
        playingEffect = true;
        meshRenderer.gameObject.SetActive(true);
        audioSource.Play();
    }

    /// <summary>
    /// Stops the effect immediately.
    /// </summary>
    private void DisableEffect() 
    {
        scanlinePos = 0;
        playingEffect = false;
        meshRenderer.gameObject.SetActive(false);
    }
}
