using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Timers;
using System.Linq;

/// <summary>Class <c>ScoreTracker</c> Component which manages the UI in the upper left of the game screen.</summary>
/// Expects there to be an object with BikeScript in the scene.
public class ScoreTracker : MonoBehaviour, IResettable
{
    private const float MAX_TIME = 90;
    private float currentTime;
    private int currentScore;
    private float _currentEnergy;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentHPText;

    public List<PlayerWeaponType> weaponPool;

    public BikeScript bike;

    public int SCORE_TILL_WEAPON_DROP = 1500;

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
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Resetting!");
            List<IResettable> resetObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IResettable>().ToList();
            foreach (IResettable r in resetObjects)
            {
                r.ResetGameObject();
            }
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
        if((currentScore % SCORE_TILL_WEAPON_DROP) + points >= SCORE_TILL_WEAPON_DROP)
        {
            if(weaponPool != null && weaponPool.Count > 0)
            {
                PlayerWeaponType gun = weaponPool[Random.Range(0, weaponPool.Count)];
                Arsenal a = bike.gameObject.GetComponent<Arsenal>();
                if (a != null)
                {
                    a.EquipGun(gun);
                }
            }
        }
        currentScore += points;
    }

    /// <summary>Updates this class's Energy to the bike's energy.</summary>
    private void UpdateUIEnergy()
    {
        if (bike == null)
        {
            Debug.Log("There is no bike.");
        }
        else
        {
            Energy = bike.Energy;
        }

    }

    /// <summary>Loads the gameover screen.</summary>
    /// <param name="survivedEvent">True if the player beat the level.</param>
    private void EndGame(bool survivedEvent)
    {
        PlayerDataObject.survivedEvent = survivedEvent;
DLevel dLevel = Object.FindObjectOfType<DLevel>();
        if (dLevel == null)
        {
            Debug.Log("cannot find Dlevel Object in scene.");
        }
        else
        {
            Object.FindObjectOfType<DLevel>().dangerTimer.Dispose(); //Dispose of timer for spawning more enemies
            SceneManager.LoadScene("TestScene");
            //StartCoroutine(LoadYourAsyncScene());
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

    public void ResetGameObject()
    {
        Init();
    }
}
