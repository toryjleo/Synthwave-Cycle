using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>Class <c>ScoreTracker</c> Component which manages the UI in the upper left of the game screen.</summary>
/// Expects there to be an object with BikeScript in the scene.
public class ScoreTracker : MonoBehaviour
{
    private const float MAX_TIME = 90;
    private float currentTime;
    private int currentScore;
    private float _currentEnergy;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentHPText;

    public BikeScript bike;

    // Basically player HP but ~flavored~
    public float Energy
    {
        set {
                _currentEnergy = value;
            }
    }


    private void Awake()
    {
        Init();
        FindBike();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIEnergy();
        UpdateText();
        if (currentTime <= 0)
        {
            // Win condition
            EndGame();
        }
        else if (_currentEnergy <=0) 
        {
            // Lose Condition
            EndGame();
        }
    }

    public void Init() 
    {
        currentTime = MAX_TIME;
        currentScore = 0;
    }

    private void FindBike() 
    {
        BikeScript[] bikeScripts = Object.FindObjectsOfType<BikeScript>();
        if (bikeScripts.Length <= 0)
        {
            Debug.LogError("WorldGenerator did not find any BikeScripts in scene");
        }
        else
        {
            bike = bikeScripts[0];
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

    /// <summary>Updates the UI displayed on screen.</summary>
    private void UpdateText() 
    {
        currentTime -= Time.deltaTime;

        timeLeftText.text = currentTime.ToString("0.00"); // Formats to 2 decimal points
        currentScoreText.text = currentScore.ToString();
        currentHPText.text = "Energy: " + _currentEnergy.ToString("0.00");
    }

    /// <summary>Adds points to the score for this game session.</summary>
    /// <param name="points">The number of points to add to the score.</param>
    public void AddToScore(int points) 
    {
        currentScore += points;
    }

    /// <summary>Updates this class's Energy to the bike's energy.</summary>
    private void UpdateUIEnergy()
    {
        Energy = bike.Energy;
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
