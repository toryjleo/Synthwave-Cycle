using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>Class <c>ScoreScreen</c> A class to manage the UI in the GameOver screen.</summary>
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
