using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalWaypoint : MonoBehaviour {

    public enum WaypointType
    {
        NonePoint,
        CoverPoint,
        AmbushPoint,

    }

    public WaypointType waypointType;
    public LaneScript.LaneSide belongsToLane;
    public TeamSide.TeamEnum closerToWhichTeamSide;

    private BoxCollider waypointArea;

	// Use this for initialization
	void Start ()
	{
	    waypointArea = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("NPC"))
        {
            other.GetComponent<NpcSimple>().inTacticalArea[waypointType] = true;
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("NPC"))
        {
            other.GetComponent<NpcSimple>().inTacticalArea[waypointType] = false;
        }
    }

}
