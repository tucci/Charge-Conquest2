using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;
using Random = System.Random;

public class NPCSquad : MonoBehaviour {


    public enum FiringHeuristic
    {
        OneToManyFire, // Each squad mate fires an independant targets. BUt moves to another target when there is no target
        FocusFire, // All squad mates fire at one enemny
    }

    [SerializeField]
    private GameObject NPCPrefab;


    private SphereCollider shootingAreaTrigger;

    [SerializeField]
    public int engageTargetCount = 0;
    private int maxTargets = 5;
    [SerializeField]
    public CharacterScript[] shootingTargets;
    [SerializeField]
    private FiringHeuristic firingHeuristic;
    

    [SerializeField]
    private GameObject target;
    public CapturePoint.CapturePointId capPointId;

    // List of Squad NPCs
    [SerializeField]
    public List<GameObject> listOfNPCs;
    public List<NpcSimple> listOfNPCSimple;
    private int lastCount;
    private bool isSpawned;

    private BehaviorTree bt;

    [SerializeField]
    private float firingModeTime = 0;
    [SerializeField]
    private float changeFiringModeAfter = 5;



    // Use this for initialization
    void Awake () {
        listOfNPCs = new List<GameObject>();
        listOfNPCSimple = new List<NpcSimple>();
        isSpawned  = false;
        shootingTargets = new CharacterScript[maxTargets];
	    shootingAreaTrigger = gameObject.AddComponent<SphereCollider>();
	    shootingAreaTrigger.radius = 20;
	    shootingAreaTrigger.isTrigger = true;


        // Start behavior tree
        bt = new BehaviorTree(Application.dataPath + "/GameScene/Scripts/npcsquad_firing.xml", this);
        bt.Start();
    }


    void Start() {

    }

    // Update is called once per frame
    void Update () {

        // Removes deleted objects from the list
        if (listOfNPCs.Count > 0) {
            // Deleting like this will give invalid operation excpetions
            // Cant modify list while iterating over it forward
            //foreach (GameObject obj in listOfNPCs) {
            //    if (obj == null) {
            //        listOfNPCs.Remove(obj);
            //        listOfNPCSimple.Remove(null);
            //    }
            //}

            // In order to delete without getting errors, we have to do it in reverse
            for (int i = listOfNPCs.Count - 1; i >= 0; --i)
            {
                if (listOfNPCs[i] == null || listOfNPCs[i].GetComponent<NpcSimple>().isPlayerGrouped)
                {
                    listOfNPCs.RemoveAt(i);
                    listOfNPCSimple.RemoveAt(i);
                }
            }
        }
        



        // TEST KEY PRESS
        //if (Input.GetKeyDown(KeyCode.P)) {
        //    spawnSquad(this.transform.position, TeamSide.TeamEnum.BlueTeam);
        //    setSquadTarget(target);
        //}

        // Assign a new Leader when it is killed
        if(!checkLeader() && (listOfNPCs.Count > 0)) {
            assignNewLeader();
        }

        // Destroy this Squad once unit count reaches 0
        if (listOfNPCs.Count == 0 && isSpawned)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Update the squad update to follow the leader
            // gameObject.transform.position = listOfNPCs[0].transform.position;
        }

