using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/// <summary>
/// Holds event list
/// </summary>
[System.Serializable]
public class EventWrapper
{
	public List<Event> events;
}

/// <summary>
/// Event class for event values
/// </summary>
[System.Serializable]
public class Event
{
	//declare possible variables for event use
	public int number;
	public string title;
	public string story;
	//add story and remove story if an event has this tag
    public string add_story;
	public bool remove_story;
	//left and right special for scaled events
	public bool left_special;
	public bool right_special;
    public string description;
	//age and threat costs
	public int cost;
	public string cost_type;
	public int threat_scale;
	public int threat_meter;
	public int threat_reject;
	//rng events use positive chance
	public int positive_chance;
	//follow up events in general or for accepting/declining certain events
	public int follow_up;
	public int follow_up_positive;
	public int follow_up_negative;
	//tracks if this event is a follow up event to not add to initial list
	public bool is_follow_up;
	//remove up to 5 events based on choices (mostly used for mafia)
	public int remove_event_one;
	public int remove_event_two;
	public int remove_event_three;
	public int remove_event_four;
	public int remove_event_five;
	//text for accepting/declining event
	public string event_accept;
	public string event_accept_positive;
	public string event_accept_negative;
	public string event_reject;
	public string event_reject_positive;
	public string event_reject_negative;
	//reputation additions/subtraction based on event
	public int reputation_accept;
	public int reputation_accept_positive;
	public int reputation_accept_negative;
	public int reputation_reject;
	public int reputation_reject_positive;
	public int reputation_reject_negative;
	//progress used for time anomaly and rogue scientist events
	public int progress;
	//event not removed upon rejecting the person (so the game is always beatable, unless poor choices made)
	public bool dont_remove_reject;
	public bool end_game;

	/// <summary>
	/// Constructor
	/// </summary>
	public Event()
	{
		number = 0;
		title = null;
		story = null;
		add_story = null;
		remove_story = false;
		left_special = false;
		right_special = false;
        description = null;
        cost = -100;
        cost_type = null;
		threat_scale = 0;
		threat_meter = 0;
		threat_reject = 0;
		positive_chance = -100;
        follow_up = -1;
        follow_up_positive = -1;
        follow_up_negative = -1;
        is_follow_up = false;
        remove_event_one = -1;
        remove_event_two = -1;
        remove_event_three = -1;
        remove_event_four = -1;
        remove_event_five = -1;
        event_accept = null;
        event_accept_positive = null;
        event_accept_negative = null;
        event_reject = null;
        event_reject_positive = null;
        event_reject_negative = null;
        reputation_accept = -100;
        reputation_accept_positive = -100;
        reputation_accept_negative = -100;
        reputation_reject = -100;
        reputation_reject_positive = -100;
        reputation_reject_negative = -100;
		progress = 0;
        dont_remove_reject = false;
        end_game = false;
    }
}

/// <summary>
/// EventManager class: creating and removing events
/// </summary>
public class EventManager : MonoBehaviour
{
	public List<Event> allEvents;
	public List<Event> eventList;
	public List<Event> disabledEvents;

	/// <summary>
	/// Loads events
	/// </summary>
	void Awake()
	{
		LoadEvents(Path.Combine(Application.streamingAssetsPath, "ChronomancerEvents.json"));
	}

	/// <summary>
	/// Adds events to initial lists
	/// </summary>
	/// <param name="filePath"></param>
	public void LoadEvents(string filePath)
	{
		//read in .json and wrap events to allEvents list
		string json = System.IO.File.ReadAllText(filePath);
		EventWrapper wrap = JsonUtility.FromJson<EventWrapper>(json);
		allEvents = wrap.events;
		//sort events into correct lists
		foreach (Event e in allEvents)
		{
			if (e.is_follow_up == true || e.story != null)
			{
				disabledEvents.Add(e);
			}
			else
			{
				eventList.Add(e);
            }
		}
	}
	/// <summary>
	/// Returns a random event
	/// </summary>
	/// <returns></returns>
	public Event GetRandomEvent()
	{
		int random = UnityEngine.Random.Range(0, eventList.Count);
		return eventList[random];
	}

	/// <summary>
	/// Adds follow up events
	/// </summary>
	/// <param name="e"></param>
	public void AddEvent(Event e)
	{
		for(int i = 0; i < disabledEvents.Count; i++)
		{
			if (disabledEvents[i].number == e.follow_up ||
                disabledEvents[i].number == e.follow_up_positive)
			{
				eventList.Add(disabledEvents[i]);
			}
		}
	}

	/// <summary>
	/// Adds event based on number: mainly used for story events
	/// </summary>
	/// <param name="num"></param>
	public void AddNumber(int num)
	{
        for (int i = 0; i < disabledEvents.Count; i++)
		{
            if (disabledEvents[i].number == num)
			{
                eventList.Add(disabledEvents[i]);
            }
        }
    }

	/// <summary>
	/// Adds all events with a given story tag
	/// </summary>
	/// <param name="name"></param>
	public void AddStory(string name)
	{
        for (int i = 0; i < disabledEvents.Count; i++)
		{
            if (disabledEvents[i].story != null && disabledEvents[i].story == name)
			{
                eventList.Add(disabledEvents[i]);
            }
        }
    }
	/// <summary>
	/// Removes events with a given story tag
	/// </summary>
	/// <param name="name"></param>
	public void RemoveStory(string name)
	{
        for (int i = 0; i < disabledEvents.Count; i++)
		{
			//prevents overflow
			try
			{
                if (eventList[i].story != null && eventList[i].story == name)
                {
                    eventList.RemoveAt(i);
                    i--;
                }
            }
			catch (Exception e)
			{

			}
		}
    }

	/// <summary>
	/// Adds follow up events that require declining someone
	/// </summary>
	/// <param name="e"></param>
	public void AddNegativeFollowUp(Event e)
	{
        for (int i = 0; i < disabledEvents.Count; i++)
        {
            if (disabledEvents[i].number == e.follow_up_negative)
            {
                eventList.Add(disabledEvents[i]);
                break;
            }
        }
    }

	/// <summary>
	/// Removes event
	/// </summary>
	/// <param name="e"></param>
	public void RemoveEvent(Event e)
	{
		eventList.Remove(e);
	}

	/// <summary>
	/// Removes event with given number
	/// </summary>
	/// <param name="num"></param>
	public void RemoveNumber(int num)
	{
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].number == num)
            {
                eventList.RemoveAt(i);
				break;
            }
        }
    }

	/// <summary>
	/// Checks if an event number is in the list
	/// </summary>
	/// <param name="num"></param>
	/// <returns></returns>
	public bool IsNumInList(int num)
	{
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].number == num)
            {
				return true;
            }
        }
        return false;
	}

	/// <summary>
	/// Removes events with a specific number
	/// </summary>
	/// <param name="e"></param>
	public void RemoveEventNumber(Event e)
	{
		for(int i = 0; i < eventList.Count; i++)
		{
			if (eventList[i].number == e.remove_event_one ||
                eventList[i].number == e.remove_event_two ||
                eventList[i].number == e.remove_event_three ||
                eventList[i].number == e.remove_event_four ||
                eventList[i].number == e.remove_event_five)
			{
				eventList.RemoveAt(i);
				i--;
			}
		}
	}
	/// <summary>
	/// Event list accessor
	/// </summary>
	public List<Event> Events { get { return eventList; } }
}