using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{

    public SkinnedMeshRenderer skin;

    private TeamSide side;

    //healthText is textMesh above enemy displaying current health - Jason
    public GameObject healthText;
    public float health;
    //damageTimer is what causes enemy to flash red for half a second upon taking damage - Jason
    public float damageTimer;
    

	// Use this for initialization
	void Start ()
	{
	    side = GetComponent<TeamSide>();
        healthText.GetComponent<TextMesh>().text = health.ToString();
        damageTimer = 1.0f;
	    switch (side.playerTeam)
	    {
	        case TeamSide.TeamEnum.None:
                healthText.GetComponent<TextMesh>().color = Color.white;
                break;
	        case TeamSide.TeamEnum.RedTeam:
                healthText.GetComponent<TextMesh>().color = Color.red;
                break;
	        case TeamSide.TeamEnum.BlueTeam:
                healthText.GetComponent<TextMesh>().color = Color.blue;
                break;
	        default:
                healthText.GetComponent<TextMesh>().color = Color.white;
                break;
	            
	    }

        
	}
	
	// Update is called once per frame
	void Update () {
        //Displays Enemies current health on a text mesh above enemy and then causes it to face camera - Jason
        healthText.GetComponent<TextMesh>().text = Mathf.Round(health).ToString();
        healthText.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        //This block causes the enemy to flash red upon taking damage - Jason
        damageTimer += Time.deltaTime;
        if(skin != null)
        {
            if (damageTimer <= 0.5f)
            {
                skin.material.color = Color.red;
            }
            else if (damageTimer >= 0.5f)
            {
                skin.material.color = Color.white;
            }
        }
        
    }

    //This function causes the chracter to take the specified damage 
    //and die if health falls below 0 - Jason
    public void takeDamage(float healthToLose)
    {
        damageTimer = 0.0f;
        health -= healthToLose;
        if(health <= 0.0f)
        {
            killUnit();
        }
    }
    
    //Kills this Unit - Jason
    void killUnit()
    {
        transform.position = new Vector3(-10000, -1000, -10000);
        skin.enabled = false;
        this.gameObject.layer = 16;
        //gameObject.GetComponent<NpcSimple>().enabled = false;
        //transform.GetComponent<MeshRenderer>().enabled = false;
        //transform.GetComponent<Rigidbody>().useGravity = false;
        //transform.GetComponent<Rigidbody>()
        //transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * 10000, ForceMode.Force);
        
        Destroy(this.gameObject, 0.2f);
    }
}
