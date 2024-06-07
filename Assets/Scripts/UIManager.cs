using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UIManager: Handles ui elements
/// </summary>
public class UIManager : MonoBehaviour
{
	public TextMeshProUGUI eventText;
	public TextMeshProUGUI dayText;
	public TextMeshProUGUI ageText;
	public Line reputationLine;
	public Line threatMeterLine;
	public Line threatScaleLine;
	public Image threatMeterImage;
	public Image threatScaleImage;
	public Sprite mafiaSprite;
	public Sprite walkerSprite;
	public Button acceptButton;
	public Button declineButton;
	public Button continueButton;
	public EventManager eventManager;
	public GameManager gameManager;

	Event currentEvent;
	bool endCheck = false;
	double chance = 0;
	int progress = 0;
	string story = "";
	int leftScale = 0;
	int rightScale = 0;
	bool leftSpecial = false;
	bool rightSpecial = false;
	bool leftDone = false;
	bool rightDone = false;

	/// <summary>
	/// Initialize variables and display first event
	/// </summary>
	void Start()
	{
		dayText.text = "Day " + gameManager.currentDay.ToString();
		ageText.text = "Age: " + gameManager.ageYears.ToString();
		reputationLine.UpdateLinePosition(gameManager.reputation);
		currentEvent = null;
		ShowEvent();
	}

	/// <summary>
	/// Shows an event to the player
	/// </summary>
	public void ShowEvent()
	{
		HideEvent();
		Thread.Sleep(500);
		currentEvent = eventManager.GetRandomEvent();
		if(currentEvent != null)
		{
			eventText.text = currentEvent.description;
		}
	}
	/// <summary>
	/// Accept button clicked
	/// </summary>
	public void Accept()
	{
		//hide accept and decline buttons and show continue button
		acceptButton.GetComponent<Button>().gameObject.SetActive(false);
		declineButton.GetComponent<Button>().gameObject.SetActive(false);
		continueButton.GetComponent<Button>().gameObject.SetActive(true);

		//if event is tagged to end game, save it for the end of the day
		if(currentEvent.end_game == true)
		{
			endCheck = true;
		}

		//adds story events if the current event is part of an initial story
		if(currentEvent.add_story != null)
		{
			eventManager.AddStory(currentEvent.add_story);
			story = currentEvent.add_story;
			//set story to mafia
			if(story == "Mafia")
			{
				gameManager.story = "Mafia";
				gameManager.threatScale = 50;
				leftScale = 49;
				rightScale = 37;
				threatScaleImage.sprite = mafiaSprite;
				threatScaleImage.gameObject.SetActive(true);
				threatScaleLine.UpdateLinePosition(gameManager.threatScale);
			}
			//set story to time anomaly
			else if(story == "Time Anomaly")
			{
				gameManager.story = "Time Anomaly";
				gameManager.threatMeter = 0;
				threatMeterImage.sprite = walkerSprite;
				threatMeterImage.gameObject.SetActive(true);
				threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
			}
			//set story to rogue scientists
			else if(story == "Rogue Scientists")
			{
				gameManager.story = "Rogue Scientists";
				gameManager.threatMeter = 0;
				gameManager.threatScale = 50;
				threatMeterImage.gameObject.SetActive(true);
				threatScaleImage.gameObject.SetActive(true);
				threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
				threatScaleLine.UpdateLinePosition(gameManager.threatScale);
			}
		}
		//if current event has threat associated, add it
		if(currentEvent.threat_scale != 0)
		{
			gameManager.threatScale += currentEvent.threat_scale;
		}
		if(currentEvent.threat_meter != 0)
		{
			gameManager.threatMeter += currentEvent.threat_meter;
		}

		//if current event is part of time anomaly or rogue scientists, add progress
		if(currentEvent.progress != 0)
		{
			progress += currentEvent.progress;
			//if progress complete, add time anomaly or rogue scientist ending event
			if(progress >= 100 && currentEvent.story == "Time Anomaly")
			{
				eventManager.AddNumber(71);
				eventManager.RemoveStory(currentEvent.story);
			}
			else if (progress >= 100 && currentEvent.story == "Rogue Scientists")
			{
				eventManager.AddNumber(92);
				eventManager.RemoveStory(currentEvent.story);
			}
		}

		//if the current event doesn't have the default value for reputation on accept, add it
		if(currentEvent.reputation_accept != -100)
		{
			gameManager.reputation += currentEvent.reputation_accept;
		}
		else
		{
			//event has a random chance associated, after rolling chance, add correlating event
			chance = Random.Range(0, 100);
			if(chance < currentEvent.positive_chance)
			{
				gameManager.reputation += currentEvent.reputation_accept_positive;
			}
			else
			{
				gameManager.reputation += currentEvent.reputation_accept_negative;
			}
		}

		//add event age to player age
		if(currentEvent.cost_type == "Week")
		{
			gameManager.ageWeeks += currentEvent.cost;
		}
		else if(currentEvent.cost_type == "Month")
		{
			gameManager.ageMonths += currentEvent.cost;
		}
		else
		{
			gameManager.ageYears += currentEvent.cost;
		}

		//update values after their additions
		gameManager.CheckValues();
		ageText.text = "Age: " + gameManager.ageYears.ToString();
		reputationLine.UpdateLinePosition(gameManager.reputation);
		//checks story related info and updates their bar graph positions if necessary
		if (story == "Mafia")
		{
			threatScaleLine.UpdateLinePosition(gameManager.threatScale);
			//if current event is special for left or right mafia, note it for later
			if (currentEvent.number == leftScale)
			{
				leftDone = true;
			}
			else if (currentEvent.number == rightScale)
			{
				rightDone = true;
			}
		}
		else if(story == "Time Anomaly")
		{
			threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
		}
		else if(story == "Rogue Scientists")
		{
			threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
			threatScaleLine.UpdateLinePosition(gameManager.threatScale);
		}

		//display event accept text if it isn't null
		if (currentEvent.event_accept != null)
		{
			eventText.text = currentEvent.event_accept;
		}
		//display positive/negative text from rng calculated earlier
		else
		{
			if(chance < currentEvent.positive_chance)
			{
				eventText.text = currentEvent.event_accept_positive;
			}
			else
			{
				eventText.text = currentEvent.event_accept_negative;
			}
		}
		//if event has a follow up event, add it
		if(currentEvent.follow_up != -1 || currentEvent.follow_up_positive != -1)
		{
			eventManager.AddEvent(currentEvent);
		}
		//if event removes at least one event, remove them
		if(currentEvent.remove_event_one != -1)
		{
			eventManager.RemoveEventNumber(currentEvent);
		}
		//if event ends a story, remove the remaining story events
		if (currentEvent.remove_story == true)
		{
			eventManager.RemoveStory(currentEvent.story);
			story = "";
		}
		//remove the current event
		eventManager.RemoveEvent(currentEvent);
	}

