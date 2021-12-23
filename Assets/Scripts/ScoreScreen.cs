using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{

    public TextMeshProUGUI scoreText;

    void Awake()
    {
        scoreText.text = PlayerDataObject.lastGameScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