        if (listOfNPCSimple != null && (listOfNPCSimple.Count > 0) && engageTargetCount != 0)
        {

            listOfNPCSimple[0].PauseMovement(true);
            EngageTargets();
        }
        else if(listOfNPCSimple != null && (listOfNPCSimple.Count > 0)) {
            listOfNPCSimple[0].PauseMovement(false);
        }




    }


    // Function to spawn a Squad of 5 units 
    public void spawnSquad(Vector3 spawnPosition, TeamSide.TeamEnum side) {

        // TODO: set the npcs lane
        // Instantiate the Squad Leader
        GameObject leader = Instantiate(NPCPrefab, spawnPosition, Quaternion.identity);
        leader.GetComponent<TeamSide>().playerTeam = side;
        leader.GetComponent<NpcSimple>().isNPCLeader = true;
        gameObject.layer = side == TeamSide.TeamEnum.RedTeam ? LayerMask.NameToLayer("RedTeam"): LayerMask.NameToLayer("BlueTeam");
        


        listOfNPCs.Add(leader);
        listOfNPCSimple.Add(leader.GetComponent<NpcSimple>());

        // Instantiate the four Squad Members
        for (int i = 0; i < 4; i++) {

            GameObject anchor = leader.GetComponent<NpcSimple>().listOfAnchorPoints[i];
            // TODO: set the npcs lane
            GameObject member = Instantiate(NPCPrefab, anchor.transform.position, Quaternion.identity);
            member.GetComponent<TeamSide>().playerTeam = side;
            member.GetComponent<NpcSimple>().GoToTarget(anchor.transform);
            listOfNPCs.Add(member);
            listOfNPCSimple.Add(member.GetComponent<NpcSimple>());
        }

        // Setting checkpoint of previous count
        lastCount = listOfNPCs.Count;
        isSpawned = true;
    }


    // Setting the target of the Squad
    public void setSquadTarget(CapturePoint.CapturePointId capturePointId) {
        // listOfNPCs[0].GetComponent<NpcSimple>().GoToTarget(target.transform);
        listOfNPCs[0].GetComponent<NpcSimple>().GoToCapturePoint(capturePointId, TacticalWaypoint.WaypointType.NonePoint, false);
        //listOfNPCs[0].GetComponent<NpcSimple>().GoToNearestCoverAround(new Vector3(14.93796f, -0.003662109f, 114.7996f));
        //listOfNPCs[0].GetComponent<NpcSimple>().StayStillOnPoint(true);
        capPointId = capturePointId;
    }


    // Sets the prefab of the NPC
    public void setPrefabObject(GameObject prefab) {
        NPCPrefab = prefab;
    }

    // Automatically assign a new team leader
    public void assignNewLeader() {
        GameObject leader = listOfNPCs[0];
        leader.GetComponent<NpcSimple>().isNPCLeader = true;
        setSquadTarget(capPointId);

        for (int i = 0; i < listOfNPCs.Count - 1; i++) {

            listOfNPCSimple[i + 1].GoToTarget(leader.GetComponent<NpcSimple>().listOfAnchorPoints[i].transform);
            //listOfNPCs[i + 1].GetComponent<NpcSimple>().GoToTarget(leader.GetComponent<NpcSimple>().listOfAnchorPoints[i].transform);
        }
    }


    // Check if we have a leader still 
    bool checkLeader() {
        foreach(NpcSimple obj in listOfNPCSimple) {
            if(obj.isNPCLeader) {
                return true;
            }
        }

        // There is no leader in our list of NPC
        return false;
    }

    void OnTriggerEnter(Collider other) {

        if (other.CompareTag("NPC") || other.CompareTag("Player"))
        {
            ++engageTargetCount;
            CharacterScript targetChar = other.GetComponent<CharacterScript>();
            shootingTargets[Mathf.Abs(targetChar.GetInstanceID()) % maxTargets] = targetChar;
        }
        
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("NPC") || other.CompareTag("Player"))
        {
            --engageTargetCount;

            CharacterScript targetChar = other.GetComponent<CharacterScript>();
            shootingTargets[Mathf.Abs(targetChar.GetInstanceID()) % maxTargets] = null;
        }
        
    }

    void EngageTargets() {

        // If I had more time, I would do this properly. 
        // This whole thing is a hack
        for (int i = 0; i < listOfNPCSimple.Count; ++i) {
            switch (firingHeuristic) {
                case FiringHeuristic.FocusFire:
                {
                    CharacterScript targetToShoot = null;
                    for (int j = 0; j < shootingTargets.Length; ++j) {
                        if (shootingTargets[j] != null) {
                            targetToShoot = shootingTargets[j];
                            // Ugly hack to break out of the loop
                            j = shootingTargets.Length + 1;
                            //break;
                        }
                    }
                    if (targetToShoot != null) {
                        listOfNPCSimple[i].ShootAtPosition(targetToShoot.transform.position);
                    }
                    
                    break;
                }
                    
                case FiringHeuristic.OneToManyFire: {
                        
                    CharacterScript targetToShoot = shootingTargets[i];
                    for (int j = i; j < maxTargets + i; ++j) {
                        targetToShoot = shootingTargets[j  % maxTargets];
                        if (targetToShoot != null) {
                            // Ugly hack to break out of the loop
                            j = maxTargets + i + 1;
                            //break;
                        }
                    }
                    
                    if (targetToShoot != null) {
                        listOfNPCSimple[i].ShootAtPosition(targetToShoot.transform.position);
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }

    private bool shouldChangeFiringMode()
    {
        firingModeTime += Time.deltaTime;
        if (firingModeTime > changeFiringModeAfter)
        {
            firingModeTime = 0;
            return true;
        }
        return false;
    }


    [BTLeaf("wait")]
    public BTCoroutine wait()
    {

        while (!shouldChangeFiringMode())
        {
            yield return BTNodeResult.NotFinished;
        }

        yield return BTNodeResult.Success;


    }



    [BTLeaf("pickRandomFiringMode")]
    public BTCoroutine pickRandomFiringMode()
    {
        firingHeuristic = Time.frameCount % 2 == 0 ? FiringHeuristic.FocusFire : FiringHeuristic.OneToManyFire;
        yield return BTNodeResult.Success;
    }

    [BTLeaf("targetSquadCountLow")]
    public bool targetSquadCountLow()
    {
        return engageTargetCount < 3;

    }

    [BTLeaf("targetSquadCountHigh")]
    public bool targetSquadCountHigh()
    {
        return engageTargetCount >= 3;

    }

    [BTLeaf("focusFire")]
    public BTCoroutine focusFire()
    {
        firingHeuristic = FiringHeuristic.FocusFire;
        yield return BTNodeResult.Success;
    }

    [BTLeaf("spreadOutFire")]
    public BTCoroutine spreadOutFire()
    {
        firingHeuristic = FiringHeuristic.OneToManyFire;
        yield return BTNodeResult.Success;
    }





}
