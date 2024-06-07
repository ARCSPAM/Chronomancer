using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Game manager class: controls events per day and victory/defeat conditions
/// </summary>
public class GameManager : MonoBehaviour
{
	//public static GameManager gameManager;
	public UIManager uiManager;
	public int currentDay = 1;
	public float ageYears = 20f;
	public float ageMonths = 0f;
	public float ageWeeks = 0f;
	public int reputation = 0;
	public FadeEffect fadeEffect;
	public int threatMeter = 0;
	public int threatScale = 10;
	public string story = "None";

	int eventsPerDay = 1;

	/*void Awake()
	{
		if (gameManager == null)
		{
			gameManager = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}*/

	/// <summary>
	/// Calls fade coroutine
	/// </summary>
	void Fade()
	{
		fadeEffect.Fade();
	}

	/// <summary>
	/// Checks if the day should end
	/// </summary>
	public void CheckEndDay()
	{
		//subtracts one after every event
		eventsPerDay--;
		//if events are done for the day, reset events based on player reputation
		if(eventsPerDay < 1)
		{
			if(reputation > 20)
			{
				eventsPerDay = 3;
			}
			else if(reputation > 10)
			{
				eventsPerDay = 2;
			}
			else
			{
				eventsPerDay = 1;
			}
			//
			EndDay();
		}
	}

	/// <summary>
	/// Ends the day
	/// </summary>
	private void EndDay()
	{
		//call fade coroutine
		Fade();
		//checks if any defeat conditions are met
		if(CheckDeath())
		{
			Defeat();
		}
		//checks if rogue scientist story should end in a loss
		if(story == "Rogue Scientists")
		{
			//if scientists helped too much, add scientist arrested event
			if(threatMeter >= 100)
			{
				uiManager.eventManager.AddNumber(95);
				uiManager.eventManager.RemoveStory("Rogue Scientists");
				//prevents multiple additions of event
				threatMeter = 99;
			}
			//if don helped too much, add ron leaving event
			else if(threatScale >= 100)
			{
				uiManager.eventManager.AddNumber(93);
				uiManager.eventManager.RemoveStory("Rogue Scientists");
				//prevents multiple additions of event
				threatScale = 99;
			}
			//if ron helped too much, add don leaving event
			else if (threatScale <= 0)
			{
				uiManager.eventManager.AddNumber(94);
				uiManager.eventManager.RemoveStory("Rogue Scientists");
				//prevents multiple additions of event
				threatScale = 1;
			}
		}
		//adds to day counter
		currentDay++;
		uiManager.dayText.text = "Day " + currentDay.ToString();
	}

	/// <summary>
	/// Keeps values within their bounds
	/// </summary>
	public void CheckValues()
	{
		//keeps reputation between 0 and 30
		if(reputation > 30)
		{
			reputation = 30;
		}
		else if(reputation < 0)
		{
			reputation = 0;
		}
		//keeps age (week, month, year) in range
		if(ageWeeks > 3)
		{
			ageMonths++;
			ageWeeks -= 4;
		}
		if(ageMonths > 11)
		{
			ageYears++;
			ageMonths -= 12;
		}
		//keeps threat at 0
		if (threatMeter < 0)
		{
			threatMeter = 0;
		}
	}

	/// <summary>
	/// Sends to victory screen
	/// </summary>
	public void Victory()
	{
		SceneManager.LoadScene("Win");
	}
	/// <summary>
	/// Learns cause of loss and sends player to correct loss screen
	/// </summary>
	public void Defeat()
	{
		string temp = "";
		//age greater than death value
		if(ageYears > 75f)
		{
			temp = "You died of old age!";
		}
		//checks story losses
		else if (story != "None")
		{
			//if agent walker events done too fast
			if (story == "Time Anomaly" && threatMeter >= 100)
			{
				string image = "galaxy";
				PlayerPrefs.SetString("LoseImage", image);
				temp = "Agent Walker accelerated the unraveling of the universe!";
			}
			//checks mafia meter
			else if (story == "Mafia")
			{
				//if too many police events done
				if(threatScale >= 100)
				{
					temp = "You were killed by the mafia!";
				}
				//if too many mafia events done
				else
				{
					temp = "You were arrested by the police!";
				}
			}
		}
		//otherwise, day count exceeded 50
		else
		{
			string image = "galaxy";
			PlayerPrefs.SetString("LoseImage", image);
			temp = "The cosmic collapse unraveled the universe!";
		}
		//sets loss text and sends to loss screen
		PlayerPrefs.SetString("Dead", temp);
		SceneManager.LoadScene("Lose");
	}

	/// <summary>
	/// Checks if the game should end
	/// </summary>
	/// <returns></returns>
	public bool CheckDeath()
	{
		//outside age range
		if(ageYears > 75f)
		{
			return true;
		}
		//outside day range
		else if (currentDay > 50)
		{
			return true;
		}
		//checks story reasons
		else if(story != "None")
		{
			//if meter is too high and agent walker events done too fast
			if(story == "Time Anomaly" && threatMeter >= 100)
			{
				return true;
			}
			//if mafia event scale out of range
			else if(story == "Mafia" && (threatScale >= 100 || threatScale <= 0))
			{
				return true;
			}
		}
		return false;
	}
}
