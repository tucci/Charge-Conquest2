using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSide : MonoBehaviour {

    public enum TeamEnum
    {
        None,
        RedTeam,//14
        BlueTeam,//15

    }

    public TeamEnum playerTeam;


    [SerializeField]
    private MeshRenderer indicatorMesh;

    [SerializeField]
    private Material noTeamMaterial;
    [SerializeField]
    private Material redTeamMaterial;
    [SerializeField]
    private Material blueTeamMaterial;

    // Use this for initialization
    void Start () {
        SetTeam(playerTeam);

        int teamLayer = 0;
        if ((int)this.gameObject.GetComponent<TeamSide>().playerTeam == 2)
        {
            teamLayer =  15;
        }
        else if ((int)this.gameObject.GetComponent<TeamSide>().playerTeam == 1)
        {
            teamLayer =  14;
        }

        this.gameObject.layer = teamLayer;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTeam(TeamEnum side)
    {
        if (indicatorMesh == null)
            return;
        switch (side)
        {
            case TeamEnum.None:
                indicatorMesh.material = noTeamMaterial;
                break;
            case TeamEnum.RedTeam:
                indicatorMesh.material = redTeamMaterial;
                break;                
            case TeamEnum.BlueTeam:
                indicatorMesh.material = blueTeamMaterial;
                break;
            default:
                indicatorMesh.material = noTeamMaterial;
                break;
        }
    }
}
