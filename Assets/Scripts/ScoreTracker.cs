using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>Class <c>ScoreTracker</c> Component which manages the UI in the upper left of the game screen.</summary>
public class ScoreTracker : MonoBehaviour
{
    private const float MAX_TIME = 90;
    private float currentTime;
    private int currentScore;
    private float _currentEnergy;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentHPText;

    // Basically player HP but ~flavored~
    public float Energy
    {
        set {
                _currentEnergy = value;
            }
    }


    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    public void Init() 
    {
        currentTime = MAX_TIME;
        currentScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;

        timeLeftText.text = currentTime.ToString("0.00"); // Formats to 2 decimal points
        currentScoreText.text = currentScore.ToString();
        currentHPText.text = "Energy: " + _currentEnergy.ToString("0.00");

        if (currentTime <= 0) 
        {
            EndGame();
        }
    }

    void OnDisable() 
    {
        // Update Score for next scene
        PlayerDataObject.lastGameScore = currentScore;

        // Update highschore if a new one is achieved
        if (currentScore > PlayerDataObject.highScore) 
        {
            PlayerPrefs.SetInt("highScore", currentScore);
            PlayerDataObject.highScore = currentScore;
        }
    }

    /// <summary>Adds points to the score for this game session.</summary>
    /// <param name="points">The number of points to add to the score.</param>
    public void AddToScore(int points) 
    {
        currentScore += points;
    }

    /// <summary>Loads the gameover screen.</summary>
    private void EndGame() 
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    /// <summary>Loads the gameover screen asycronously.</summary>
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EndGameScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
