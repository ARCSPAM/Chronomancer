using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls for all menu buttons
/// </summary>
public class MenuButtons : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI buttonText;
    public Image topImage;
    public Image reputationImage;
    public Image scientistImage;
    public Image mafiaImage;
    public Image walkerImage;
    int buttonPresses;
    /// <summary>
    /// Start method
    /// </summary>
    void Start()
    {
        Time.timeScale = 1.0f;
        buttonPresses = 0;
    }

    /// <summary>
    /// Checks which part of the tutorial the player is on and shows correlating images
    /// </summary>
    public void IncrementButton()
    {
        buttonPresses++;
        switch(buttonPresses)
        {
            case 0:
                tutorialText.text = "You are a novice chronomancer, who learned the powers of time manipulation from your late parents.";
                break;
            case 1:
                tutorialText.text = "You found an old journal from your father, detailing an event he dubbed the 'cosmic collapse.'";
                break;
            case 2:
                tutorialText.text = "You have learned that time will unravel itself in 50 days. Untold damage would be caused.";
                break;
            case 3:
                tutorialText.text = "You are merely a novice, and would have no way of stopping this event yourself.";
                break;
            case 4:
                tutorialText.text = "You opened a stand in a marketplace, offering time to people who may want it.";
                break;
            case 5:
                tutorialText.text = "This time allows others to visit the past, but not change the present day.";
                break;
            case 6:
                tutorialText.text = "The time is pulled from yourself, and you will begin to age as you give time.";
                break;
            case 7:
                tutorialText.text = "Assuming the right people hear of your stand, you can stop the cosmic collapse.";
                break;
            case 8:
                topImage.gameObject.SetActive(true);
                tutorialText.text = "The current day and your age are displayed at the top of the screen. You have 50 days and roughly 55 more years of your life to work with.";
                break;
            case 9:
                topImage.gameObject.SetActive(false);
                reputationImage.gameObject.SetActive(true);
                tutorialText.text = "Your reputation is displayed in the bottom right. The higher your reputation, the more people will show up per day.";
                break;
            case 10:
                tutorialText.text = "Your reputation among the community will typically go up by helping people, and go down by turning people away.";
                break;
            case 11:
                reputationImage.gameObject.SetActive(false);
                scientistImage.gameObject.SetActive(true);
                tutorialText.text = "Some people may attract unwanted ire from others. These come in forms of threat, displayed in the bottom left.";
                break;
            case 12:
                scientistImage.gameObject.SetActive(false);
                mafiaImage.gameObject.SetActive(true);
                tutorialText.text = "Scaled threat starts at the middle. Reaching either end of the scale could result in consequences.";
                break;
            case 13:
                mafiaImage.gameObject.SetActive(false);
                walkerImage.gameObject.SetActive(true);
                tutorialText.text = "Metered threat starts at the left side. Reaching the right side of this meter could result in consequences.";
                break;
            case 14:
                walkerImage.gameObject.SetActive(false);
                tutorialText.text = "Do your best to stop the cosmic collapse!";
                buttonText.text = "Play Game";
                break;
            //if all screens complete, go to game
            default:
                GoToGame();
                break;
        }
    }
    /// <summary>
    /// Sends to main game scene
    /// </summary>
    public void GoToGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    /// <summary>
    /// Sends to tutorial
    /// </summary>
    public void GoToHelpMenu()
    {
        SceneManager.LoadScene("HelpMenu");
    }
    /// <summary>
    /// Sends to main menu
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    /// <summary>
    /// Exits game
    /// </summary>
    public void ExitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
