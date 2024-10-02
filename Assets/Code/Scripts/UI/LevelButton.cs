using System.Collections;
using System.Collections.Generic;
using EditorObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private GameLevel level;
    [SerializeField] private TextMeshProUGUI textArea;

    // Start is called before the first frame update
    void Start()
    {
        if (!level)
        {
            Debug.Log("No level on level select button!");
        }
        else
        {
            Image buttonImage = this.gameObject.GetComponent<Image>();
            buttonImage.sprite = level.LevelImage;
            textArea.text = level.LevelName;
        }
    }
}
