using System.Collections;
using System.Collections.Generic;
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
  private PlayerHealth playerHealth;

  // Start is called before the first frame update
  void Start()
  {
    dangerLevelSlider.value = 0;
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
  }

  // Update is called once per frame
  void Update()
  {
    dangerLevelSlider.value = DangerLevel.Instance.PercentProgress;
  }

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
}
