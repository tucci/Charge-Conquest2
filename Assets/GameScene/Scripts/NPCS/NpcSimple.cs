using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;
using Random = UnityEngine.Random;

public class NpcSimple : MonoBehaviour
{

    [SerializeField]
    private WaypointGraph waypointGraph;

    [SerializeField]
    private CapturePointMap capturePointMap;

    [SerializeField]
    public LaneScript.LaneSide assignedLane;


    // TODO: do we need both of these
    [SerializeField] public Transform targetTransform;
    [SerializeField] public SphereCollider targetArea;
    [SerializeField] private Vector3 randomTargetTransformInArea;
    public bool pauseMovementToTarget = false;

    [SerializeField] private float moveToNextRandomPointInAreaAfterTime = 2.0f;
    [SerializeField] private float pointMoveTime = 0;
    [SerializeField] private bool _stayStillOnPoint = true;

    [SerializeField]
    private float gunChangeTime = 0;
    [SerializeField]
    private float gunChangeTimeAfter = 5;



    [SerializeField] private Transform runTargetTransform;
    


    [SerializeField]
    private float npcSpeed;

    private NavMeshAgent agent;
    private SoliderAnimScript anim;
    private CharacterScript character;
    private TeamSide team;
    private HealthScript health;


    // Behaviors
    private BehaviorTree bt;
    public int timeInLeaf;

    // TODO instead of a bunch of bools, see if we can use a bit field
    public Dictionary<TacticalWaypoint.WaypointType, bool> inTacticalArea;
    public bool IsAtCapturePoint;



    // List of all AnchorPoints around the character
    public List<GameObject> listOfAnchorPoints;
    public bool isNPCLeader;
    public bool isPlayerGrouped;

    //// TODO: remove when done testing
    //void Start()
    //{
    //    isNPCLeader = false;

    //    // targetTransform = null;

    //    // SetUpTactialAreas();
    //    // SetUpAgentAndAnim();

    //    // GoToNearestCoverAround();

    //    //targetTransform = null;

    //    SetUpTactialAreas();
    //    SetUpAgentAndAnim();

    //    //GoToNearestAmbushAround();
    //    //GoToNearestCoverAround();
    //    //GoToCapturePoint(CapturePoint.CapturePointId.H, TacticalWaypoint.WaypointType.AmbushPoint, true);
    //}

    // Use this for initialization
    void Awake ()
	{

        isNPCLeader     = false;
        isPlayerGrouped = false;

	    waypointGraph = GameObject.FindGameObjectWithTag("WaypointGraph").GetComponent<WaypointGraph>();
	    capturePointMap = GameObject.FindGameObjectWithTag("CapturePointMap").GetComponent<CapturePointMap>();

        // targetTransform = null;

        // SetUpTactialAreas();
        // SetUpAgentAndAnim();

        // GoToNearestCoverAround();

        //targetTransform = null;

        SetUpTactialAreas();
        SetUpAgentAndAnim();

        //GoToNearestAmbushAround();
        //GoToNearestCoverAround();
        // GoToCapturePoint(CapturePoint.CapturePointId.A, TacticalWaypoint.WaypointType.AmbushPoint, true);



        // Start behavior tree
        bt = new BehaviorTree(Application.dataPath + "/GameScene/Scripts/npc_firing.xml", this);
        bt.Start();
    }

    // Update is called once per frame
    void Update() {

        if (targetTransform != null)
        {

            if (IsAtCapturePoint)
            {
                agent.speed = 3;
                GoToTargetArea();
                return;
            }
            
            if (pauseMovementToTarget)
            {
                agent.speed = 1;
                GoToTargetArea();
                return;
            }
            

            agent.speed = 7;

            GoToTargetArea();

        }

    }


    public void PauseMovement(bool pause)
    {
        pauseMovementToTarget = pause;
    }


    public void GoToTarget(Transform target)
    {
        targetTransform = target;
    }

    public void GoToTarget()
    {
        agent.SetDestination(targetTransform.transform.position);
    }

