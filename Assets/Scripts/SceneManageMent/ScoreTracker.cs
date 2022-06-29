using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Timers;

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
        GetInput();
        UpdateUIEnergy();
        UpdateText();
        if (currentTime <= 0)
        {
            // Win condition
            EndGame(true);
        }
        else if (_currentEnergy <=0)
        {
            // Lose Condition
            EndGame(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
    /// <param name="survivedEvent">True if the player beat the level.</param>
    private void EndGame(bool survivedEvent)
    {
        PlayerDataObject.survivedEvent = survivedEvent;

        Object.FindObjectOfType<DLevel>().dangerTimer.Dispose(); //Dispose of timer for spawning more enemies
        SceneManager.LoadScene("TestScene");
        //StartCoroutine(LoadYourAsyncScene());
    }
    /// <summary>This resets the game, first killing all enemies, then removing all bullets, then reseting the scoreTracker, bike health, and bike motion, then updating the scoreTrackers ui</summary>
    public void ResetGame(){
// Restart the game
            //kill all enemies
            EnemyManager enemyControlPanel=Object.FindObjectOfType<EnemyManager>();
            enemyControlPanel.killAllEnemies();
            //Remove all currently flying bullets, this may cause issues with null pointers so I need to make sure that the pools aren't emptied too quickly
            Bullet[] AllBulletsManager=Object.FindObjectsOfType<Bullet>();
            foreach(Bullet bulletToDespawn in AllBulletsManager){
                bulletToDespawn.gameObject.SetActive(false);
            }
            //reset score, set bike movement to 0, health to starting health. First the values are reset and then the UI is updated
            ScoreTracker scoreControlPanel=Object.FindObjectOfType<ScoreTracker>();
            scoreControlPanel.Init();
            bike.resetBikeHealth();
            bike.resetBikeMotion();
            UpdateUIEnergy();
            UpdateText();
            //Rather than update the location or camera these values are just reset
            //TODO either reset location or do something to ensure that they don't end up going over the edge
    }
    //This seems to exist only to reset the scene
private void GetInput() 
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            resetGame();
        }
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
