using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneWave : MonoBehaviour {

    // ----------- Class Variables ----------- //  


    [SerializeField]
    public LaneScript.LaneSide laneSide;

    // --- Spawn Points --- 
    [SerializeField]
    private GameObject redSpawnPoint;

    [SerializeField]
    private GameObject blueSpawnPoint;

    // --- Capture Points --- 
    [SerializeField]
    private GameObject blueCapturePoint;

    [SerializeField]
    private GameObject midCapturePoint;

    [SerializeField]
    private GameObject redCapturePoint;

    // NPC Prefab
    [SerializeField]
    private GameObject NPCPrefab;


    public GameObject capturePointMap;

    // --- Private Variables --- //

    // List of Squads
    private List<GameObject> blueSquads;
    private List<GameObject> redSquads;

    // Red/Blue team target to walk to
    [SerializeField] 
    private GameObject blueTarget;
    [SerializeField]
    private GameObject redTarget;

    private bool updateBlueTarget = false;
    private bool updateRedTarget  = false;

    // Use this for initialization
    void Awake() {

        blueSquads = new List<GameObject>();
        redSquads  = new List<GameObject>();

        // Assignment the middle point by default
        // blueTarget = midCapturePoint;
        // redTarget  = midCapturePoint;
    }

    // Update is called once per frame
    void Update() {

        // Remove Null Blue Squads
        for (int i = blueSquads.Count - 1; i >= 0; --i) {
            if (blueSquads[i] == null) {
                blueSquads.RemoveAt(i);
            }
        }

        // Remove Null Red Squads
        for (int i = redSquads.Count - 1; i >= 0; --i) {
            if (redSquads[i] == null) {
                redSquads.RemoveAt(i);
            }
        }

        // ======== Updating the new Capture Point Targets for Blue Team on Spawn ======== //

        // There are no more enemies on the point, proceed to next capture point

        blueTarget = nearestEnemyCapturePoint(TeamSide.TeamEnum.BlueTeam);

        // Assign new Capture Point on Successful Capture 
        foreach (GameObject squad in blueSquads) {
            if (squad != null && isPointClearForBlue(squad)) {
                squad.GetComponent<NPCSquad>().setSquadTarget(blueTarget.GetComponent<CapturePoint>().capturePointId);
            }
        }

        // ======== Updating the new Capture Point Targets for Red Team on Spawn ======== //

        // There are no more enemies on the point, proceed to next capture point
        redTarget = nearestEnemyCapturePoint(TeamSide.TeamEnum.RedTeam);

        // Assign new Capture Point on Successful Capture 
        foreach (GameObject squad in redSquads) {
            if (squad != null && isPointClearForRed(squad)) {
                squad.GetComponent<NPCSquad>().setSquadTarget(redTarget.GetComponent<CapturePoint>().capturePointId);
            }
        }


        // TEST KEY PRESS TO SPAWN WAVE AND MOVE TO CLOSEST COVER POINT
        if (Input.GetKeyDown(KeyCode.O)) {
            spawnMinionToCoverPoint(blueSpawnPoint, TeamSide.TeamEnum.BlueTeam, midCapturePoint);
            spawnMinionToCoverPoint(redSpawnPoint, TeamSide.TeamEnum.RedTeam, midCapturePoint);
        }

        // TEST KEY PRESS TO SPAWN WAVE ON EACH SIDE
        if (Input.GetKeyDown(KeyCode.I)) {
            spawnBlueWave();
            spawnRedWave();
        }

        // TEST KEY PRESS TO SPAWN WAVE AND MOVE TO CLOSEST AMBUSH POINT
        if (Input.GetKeyDown(KeyCode.U)) {
            spawnMinionToAmbushPoint(blueSpawnPoint, TeamSide.TeamEnum.BlueTeam, midCapturePoint);
            spawnMinionToAmbushPoint(redSpawnPoint, TeamSide.TeamEnum.RedTeam, midCapturePoint);
        }

    }

    // Spawn Blue Wave
    public void spawnBlueWave() {
        spawnMinion(blueSpawnPoint, TeamSide.TeamEnum.BlueTeam, blueTarget);
    }

    // Spawn Red Wave
    public void spawnRedWave() {
        spawnMinion(redSpawnPoint, TeamSide.TeamEnum.RedTeam, redTarget);
    }

    // Function to spawn Units on call 
    void spawnMinion(GameObject spawnPoint, TeamSide.TeamEnum side, GameObject targetLocation) {

        GameObject squad = new GameObject();
        squad.AddComponent<NPCSquad>();
        squad.GetComponent<NPCSquad>().setPrefabObject(NPCPrefab);
        squad.GetComponent<NPCSquad>().spawnSquad(spawnPoint.transform.position, side);
        squad.GetComponent<NPCSquad>().setSquadTarget(targetLocation.GetComponent<CapturePoint>().capturePointId);

        if(side == TeamSide.TeamEnum.BlueTeam) {
            blueSquads.Add(squad);
        }

        else {
            redSquads.Add(squad);
        }
        
    }

    // Function to spawn Units on call 
    void spawnMinionToCoverPoint(GameObject spawnPoint, TeamSide.TeamEnum side, GameObject targetLocation) {

        GameObject squad = new GameObject();
        squad.AddComponent<NPCSquad>();
        squad.GetComponent<NPCSquad>().setPrefabObject(NPCPrefab);
        squad.GetComponent<NPCSquad>().spawnSquad(spawnPoint.transform.position, side);


        squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().assignedLane = laneSide;

        squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().GoToNearestCoverAround(targetLocation.transform.position);


    }

    // Function to spawn Units on call 
    void spawnMinionToAmbushPoint(GameObject spawnPoint, TeamSide.TeamEnum side, GameObject targetLocation) {

        GameObject squad = new GameObject();
        squad.AddComponent<NPCSquad>();
        squad.GetComponent<NPCSquad>().setPrefabObject(NPCPrefab);
        squad.GetComponent<NPCSquad>().spawnSquad(spawnPoint.transform.position, side);
        // squad.GetComponent<NPCSquad>().setSquadTarget(targetLocation.GetComponent<CapturePoint>().capturePointId);
        squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().assignedLane = laneSide;
        squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().GoToNearestAmbushAround(targetLocation.transform.position);


    }


    // Returns the Moral Value for this lane
    // [ Negative Value = Red Advantage; Positive Value = Blue Advantage ]
    public int laneMoralValue() {
        return (caputrePointMoralValue(blueCapturePoint) + caputrePointMoralValue(midCapturePoint) + caputrePointMoralValue(redCapturePoint));
    }


    // Gets the Moral Value of the capture point 
    int caputrePointMoralValue(GameObject capturePoint) {
        if (capturePoint.GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.BlueTeam) {
            return 1;
        }

        else if (capturePoint.GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam) {
            return -1;
        }

        else {
            return 0;
        }
    }


    GameObject nearestEnemyCapturePoint(TeamSide.TeamEnum side) {

        Vector3 spawnPos;
        GameObject cloestEnemyCapPoint = null; 
        float smallestDistance = 99999999.0f;

        if(side == TeamSide.TeamEnum.BlueTeam) {
            spawnPos = blueSpawnPoint.transform.position;
        }

        else if(side == TeamSide.TeamEnum.RedTeam){
            spawnPos = redSpawnPoint.transform.position;
        }

        else {
            return null;
        }

        // ======== Prioritize Neutral Capture Points ======== //
        if (blueCapturePoint.GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.None) {
            return blueCapturePoint;
        }

        else if (midCapturePoint.GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.None) {
            return midCapturePoint;
        }

        else if (redCapturePoint.GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.None) {
            return redCapturePoint;
        }

        // Distance from point to BlueCapturePoint
        float blueCapPointDist = Vector3.Distance(blueCapturePoint.transform.position, spawnPos);

        if (blueCapPointDist < smallestDistance && (blueCapturePoint.GetComponent<CapturePoint>().belongsToTeam != side)) {
            smallestDistance = blueCapPointDist;
            cloestEnemyCapPoint = blueCapturePoint;

            // Debug.Log("BLUE CAP POINT");
        }

        // Distance from point to MidCapturePoint
        float midCapPointDist = Vector3.Distance(midCapturePoint.transform.position, spawnPos);

        if (midCapPointDist < smallestDistance && (midCapturePoint.GetComponent<CapturePoint>().belongsToTeam != side)) {
            smallestDistance = midCapPointDist;
            cloestEnemyCapPoint = midCapturePoint;

            // Debug.Log("MID CAP POINT");
        }

        // Distance from point to BlueCapturePoint
        float redCapPointDist = Vector3.Distance(redCapturePoint.transform.position, spawnPos);

        if (redCapPointDist < smallestDistance && (redCapturePoint.GetComponent<CapturePoint>().belongsToTeam != side)) {
            smallestDistance = redCapPointDist;
            cloestEnemyCapPoint = redCapturePoint;

            // Debug.Log("RED CAP POINT");
        }


        // FIND FURTHEST CAPTURE POINT
        if (cloestEnemyCapPoint == null) {

            GameObject furtherPoint = null;
            float largestDistance = 0.0f;

            // Distance from point to BlueCapturePoint
            float blueCapPointDistLong = Vector3.Distance(blueCapturePoint.transform.position, spawnPos);

            if (blueCapPointDistLong > largestDistance && (blueCapturePoint.GetComponent<CapturePoint>().belongsToTeam == side)) {
                largestDistance = blueCapPointDistLong;
                furtherPoint = blueCapturePoint;

                // Debug.Log("BLUE CAP POINT");
            }

            // Distance from point to MidCapturePoint
            float midCapPointDistLong = Vector3.Distance(midCapturePoint.transform.position, spawnPos);

            if (midCapPointDistLong > largestDistance && (midCapturePoint.GetComponent<CapturePoint>().belongsToTeam == side)) {
                largestDistance = midCapPointDistLong;
                furtherPoint = midCapturePoint;

                // Debug.Log("MID CAP POINT");
            }

            // Distance from point to BlueCapturePoint
            float redCapPointDistLong = Vector3.Distance(redCapturePoint.transform.position, spawnPos);

            if (redCapPointDistLong > largestDistance && (redCapturePoint.GetComponent<CapturePoint>().belongsToTeam == side)) {
                largestDistance = redCapPointDistLong;
                furtherPoint = redCapturePoint;

                // Debug.Log("RED CAP POINT");
            }

            return furtherPoint;
        }

        else {
            return cloestEnemyCapPoint;
        }
    }




    bool isPointClearForBlue(GameObject squad) {

        bool isAtCapPoint = squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().IsAtCapturePoint;
        CapturePoint.CapturePointId capId = squad.GetComponent<NPCSquad>().capPointId;

        CapturePoint point = null;

        foreach (KeyValuePair<CapturePoint.CapturePointId, CapturePoint> cp in capturePointMap.GetComponent<CapturePointMap>().capturePoints) {

            if (cp.Key == capId) {
                point = cp.Value;
            }
        }

        if (isAtCapPoint && (point.GetComponent<CapturePoint>().redPlayersInside != 0)) {
            return false;
        }
        

        return true;
    }

    bool isPointClearForRed(GameObject squad) {

        bool isAtCapPoint = squad.GetComponent<NPCSquad>().listOfNPCs[0].GetComponent<NpcSimple>().IsAtCapturePoint;
        CapturePoint.CapturePointId capId = squad.GetComponent<NPCSquad>().capPointId;

        CapturePoint point = null;

        foreach (KeyValuePair<CapturePoint.CapturePointId, CapturePoint> cp in capturePointMap.GetComponent<CapturePointMap>().capturePoints) {

            if (cp.Key == capId) {
                point = cp.Value;
            }
        }

        if (isAtCapPoint && (point.GetComponent<CapturePoint>().bluePlayersInside != 0)) {
            return false;
        }
        

        return true;
    }

}
