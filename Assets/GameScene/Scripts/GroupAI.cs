using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAI : MonoBehaviour {

    // List of all AnchorPoints around the character
    [SerializeField]
    private List<GameObject> listOfAnchorPoints;

    public GameObject capturePointMap;
    
    // Player TeamSide Variable
    private TeamSide playerTeamSide;
    public List<GameObject> listOfCollision;
    private bool isAssembled;


    private List<bool> isAnchorTaken;


    // Use this for initialization
    void Start () {

        // Setting the player's team side
		var comp = GetComponentsInParent<TeamSide>();
        playerTeamSide = comp[0];

        listOfCollision = new List<GameObject>();
        isAssembled = false;

        isAnchorTaken = new List<bool>();
        

        // Populating the Boolean List
        foreach(GameObject obj in listOfAnchorPoints) {
            isAnchorTaken.Add(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
        // No AIs around you to group 
        if(listOfCollision.Count == 0) {
            //Debug.Log("No NPC within range!");
            // isAssembled = false;
        }

        else if(Input.GetKeyDown(KeyCode.E)) {

            if (!isAssembled) {
                assembleGroupAI();
            }
            else {
                disassembleGroupAI();
            }
        }
	}

    // Add NPC GameObject to the list of Group AI
    void OnTriggerEnter(Collider col) {

        TeamSide collisionTeamSide = col.GetComponent<TeamSide>();

        if (col.gameObject.tag == "NPC" && collisionTeamSide != null && (collisionTeamSide.playerTeam == playerTeamSide.playerTeam)) {
            listOfCollision.Add(col.gameObject);
        }
    }

    // Removes NPC GameObject from the list of Group AI
    void OnTriggerExit(Collider col) {
        listOfCollision.Remove(col.gameObject);      
    }


    // On KeyPress, Assembles up to 4 AIs around you
    void assembleGroupAI() {
        isAssembled = true;

        for (int i = 0; i < listOfAnchorPoints.Count; i++) {
            if(i < listOfCollision.Count) {
                listOfCollision[i].gameObject.GetComponent<NpcSimple>().GoToTarget(listOfAnchorPoints[i].transform);
                listOfCollision[i].gameObject.GetComponent<NpcSimple>().isPlayerGrouped = true;
            }
        }
    }


    // On KeyPress, Disassembles all AIs around you
    void disassembleGroupAI() {
        isAssembled = false;

        GameObject disassembleSquad = new GameObject();
        disassembleSquad.AddComponent<NPCSquad>();
        disassembleSquad.layer = LayerMask.NameToLayer("BlueTeam");

        foreach (GameObject obj in listOfCollision) {
            if(obj.gameObject.tag == "NPC") {

                obj.GetComponent<NpcSimple>().isPlayerGrouped = false;

                disassembleSquad.GetComponent<NPCSquad>().listOfNPCs.Add(obj);
                disassembleSquad.GetComponent<NPCSquad>().listOfNPCSimple.Add(obj.GetComponent<NpcSimple>());

                // obj.GetComponent<NpcSimple>().GoToCapturePoint(findNearestCapturePoint(), TacticalWaypoint.WaypointType.NonePoint, false);
                // obj.GetComponent<NpcSimple>().isPlayerGrouped = false;
                // obj.GetComponent<NpcSimple>().GoToTarget(null);
            }
        }

        disassembleSquad.GetComponent<NPCSquad>().assignNewLeader();

        disassembleSquad.GetComponent<NPCSquad>().setSquadTarget(findNearestCapturePoint());

    }


    CapturePoint.CapturePointId findNearestCapturePoint() {

        CapturePoint.CapturePointId pointId = 0;
        float smallestDistance = 999999999.0f;

        foreach(KeyValuePair<CapturePoint.CapturePointId, CapturePoint> point in capturePointMap.GetComponent<CapturePointMap>().capturePoints) {

            // Calculate shortest distance to enemy flags 
            if(!(point.Value.GetComponent<CapturePoint>().belongsToTeam == playerTeamSide.playerTeam)) {
                float distance = Vector3.Distance(point.Value.transform.position, listOfCollision[0].transform.position);

                if(distance < smallestDistance) {
                    smallestDistance = distance;
                    pointId = point.Key;
                }
            }
        }

        return pointId;
    }

}
