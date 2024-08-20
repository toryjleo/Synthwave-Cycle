using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField]
    public Slider dangerLevelSlider;

    [SerializeField]
    public Slider healthSlider;
    [SerializeField]
    public Image healthBarFill;
    [SerializeField]
    public Image healthBarBackground;

    [SerializeField]
    public TextMeshProUGUI timerText;
    [SerializeField]
    public GameObject boundsWarning;

    [SerializeField]
    private GameObject transmissionRadio;

    private BoundsChecker boundsChecker;
    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
      // Danger Level
      dangerLevelSlider.value = 0;

      // Health
      playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
      if (!playerHealth)
      {
        Debug.LogWarning("No Player Health object found.");
      }
      else
      {
        playerHealth.onHealthChange += UpdateHealth;
        healthSlider.value = playerHealth.PercentProgress;
      }
      healthBarFill.color = Color.red;
      healthBarBackground.color = Color.clear;

      // OOB Timer
      boundsChecker = GameObject.FindObjectOfType<BoundsChecker>();
      if (!boundsChecker)
      {
        Debug.LogWarning("No Bounds Checker object found.");
      }
      else
      {
        boundsChecker.NotifyTimerEvent += UpdateTimer;
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (DangerLevel.Instance)
      {
        dangerLevelSlider.value = DangerLevel.Instance.PercentProgress;
      }

      timerText.text = boundsChecker.TimeLeft.ToString("0.00");

        if (Input.GetKeyDown(KeyCode.Delete)) 
        {
            transmissionRadio.SetActive(!transmissionRadio.active);
        }
    }

    /// <summary>
    /// UpdateHealth sets the health slider (defined in inspector) and sets the health bar fill and background colors
    /// according to the PlayerHealth's CurrentBar value
    /// </summary>
    private void UpdateHealth()
    {
      healthSlider.value = playerHealth.PercentProgress;

      switch (playerHealth.CurrentBar)
      {
        case PlayerHealth.BarMax.Bar1:
          healthBarFill.color = Color.red;
          healthBarBackground.color = Color.clear;
          break;
        case PlayerHealth.BarMax.Bar2:
          healthBarFill.color = Color.yellow;
          healthBarBackground.color = Color.red;
          break;
        case PlayerHealth.BarMax.Bar3:
          healthBarFill.color = Color.green;
          healthBarBackground.color = Color.yellow;
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timerIsOn"></param>
    private void UpdateTimer(bool timerIsOn)
    {
        timerText.gameObject.SetActive(timerIsOn);
        boundsWarning.gameObject.SetActive(timerIsOn);
    }
}
