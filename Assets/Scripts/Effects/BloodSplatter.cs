using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that manages the materials and renderers associated with a material
/// </summary>
public class BloodSplatter : MonoBehaviour
{
    // Set in editor
    private MeshRenderer meshRenderer;
    private Material material;

    #region MatPropNames
    const string _AlphaMask = "_AlphaMask";
    const string _MainTex = "_MainTex";
    const string _AlphaMaskRotation = "_AlphaMaskRotation";
    const string _MainTexRotation = "_MainTexRotation";
    const string _AlphaMaskScale = "_AlphaMaskScale";
    const string _MainTexScale = "_MainTexScale";


    #endregion

    private float alphaMaskMaxScale;
    private float alphaMaskCurScale;
    private float timeSinceBloodSpilled;


    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        GetSingleInstanceOfMat();
    }

    // Update is called once per frame
    void Update()
    {
        GrowBloodPool();

    }


    public void Init() 
    {
        meshRenderer.enabled = false;
    }

    /// <summary>
    /// Shows the image
    /// </summary>
    public void DisplayBlood() 
    {
        meshRenderer.enabled = true;

        // Randomize rotations and splatter scale

        alphaMaskMaxScale = Random.Range(.9f, 1.3f);
        alphaMaskCurScale = Random.Range(.5f, .8f);
        material.SetFloat(_AlphaMaskRotation, Random.Range(0, 2 * Mathf.PI));
        material.SetFloat(_MainTexRotation, Random.Range(0, 2 * Mathf.PI));
        material.SetFloat(_AlphaMaskScale, alphaMaskCurScale);
        material.SetFloat(_MainTexScale, Random.Range(.9f, 1.3f));

        timeSinceBloodSpilled = 0;

    }

    /// <summary>
    /// Slowly grow the pool of blood once it has been placed
    /// </summary>
    private void GrowBloodPool() 
    {
        if (meshRenderer.enabled)
        {
            timeSinceBloodSpilled += (Time.deltaTime / 100);
            float poolIncrease = (Mathf.Log(timeSinceBloodSpilled * .5f + 1f)) * 7;
            material.SetFloat(_AlphaMaskScale, Mathf.Clamp(alphaMaskCurScale + poolIncrease, .3f, alphaMaskMaxScale));
            Debug.Log(timeSinceBloodSpilled + ", " + alphaMaskCurScale + poolIncrease);
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

}
