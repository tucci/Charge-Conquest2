using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tankScript : MonoBehaviour {
    bool playerSeen;
    public GameObject muzzleFlash;
    float reloadTime;
    public float timerBetweenShots;
    AudioSource[] audioSources;
    int ind;
    int tankPathIndex;
    public float damage;
    public GameObject[] path;
    public GameObject[] pathNone;
    public GameObject[] pathOne;
    public GameObject[] pathTwo;
    public GameObject[] pathThree;
    public GameObject[] capturePoints;
    public GameObject[] currentPath;
    [SerializeField] private float tankSpeed;
    [SerializeField] private float tankRotation;
    // Use this for initialization
    void Start () {
        playerSeen = false;
        path = pathNone;
        reloadTime = 0.0f;
        audioSources = GetComponents<AudioSource>();
        ind = 0;
        tankPathIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (capturePoints[0].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam &&
            capturePoints[1].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam &&
            capturePoints[2].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam && (transform.position == pathNone[0].transform.position))
        {
            path = pathThree;
        }
        else if (capturePoints[0].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam &&
            capturePoints[1].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam &&
            capturePoints[2].GetComponent<CapturePoint>().belongsToTeam != TeamSide.TeamEnum.RedTeam && (transform.position == pathNone[0].transform.position))
        {
            path = pathTwo;
        }
        else if (capturePoints[0].GetComponent<CapturePoint>().belongsToTeam == TeamSide.TeamEnum.RedTeam &&
            capturePoints[1].GetComponent<CapturePoint>().belongsToTeam != TeamSide.TeamEnum.RedTeam && (transform.position == pathNone[0].transform.position))
        {
            path = pathOne;
        }
        else if (capturePoints[0].GetComponent<CapturePoint>().belongsToTeam != TeamSide.TeamEnum.RedTeam && (transform.position == pathNone[0].transform.position))
        { 
            path = pathNone;
        }


        reloadTime += Time.deltaTime;
        if(ind >= audioSources.Length)
        {
            ind = 0;
        }

        Vector3 dir2 = GameObject.FindGameObjectWithTag("Player").transform.position - this.transform.position;
        RaycastHit hit2;
        if (Physics.Raycast(this.transform.position, dir2, out hit2))
        {
            if (hit2.transform.gameObject.CompareTag("Player"))
            {
                playerSeen = true;
                Quaternion neededRotation2 = Quaternion.LookRotation(hit2.transform.position - gameObject.transform.position);
                neededRotation2.x = 0.0f;
                neededRotation2.z = 0.0f;
               
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, neededRotation2, Time.deltaTime * tankRotation);

            }
            else
            {
                playerSeen = false;
            }
        }

        Vector3 dir = this.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, dir, out hit, 20.0f))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                
                if (reloadTime >= timerBetweenShots)
                {
                    muzzleFlash.GetComponent<ParticleSystem>().Play();

                    hit.transform.gameObject.GetComponent<HealthScript>().takeDamage(damage);
                    audioSources[ind].Play();
                    ind++;
                    reloadTime = 0.0f;
                }
            }
        }


        if (!playerSeen)
        {
            Quaternion neededRotation = Quaternion.LookRotation(path[tankPathIndex].transform.position - gameObject.transform.position);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, neededRotation, Time.deltaTime * tankRotation);



            if ((Quaternion.Angle(transform.rotation, neededRotation)) <= 5.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, path[tankPathIndex].transform.position, Time.deltaTime * tankSpeed);
            }


            if (path[tankPathIndex].transform.position == transform.position)
            {
                if (tankPathIndex >= (path.Length - 1))
                {
                    System.Array.Reverse(path);
                    tankPathIndex = 0;
                }
                else
                {
                    tankPathIndex++;
                }
            }
        }

    }
}
