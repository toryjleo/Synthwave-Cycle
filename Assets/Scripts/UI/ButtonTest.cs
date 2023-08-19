using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    public Button TestButton1 = null;

    // Start is called before the first frame update
    void Start()
    {
        TestButton1.onClick.AddListener(fuck);
        Debug.Log("Fuck");
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.deltaTime;
    }
    private void fuck()
    {
        Debug.Log("FUCK");
    }
}
