using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets loss screen
/// </summary>
public class Lose : MonoBehaviour
{
    public TextMeshProUGUI lose;
    public Image loseImage;
    public Sprite galaxy;
    // Start is called before the first frame update
    void Start()
    {
        //sets loss screen background, default gravestone
        string temp = PlayerPrefs.GetString("LoseImage");
        if (temp == "galaxy")
        {
            loseImage.sprite = galaxy;
        }
        //sets loss screen text
        lose.text = PlayerPrefs.GetString("Dead");
    }
}
