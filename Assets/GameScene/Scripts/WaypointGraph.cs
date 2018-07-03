using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{



    // Split the waypoints by type for faster searching
    public List<GameObject> coverPoints;
    public List<GameObject> ambushPoints;




    public CapturePointMap capturePointMap;


    [SerializeField]
    private BoxCollider leftLaneCollider;
    [SerializeField]
    private BoxCollider centerLaneCollider;
    [SerializeField]
    private BoxCollider rightLaneCollider;

    [SerializeField]
    private BoxCollider redSideCollider;
    [SerializeField]
    private BoxCollider blueSideCollider;

    private int caputureLayer;

    // Use this for initialization
    void Start ()
    {

        caputureLayer = LayerMask.NameToLayer("CapturePointLayer");
        coverPoints = new List<GameObject>();
        ambushPoints = new List<GameObject>();

        // Filter the waypoint types
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (var w in waypoints)
        {
            TacticalWaypoint waypoint = w.GetComponent<TacticalWaypoint>();
            // Hide the waypoints when it comes time to gameplay
            w.GetComponent<MeshRenderer>().enabled = false;

            AssignLane(w, waypoint);
            AssignCloserToTeamSide(w, waypoint);
            CheckIfInCapturePoint(w, waypoint);
            

     

            switch (waypoint.waypointType)
            {
                case TacticalWaypoint.WaypointType.CoverPoint:
                {
                    coverPoints.Add(w);
                    break;
                }
                case TacticalWaypoint.WaypointType.AmbushPoint:
                {
                    ambushPoints.Add(w);
                    break;
                }
                default:
                {
                    break;
                }

            }
            
        }
        
    }

    private void CheckIfInCapturePoint(GameObject w, TacticalWaypoint waypoint)
    {
        foreach (var capturePoint in capturePointMap.capturePoints)
        {
            BoxCollider waypointDataTrigger = capturePoint.Value.waypointDataTrigger;
            if (waypointDataTrigger.bounds.Contains(w.transform.position))
            {
                switch (waypoint.waypointType)
                {
                    case TacticalWaypoint.WaypointType.NonePoint:
                        break;
                    case TacticalWaypoint.WaypointType.CoverPoint:
                        capturePoint.Value.coverpointsOnCapturePoint.Add(waypoint);
                        break;
                    case TacticalWaypoint.WaypointType.AmbushPoint:
                        capturePoint.Value.ambushPointsOnCapturePoint.Add(waypoint);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                

                // return as soon as we find one. if it is on one capture point, it will never be on another point.
                // no need to check the other points
                return;
            }
        }
        
    }

    private void AssignLane(GameObject waypoint, TacticalWaypoint wTacticalWaypoint)
    {
        if (leftLaneCollider.bounds.Contains(waypoint.transform.position))
        {
            wTacticalWaypoint.belongsToLane = LaneScript.LaneSide.LeftLane;
        }
        else if (centerLaneCollider.bounds.Contains(waypoint.transform.position))
        {
            wTacticalWaypoint.belongsToLane = LaneScript.LaneSide.CenterLane;
        }
        else if (rightLaneCollider.bounds.Contains(waypoint.transform.position))
        {
            wTacticalWaypoint.belongsToLane = LaneScript.LaneSide.RightLane;
        }
    }

    private void AssignCloserToTeamSide(GameObject waypoint, TacticalWaypoint wTacticalWaypoint)
    {
        if (wTacticalWaypoint.closerToWhichTeamSide != TeamSide.TeamEnum.None)
        {
            // Don't override the team side. This was manually put in the scene view
        }
        else
        {
            if (redSideCollider.bounds.Contains(waypoint.transform.position))
            {
                wTacticalWaypoint.closerToWhichTeamSide = TeamSide.TeamEnum.RedTeam;
            }
            if (blueSideCollider.bounds.Contains(waypoint.transform.position))
            {
                wTacticalWaypoint.closerToWhichTeamSide = TeamSide.TeamEnum.BlueTeam;
            }
        }

    }

    public GameObject FindNearestWayPointByType(Vector3 fromPosition, TacticalWaypoint.WaypointType type)
    {
        List<GameObject> waypointArray = null;
        switch (type)
        {
                
            case TacticalWaypoint.WaypointType.CoverPoint:
                waypointArray = coverPoints;
                break;
            case TacticalWaypoint.WaypointType.AmbushPoint:
                waypointArray = ambushPoints;
                break;
            default:
                break;
        }

        GameObject nearestWaypoint = null;

        if (waypointArray != null)
        {
            float distance = Mathf.Infinity;
            foreach (var w in waypointArray)
            {
                Vector3 diff = w.transform.position - fromPosition;
                if (diff.magnitude < distance)
                {
                    nearestWaypoint = w;
                    distance = diff.magnitude;
                }
            }
        }
        return nearestWaypoint;
    }


    // Find a cover point that is closest to "from position" but is also not in sight of "target vision"
    // fromPosition the starting position that we want to leave from or run away from
    // targetPosition the position that we want to be out of sight. we want a cover to between the cover point and this target position
    // laneSide, only find cover points that are in this lane
    public GameObject FindNearestCoverPoint(Vector3 fromPosition, Vector3 targetPosition, LaneScript.LaneSide laneSide, TeamSide.TeamEnum whichTeamSide)
    {

        GameObject nearestCoverPoint = null;   
        float distance = Mathf.Infinity;

        // Go through all the cover points
        foreach (GameObject w in coverPoints)
        {
            TacticalWaypoint tw = w.GetComponent<TacticalWaypoint>();
            // Skip lanes that are not in the same lane as the current lane we are trying to search for
            if (tw.belongsToLane != laneSide || tw.closerToWhichTeamSide != whichTeamSide)
            {
                continue;
            }

            // Get the distance to compare if we found one closer than a previous one
            Vector3 diff = w.transform.position - fromPosition;
            
            // We found a closer coverpoint
            if (diff.magnitude < distance)
            {
                RaycastHit hit;

                // Check to see if the we are actually in cover or exposed
                if (Physics.Raycast(w.transform.position, targetPosition - w.transform.position, out hit, Mathf.Infinity, caputureLayer))
                {
                    Debug.DrawRay(w.transform.position, targetPosition - w.transform.position, Color.blue, 2);
                    if (hit.collider.CompareTag("Cover"))
                    {
                        // Set the nearest coverpoint to the one that is closest and actually in cover
                        nearestCoverPoint = w;
                        distance = diff.magnitude;
                    }
                }    
            }
        }

        Debug.DrawRay(targetPosition, nearestCoverPoint.transform.position - targetPosition, Color.yellow, 5);
        return nearestCoverPoint;
    }


    public GameObject FindNearestAmbushPoint(Vector3 fromPosition, Vector3 targetPosition, LaneScript.LaneSide laneSide, TeamSide.TeamEnum whichTeamSide)
    {
        GameObject nearestAmbushPoint = null;
        float distance = Mathf.Infinity;

        // Go through all the cover points
        foreach (GameObject w in ambushPoints)
        {
            TacticalWaypoint tw = w.GetComponent<TacticalWaypoint>();
            // Skip lanes that are not in the same lane as the current lane we are trying to search for
            if (tw.belongsToLane != laneSide || tw.closerToWhichTeamSide != whichTeamSide)
            {
                continue;
            }
            Debug.DrawRay(w.transform.position, targetPosition - w.transform.position, Color.blue, 2);
            // Get the distance to compare if we found one closer than a previous one
            Vector3 diff = w.transform.position - fromPosition;

            // We found a closer coverpoint
            if (diff.magnitude < distance)
            {
                // Set the nearest coverpoint to the one that is closest and actually in cover
                nearestAmbushPoint = w;
                distance = diff.magnitude;
            }
        }
        Debug.DrawRay(targetPosition, nearestAmbushPoint.transform.position - targetPosition, Color.yellow, 5);

        return nearestAmbushPoint;
    }




    // Update is called once per frame
    void Update () {
		
	}
}
