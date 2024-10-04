using System.Collections;
using System.Collections.Generic;
using EditorObject;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual level select button functions, loads level and starts main scene on click
/// </summary>
public class LevelButton : MonoBehaviour
{
    [SerializeField] private GameLevel level;
    [SerializeField] private TextMeshProUGUI textArea;

    private Button button;
    private MainMenu menu;
    private LevelSelector selector;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        if (!button)
        {
            Debug.LogError("No button reference on level select button!");
        }
        else
        {
            // Find main menu script
            menu = FindObjectOfType<MainMenu>();
            if (!menu)
            {
                Debug.LogError("No menu to load level!");
            }

            // Check field for level
            if (!level)
            {
                Debug.LogError("No level on level select button!");
            }
            else
            {
                Image buttonImage = this.gameObject.GetComponent<Image>();
                buttonImage.sprite = level.LevelImage;
                textArea.text = level.LevelName;
            }

            // Find level selector
            selector = FindObjectOfType<LevelSelector>();
            if (!selector)
            {
                Debug.LogError("No level selector in scene!");
            }

            //Set event handler
            button.onClick.AddListener(HandleOnClick);
        }
    }

    /// <summary>
    /// Attached to button onClick event
    /// </summary>
    private void HandleOnClick()
    {
        selector.SetSelectedLevel(level);
        menu.StartOnClick();
    }
}
