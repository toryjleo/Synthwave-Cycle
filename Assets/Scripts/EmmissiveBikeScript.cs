using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmissiveBikeScript : MonoBehaviour
{

    [SerializeField] private Material emissiveMaterial;
    [SerializeField] private Renderer objectToChange;
    // Start is called before the first frame update

    public List<BikeLight> Lights;

    [System.Serializable]
    public class BikeLight
    {
        public GameObject Light;
        public Color color;

        public BikeLight(Color c, GameObject g)
        {
            c = color;
            g = Light;
        }

    }
    

    void Start()
    {
        emissiveMaterial = objectToChange.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
