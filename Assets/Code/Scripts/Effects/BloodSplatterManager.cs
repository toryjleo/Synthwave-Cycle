using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages blood splatters for enemy AI's. Enemy AI's WILL NOT reference this script.
/// </summary>
public class BloodSplatterManager : MonoBehaviour
{
    // Need to manually assign AI
    [SerializeField] private Ai ai;
    [SerializeField] private BloodSplatter bloodSplatter;
    [SerializeField] private Texture[] splatterTextures;


    // Start is called before the first frame update
    void Start()
    {
        if (ai != null)
        {
            // Hook up events
            ai.DeadVisualsEvent += ShowBlood;
            // Make sure that the blood splatters get turned off when the enemy despawns
            ai.RespawnEvent += Init;
        }

        Init();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Init();
            ShowBlood();
        }*/
    }

    /// <summary>
    /// Initialization function to be used by Despawn calls.
    /// </summary>
    /// <param name="entity">Entity getting despawned.</param>
    public void Init(SelfDespawn entity)
    {
        Init();
    }

    public virtual void Init()
    {
        bloodSplatter.HideBlood();
    }

    /// <summary>
    /// Returns a random texture.
    /// </summary>
    /// <returns>A random texture.</returns>
    private Texture GetRandomTexture()
    {
        return splatterTextures[Random.Range(0, splatterTextures.Length - 1)];
    }

    /// <summary>
    /// Displays blood upon enemy death.
    /// </summary>
    private void ShowBlood()
    {
        Texture alphaTex = GetRandomTexture();
        Texture albedoTex = GetRandomTexture();
        bloodSplatter.SetAlphaMask(alphaTex);
        bloodSplatter.SetMainTexture(albedoTex);
        bloodSplatter.DisplayBlood();
    }
}
