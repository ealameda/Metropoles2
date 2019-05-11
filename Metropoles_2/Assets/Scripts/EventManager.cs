using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Building Events. Some useful information can be gleaned from the building object.

	// A building is snapped when a user is selecting a building and hovers over an empty zone slot.
	// The building is now trying to get to that slot. 
	void buildingSnapEvent(GameObject building)
	{
		Vector3 position = building.transform.position;
		Quaternion rotation = building.transform.rotation;

		Grabable buildingScript = building.GetComponent<Grabable>();
		// GameObject manipulator = buildingScript.getManipulator();
		// GameObject zone = buildingScript.getZone();
		// Vector3? snappedPosition = buildingScript.getSnappedPosition();
		Debug.Log("Building " + building.name + " Snapped.");
	}

	// A building is unsnapped when it gets unmoored from a slot. 
	void buildingUnsnapEvent (GameObject building)
	{
		Debug.Log("Building " + building.name + " Unsnapped.");
	}

	// A building is placed when it locks into a slot. As in, when it actually gets to that slot.
	void buildingPlaceEvent(GameObject building)
	{
		Debug.Log("Building " + building.name + " Placed.");	
	}

	// A building is highlighted when a manipulator points to it, but before selection.
	void buildingHighlightEvent(GameObject building)
	{
		Debug.Log("Building " + building.name + " Highlighted.");
		// if (building.transform.childCount > 1)
		// 	building.transform.GetChild(1).gameObject.SetActive(true);
    }

	// A building is unhighlighted when a manipulator stops pointing at it. 
	void buildingUnhighlightEvent(GameObject building)
	{
		Debug.Log("Building " + building.name + " Unhighlighted.");
		// if (building.transform.childCount > 1)
	    //     building.transform.GetChild(1).gameObject.SetActive(false);
    }

	// A building is selected when it become manipulated. 
	void buildingSelectEvent(GameObject building)
	{
		Debug.Log("Building " + building.name + " Selected.");	
	}

	// A building is unselected when it stops being manipulated. 
	void buildingUnselectEvent(GameObject building)
	{
		Debug.Log("Building " + building.name + " Unselected.");	
	}

	// A building is moved whenever it gets a new position to move towards. 
	// WARNING: This happens every frame when the building is manipulated. 
	void buildingMoveEvent(GameObject building)
	{

	}
}
