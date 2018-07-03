using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime.DynamicDispatching;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{

    public enum CapturePointId
    {
        A, B, C,
        D, E, F,
        G, H, I
    }


    public CapturePointId capturePointId;

    [SerializeField]
    public TeamSide.TeamEnum belongsToTeam;

    public BoxCollider waypointDataTrigger;
    public List<TacticalWaypoint> coverpointsOnCapturePoint;
    public List<TacticalWaypoint> ambushPointsOnCapturePoint;

    public GameObject pointIndicator;

    // Team Material Color
    public Material NeutralTeamColor;
    public Material RedTeamColor;
    public Material BlueTeamColor;


    // 0 Means no one owns it
    // -Max_Capture_Value means red owns it
    // Max_Capture_Value means blue owns it
    public float captureValue = 0;
    public const float Max_Capture_Value = 100.0f;
    
    // Capture Speed Modifier to slow down capture speed
    public const float Capture_Rate_Modifier = 10.0f;

    // Point Transparency Value
    public const float Point_Transparency_Alpha = 0.2f;

    private SphereCollider captureArea;

    [SerializeField]
    public int redPlayersInside;
    [SerializeField]
    public int bluePlayersInside;


	// Use this for initialization
	void Awake ()
	{
	    redPlayersInside = 0;
	    bluePlayersInside = 0;
	    captureArea = GetComponent<SphereCollider>();
	    //waypointDataTrigger = GetComponentInChildren<BoxCollider>();
        coverpointsOnCapturePoint = new List<TacticalWaypoint>();
        ambushPointsOnCapturePoint = new List<TacticalWaypoint>();

        // -------- Setting the Initial Color of the Flag -------- 
        if (belongsToTeam == TeamSide.TeamEnum.BlueTeam) {
            changePointColor(belongsToTeam);
        }

        else if (belongsToTeam == TeamSide.TeamEnum.RedTeam) {
            changePointColor(belongsToTeam);
        }

        else {
            changePointColor(belongsToTeam);
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
        
	    // Capture Speed Multiplier 
        int captureRatio = bluePlayersInside - redPlayersInside;

        // Unit Count is even on both sides, capture is halted 
        if(captureRatio == 0 || (belongsToTeam == TeamSide.TeamEnum.RedTeam && captureRatio < 0) || (belongsToTeam == TeamSide.TeamEnum.BlueTeam && captureRatio > 0)) {
            return;
        }
        
        // Calculate the new capture point score
        captureValue += (captureRatio * Time.deltaTime * Capture_Rate_Modifier);

        
        // Red has captured the point
        if (captureValue <= -Max_Capture_Value) {
            belongsToTeam = TeamSide.TeamEnum.RedTeam;
            changePointColor(belongsToTeam);
        }

        // Blue has captured the point
        else if (captureValue >= Max_Capture_Value) {
            belongsToTeam = TeamSide.TeamEnum.BlueTeam;
            changePointColor(belongsToTeam);

        }

        // Once the caputure has passed by the value of 0, make the point neutral before capturing it
        else if((belongsToTeam == TeamSide.TeamEnum.RedTeam && captureValue > 0) || (belongsToTeam == TeamSide.TeamEnum.BlueTeam && captureValue < 0)) {
            belongsToTeam = TeamSide.TeamEnum.None;
            changePointColor(belongsToTeam);

        }

        //Debug.Log("Capture Rate: " + captureValue);

    }


    void OnTriggerEnter(Collider other)
    {

        TeamSide teamSide = other.GetComponent<TeamSide>();
        NpcSimple npc = other.GetComponent<NpcSimple>();
        
        if (npc != null)
        {
            npc.IsAtCapturePoint = true;
        }

        if (teamSide != null)
            

                switch (teamSide.playerTeam)
                {
                case TeamSide.TeamEnum.None:
                    {
                        // Debug.Log("None is capturing");
                        break;
                    }
                        
                case TeamSide.TeamEnum.RedTeam:
                    {
                        ++redPlayersInside;

                        //Debug.Log(redPlayersInside + ": Red is capturing");
                        
                        break;
                    }
                case TeamSide.TeamEnum.BlueTeam:
                    {
                        ++bluePlayersInside;
                        //Debug.Log(bluePlayersInside + ": Blue is capturing");
                        break;
                    }

                }
        
        
    }

    void OnTriggerExit(Collider other)
    {
        TeamSide teamSide = other.GetComponent<TeamSide>();
        NpcSimple npc = other.GetComponent<NpcSimple>();
        if (npc != null)
        {
            npc.IsAtCapturePoint = false;
        }

        if (teamSide != null)


            switch (teamSide.playerTeam)
            {
                case TeamSide.TeamEnum.None:
                    {
                        //Debug.Log("None is capturing");
                        break;
                    }

                case TeamSide.TeamEnum.RedTeam:
                    {
                        --redPlayersInside;
                        //Debug.Log(redPlayersInside  + ":Red capturing, red left");
                        break;
                    }
                case TeamSide.TeamEnum.BlueTeam:
                    {
                        --bluePlayersInside;
                        //Debug.Log(bluePlayersInside + ":blue capturing, blue left");
                        break;
                    }

            }
    }


    // Makes the circle of the capture point transparent 
    public void applyPointTransparency(float alphaValue) {
        var color = pointIndicator.GetComponent<MeshRenderer>().material.color;
        color.a = alphaValue;

        pointIndicator.GetComponent<MeshRenderer>().material.color = color;
    }


    // Change the Color of the point
    public void changePointColor(TeamSide.TeamEnum side) {

        // Change Point to Red
        if (side == TeamSide.TeamEnum.RedTeam) {
            this.GetComponent<MeshRenderer>().material = RedTeamColor;
            pointIndicator.GetComponent<MeshRenderer>().material = RedTeamColor;
            applyPointTransparency(Point_Transparency_Alpha);
        }

        // Change Point to Blue
        else if(side == TeamSide.TeamEnum.BlueTeam) {
            this.GetComponent<MeshRenderer>().material = BlueTeamColor;
            pointIndicator.GetComponent<MeshRenderer>().material = BlueTeamColor;
            applyPointTransparency(Point_Transparency_Alpha);
        }

        // Change Point to Neutral
        else if(side == TeamSide.TeamEnum.None) {
            this.GetComponent<MeshRenderer>().material = NeutralTeamColor;
            pointIndicator.GetComponent<MeshRenderer>().material = NeutralTeamColor;
            applyPointTransparency(Point_Transparency_Alpha);
        }
    }

}
