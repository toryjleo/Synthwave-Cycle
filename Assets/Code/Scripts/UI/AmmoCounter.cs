using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Slider ammoCounter;
    [SerializeField] private Slider overheatCounter;

    private Gun playerGun;

    // Start is called before the first frame update
    void Start()
    {
        playerGun = FindObjectOfType<Gun>();
        if (playerGun == null)
        {
            Debug.LogWarning("No gun found for ammo counter!");
            // Disable this object
            this.gameObject.SetActive(false);
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
