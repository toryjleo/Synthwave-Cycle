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
    }

    public void Init()
    {
        currentScore = 0;
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
            Debug.LogError("No bike found in the scene.");
        }
        else
        {
            Energy = bike.Energy;
        }

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

    public void ResetGameObject()
    {
        Init();
    }
}
