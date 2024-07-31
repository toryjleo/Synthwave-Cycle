using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
  [SerializeField]
  public Slider dangerLevelSlider;

  // Start is called before the first frame update
  void Start()
  {
    dangerLevelSlider.value = 0;
  }

  // Update is called once per frame
  void Update()
  {
    dangerLevelSlider.value = DangerLevel.Instance.PercentProgress;
  }
}