	/// <summary>
	/// Decline button clicked
	/// </summary>
	public void Decline()
	{
		//hide accept/decline buttons and show continue button
		acceptButton.GetComponent<Button>().gameObject.SetActive(false);
		declineButton.GetComponent<Button>().gameObject.SetActive(false);
		continueButton.GetComponent<Button>().gameObject.SetActive(true);

		//if event doesn't have default reject value, add it to reputation
		if (currentEvent.reputation_reject != -100)
		{
			gameManager.reputation += currentEvent.reputation_reject;
		}
		//event has rng reject
		else
		{
			//calculate rng chance and add correlating value
			chance = Random.Range(0, 100);
			if (chance < currentEvent.positive_chance)
			{
				gameManager.reputation += currentEvent.reputation_reject_positive;
			}
			else
			{
				gameManager.reputation += currentEvent.reputation_reject_negative;
			}
		}
		//if the event adds threat for rejecting, add it (story events only)
		if (currentEvent.threat_reject != 0)
		{
			gameManager.threatScale += currentEvent.threat_reject;
		}

		//update player values
		gameManager.CheckValues();
		ageText.text = "Age: " + gameManager.ageYears.ToString();
		reputationLine.UpdateLinePosition(gameManager.reputation);
		//update line positions for stories if necessary
		if (story == "Mafia")
		{
			threatScaleLine.UpdateLinePosition(gameManager.threatScale);
		}
		else if (story == "Time Anomaly")
		{
			threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
		}
		else if (story == "Rogue Scientists")
		{
			threatScaleLine.UpdateLinePosition(gameManager.threatScale);
			threatMeterLine.UpdateLinePosition(gameManager.threatMeter);
		}

		//if current event has reject text, display it
		if (currentEvent.event_reject != null)
		{
			eventText.text = currentEvent.event_reject;
		}
		//current event has default reject value
		else
		{
			//display rng text from earlier calculation
			if (chance < currentEvent.positive_chance)
			{
				eventText.text = currentEvent.event_reject_positive;
			}
			else
			{
				eventText.text = currentEvent.event_reject_negative;
			}
		}

		//if event adds a follow up upon declining it, add that event
		if (currentEvent.follow_up_negative != -1)
		{
			eventManager.AddNegativeFollowUp(currentEvent);
		}

		//if event shouldn't be removed upon rejecting (mainly for victory purposes), don't remove it
		if (currentEvent.dont_remove_reject != true)
		{
			eventManager.RemoveEvent(currentEvent);
		}
	}

	/// <summary>
	/// Continue button clicked
	/// </summary>
	public void Continue()
	{
		//check story event values
		if(story == "Mafia")
		{
			if(currentEvent.left_special)
			{
				leftSpecial = true;
			}
			else if(currentEvent.right_special)
			{
				rightSpecial = true;
			}

			//if 2 special events aren't in the list and need to be, or vice versa, add/remove them
			if(!leftDone && !eventManager.IsNumInList(leftScale) && (leftSpecial || gameManager.threatScale <= 25))
			{
				eventManager.AddNumber(leftScale);
			}
			else if(eventManager.IsNumInList(leftScale) && gameManager.threatScale > 25 && !leftSpecial)
			{
				eventManager.RemoveNumber(leftScale);
			}

			if (!rightDone && !eventManager.IsNumInList(rightScale) && (rightSpecial || gameManager.threatScale >= 75))
			{
				eventManager.AddNumber(rightScale);
			}
			else if (eventManager.IsNumInList(rightScale) && gameManager.threatScale < 75 && !rightSpecial)
			{
				eventManager.RemoveNumber(rightScale);
			}

		}
		//subtract threat meter for every event
		else if(story == "Time Anomaly" || story == "Rogue Scientists")
		{
			gameManager.threatMeter--;
		}

		//update values
		gameManager.CheckValues();

		//if the game should end in victory, disable buttons and send the player to victory screen
		if (endCheck)
		{
			gameManager.Victory();
			acceptButton.GetComponent<Button>().gameObject.SetActive(false);
			declineButton.GetComponent<Button>().gameObject.SetActive(false);
			continueButton.GetComponent<Button>().gameObject.SetActive(false);
		}
		//check if the day should end, re-enable accept/decline buttons
		else
		{
			gameManager.CheckEndDay();
			acceptButton.GetComponent<Button>().gameObject.SetActive(true);
			declineButton.GetComponent<Button>().gameObject.SetActive(true);
			continueButton.GetComponent<Button>().gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Hides event text
	/// </summary>
	public void HideEvent()
	{
		eventText.text = "";
	}
}
