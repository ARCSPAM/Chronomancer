using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls pause menu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if escape pressed, display pause menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //disable menu if paused, otherwise enable it
            if(paused)
            {
                pauseMenu.SetActive(false);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
            //swap if menu is paused
            paused = !paused;
        }
    }
}