    public void GoToTargetArea()
    {

        // If we don't have an area, just go the target
        if (targetArea == null)
        {
            GoToTarget();
            return;
        }


        // Only recalculate when we are in the area
        if (targetArea.bounds.Contains(transform.position) && !_stayStillOnPoint)
        {
            pointMoveTime += Time.deltaTime;

            if (pointMoveTime > moveToNextRandomPointInAreaAfterTime)
            {
                pointMoveTime = 0;
                CalcRandomPointInArea();
            }
            agent.SetDestination(randomTargetTransformInArea);
            return;
        }

        GoToTarget();
            
    }


    private void CalcRandomPointInArea()
    {
        Vector3 randomDir = targetArea.center - (targetArea.center + Random.insideUnitSphere);
        randomDir = randomDir.normalized * Random.Range(1, targetArea.bounds.extents.x - targetArea.bounds.extents.x / 3);
        randomTargetTransformInArea = targetArea.transform.position + randomDir;
    }





    private void SetUpAgentAndAnim()
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.speed = npcSpeed;
        anim = GetComponent<SoliderAnimScript>();
        anim.setNpc(agent);
        character = GetComponent<CharacterScript>();
        team = GetComponent<TeamSide>();
        health = GetComponent<HealthScript>();

        if (!agent)
        {
            Debug.Log("No NavMeshAgent attached to this npc");
        }
    }


    private void SetUpTactialAreas()
    {
        inTacticalArea = new Dictionary<TacticalWaypoint.WaypointType, bool>();
        inTacticalArea.Add(TacticalWaypoint.WaypointType.CoverPoint, false);
        inTacticalArea.Add(TacticalWaypoint.WaypointType.AmbushPoint, false);
    }
    


    // List of behaviours
    public void GoToNearestCoverAround(Vector3 pos)
    {
        GameObject waypoint = waypointGraph.FindNearestCoverPoint(pos, pos, assignedLane, team.playerTeam);
        targetArea = waypoint.GetComponent<SphereCollider>();
        GoToTarget(waypoint.transform);
    }

    public bool IsInCover()
    {
        return inTacticalArea[TacticalWaypoint.WaypointType.CoverPoint];
    }


    public void GoToNearestAmbushAround(Vector3 pos)
    {
        GameObject waypoint = waypointGraph.FindNearestAmbushPoint(pos, pos, assignedLane, team.playerTeam);
        targetArea = waypoint.GetComponent<SphereCollider>();
        GoToTarget(waypoint.transform);

    }

    public bool IsAtAmbushPoint()
    {
        return inTacticalArea[TacticalWaypoint.WaypointType.AmbushPoint];
    }



    public void GoToCapturePoint(CapturePoint.CapturePointId captureId, TacticalWaypoint.WaypointType findWaypointType, bool preferTeamCover=true)
    {
        CapturePoint point = capturePointMap.capturePoints[captureId];
        

        targetArea = point.GetComponent<SphereCollider>();

        List<TacticalWaypoint> waypointTypeOnCapturePoint = null;
        switch (findWaypointType)
        {
            case TacticalWaypoint.WaypointType.NonePoint:
                waypointTypeOnCapturePoint = null;
                break;
            case TacticalWaypoint.WaypointType.CoverPoint:
                waypointTypeOnCapturePoint = point.coverpointsOnCapturePoint;
                break;
            case TacticalWaypoint.WaypointType.AmbushPoint:
                waypointTypeOnCapturePoint = point.ambushPointsOnCapturePoint;
                break;
            default:
                waypointTypeOnCapturePoint = null;
                break;
        }

        // If we want to find a cover point, and the capture point has one
        if (waypointTypeOnCapturePoint != null && waypointTypeOnCapturePoint.Count != 0)
        {

            // If we want cover that is closer to the players team side
            if (preferTeamCover)
            {
                // Create a local list of cover points that belong to a team
                List<TacticalWaypoint> teamWaypointTypes = new List<TacticalWaypoint>();

                foreach (var waypointType in waypointTypeOnCapturePoint)
                {
                    if (waypointType.closerToWhichTeamSide == team.playerTeam)
                    {
                        teamWaypointTypes.Add(waypointType);
                    }
                }

                // If we have some team cover points to go to, then pick a random one
                if (teamWaypointTypes.Count != 0)
                {
                    //targetTransform = teamWaypointTypes[Random.Range(0, teamWaypointTypes.Count)].transform;
                    TacticalWaypoint tw = teamWaypointTypes[Random.Range(0, teamWaypointTypes.Count)];
                    targetTransform = tw.transform;
                    targetArea = tw.GetComponent<SphereCollider>();
                }
                else
                {
                    // If there are no team cover points found, just pick a random one
                    //targetTransform = waypointTypeOnCapturePoint[Random.Range(0, waypointTypeOnCapturePoint.Count)].transform;
                    TacticalWaypoint tw = waypointTypeOnCapturePoint[Random.Range(0, waypointTypeOnCapturePoint.Count)];
                    targetTransform = tw.transform;
                    targetArea = tw.GetComponent<SphereCollider>();
                }
            }
            else
            {
                // For now go to a random cover point
                // targetTransform = waypointTypeOnCapturePoint[Random.Range(0, waypointTypeOnCapturePoint.Count)].transform;
                TacticalWaypoint tw = waypointTypeOnCapturePoint[Random.Range(0, waypointTypeOnCapturePoint.Count)];
                targetTransform = tw.transform;
                targetArea = tw.GetComponent<SphereCollider>();
            }
        }
        else
        {
            // If we dont want to find either a cover point, or an ambush type, then just go to the capture point
            //targetTransform = capturePointMap.capturePoints[captureId].transform;

            CapturePoint tw = capturePointMap.capturePoints[captureId];
            targetTransform = tw.transform;
            targetArea = tw.GetComponent<SphereCollider>();


        }


    }

    public bool HasArrivedAtCapturePoint(CapturePoint.CapturePointId capturePointId)
    {
        return IsAtCapturePoint;
    }

    


    


    public void ShootEnemy(GameObject enemy)
    {
        character.TryShootEnemy(enemy);
    }

    public void ShootEnemy(NpcSimple enemy)
    {
        character.TryShootEnemy(enemy);
    }

    public void ShootTarget()
    {
        character.TryShootTarget(targetTransform.transform.position);
    }

    private void FacePosition(Vector3 pos)
    {
        Vector3 targetDir = pos - transform.position;
        // The step size is equal to speed times frame time.
        float step = 5 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    public void ShootAtPosition(Vector3 pos)
    {
        FacePosition(pos);
        character.TryShootTarget(pos);
    }

    public void StayStillOnPoint(bool stayStill)
    {
        _stayStillOnPoint = stayStill;
    }



    [BTLeaf("waitForGunChange")]
    public BTCoroutine waitForGunChange()
    {


        while (!shouldChangeWeapon())
        {
            yield return BTNodeResult.NotFinished;
        }

        yield return BTNodeResult.Success;
        
    
    }

    private bool shouldChangeWeapon()
    {
        gunChangeTime += Time.deltaTime;
        if (gunChangeTime > gunChangeTimeAfter)
        {
            gunChangeTime = 0;
            return true;
        }
        return false;
    }


    [BTLeaf("lowHealth")]
    public bool lowHealth()
    {
        return health.health < 25;
        
    }

    [BTLeaf("highHealth")]
    public bool enemyHealthHigh()
    {
        return health.health >= 25;
    }

    [BTLeaf("lowVelocity")]
    public bool lowVelocity()
    {
        return agent.velocity.magnitude < 3.0f;
        
    }

    [BTLeaf("highVelocity")]
    public bool highVelocity()
    {
        return agent.velocity.magnitude >= 3.0f;
    }


    [BTLeaf("pickRandomGun")]
    public BTCoroutine pickRandomGun()
    {
        character.SwitchToWeapon(Random.Range(0, 3));
        yield return BTNodeResult.Success;
    }


    [BTLeaf("rifle")]
    public BTCoroutine rifle()
    {
        character.SwitchToWeapon(0);
        yield return BTNodeResult.Success;
    }


    [BTLeaf("shotgun")]
    public BTCoroutine shotgun()
    {
        character.SwitchToWeapon(1);
        yield return BTNodeResult.Success;
    }

    [BTLeaf("ak")]
    public BTCoroutine ak()
    {
        character.SwitchToWeapon(2);
        yield return BTNodeResult.Success;
    }








}
