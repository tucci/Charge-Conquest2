using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{

    public bool isNpc;

    public Camera PlayerCamera;
    // The camera offsets and rotation from the player
    // This allows us to have a top down view
    public Vector3 CameraOffsetPostion;
    public Vector3 CameraEulerRotation;
    public float characterMovement;
    public float diagonalChracterMovement;
    public GameObject[] guns;
    public GameObject currentGun;
    public bool bulletReady;
    public float bulletTimer;
    public GameObject aimMarker;
    public Material aimGreen;
    public Material aimRed;
    Vector2 hotSpot = Vector2.zero;

    private TeamSide.TeamEnum myTeam;


    
    

    
    
    // We need this to prevent the player from being stuck in the floor
    public float PlayerHalfHeight = 1;

    // Use this for initialization
    void Start () {
        bulletReady = true;
        bulletTimer = 2.0f;
        SwitchToWeapon(0);
        myTeam = GetComponent<TeamSide>().playerTeam;
        
        //cursor invisible for aesthetics comment out for testing - Jason
        //Cursor.visible = false;
    }


    private void OnCollisionExit(Collision collision)
    {
        // TODO: see if we still need this. I just tested with it commented out, and everything seems to still work fine
        //this is because when you collide with something you receive a small velocity that sends you in a specific direction (temporary fix) - Jason
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    // Update is called once per frame
    void Update () {
        if (isNpc)
        {
            DoNPCUpdate();
        }
        else
        {
            DoHumanUpdate();
        }

        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);

    }

    // Interface methods
    public void TryShootEnemy(GameObject enemy)
    {
        if (bulletReady)
        {
            bulletTimer = 0.0f;
            PlayGunSound();
            enemy.GetComponent<HealthScript>().takeDamage(currentGun.GetComponent<GunScript>().damage);
            
        }
    }

    public void TryShootEnemy(NpcSimple enemy)
    {
        if (bulletReady)
        {
            bulletTimer = 0.0f;
            PlayGunSound();
            enemy.GetComponent<HealthScript>().takeDamage(currentGun.GetComponent<GunScript>().damage);

        }
    }

    public void TryShootTarget(Vector3 position)
    {
        if (bulletReady)
        {
            bulletTimer = 0.0f;
            PlayGunSound();

            Vector3 dir3 = position - transform.position;
            RaycastHit hit3;
            
            if (Physics.SphereCast(this.transform.position, 0.75f, dir3, out hit3, Mathf.Infinity))
            {
                if (hit3.transform.gameObject.CompareTag("NPC"))
                {
                    if (hit3.transform.gameObject.GetComponent<TeamSide>().playerTeam != myTeam)
                    {       
                        hit3.transform.gameObject.GetComponent<HealthScript>().takeDamage(currentGun.GetComponent<GunScript>().damage);
                    }
                    
                }

                if (hit3.transform.gameObject.CompareTag("Player"))
                {
                    if (hit3.transform.gameObject.GetComponent<TeamSide>().playerTeam != myTeam)
                    {
                        hit3.transform.gameObject.GetComponent<HealthScript>().takeDamage(currentGun.GetComponent<GunScript>().damage);
                    }
                }
            }
        }   
    }
    


    public void SwitchToWeapon(int slot)
    {

        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }
        guns[slot].SetActive(true);
        currentGun = guns[slot];
    }


    // Internal methods

    private void PlayGunSound()
    {
        currentGun.GetComponent<GunScript>().playSound();
        currentGun.GetComponent<GunScript>().playFlash();
    }

    private void DoNPCUpdate()
    {
        bulletTimer += Time.deltaTime;
        if (bulletTimer >= currentGun.GetComponent<GunScript>().reloadTime)
        {
            bulletReady = true;
        }
        if (bulletTimer < currentGun.GetComponent<GunScript>().reloadTime)
        {
            bulletReady = false;
        }
    }

    private void DoHumanUpdate()
    {
    
        Vector3 dir3 = this.transform.forward;
        RaycastHit hit3;
        if (Physics.SphereCast(this.transform.position, 0.75f, dir3, out hit3, Mathf.Infinity))
        {
            //print("hit something: " + hit3.transform.position);
            if (hit3.transform.gameObject.CompareTag("NPC"))
            {
                if (hit3.transform.gameObject.GetComponent<TeamSide>().playerTeam != myTeam)
                {
                    aimMarker.gameObject.GetComponent<Renderer>().material = aimRed;
                    if (Input.GetMouseButton(0))
                    {
                        if (currentGun != null && Time.timeScale != 0)
                        {
                            TryShootEnemy(hit3.transform.gameObject);
                        }
                    }
                }

                
            }
            else
            {
                aimMarker.gameObject.GetComponent<Renderer>().material = aimGreen;
            }
        }
        else
        {
            aimMarker.gameObject.GetComponent<Renderer>().material = aimGreen;
        }

        bulletTimer += Time.deltaTime;
        if (bulletTimer >= currentGun.GetComponent<GunScript>().reloadTime)
        {
            bulletReady = true;
        }
        if (bulletTimer < currentGun.GetComponent<GunScript>().reloadTime)
        {
            bulletReady = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (currentGun != null)
            {
                if (bulletReady && Time.timeScale != 0)
                {
                    bulletTimer = 0.0f;
                    PlayGunSound();
                }
            }
        }

        //This block handles movement input such as moving in diagonals as well as in straight lines WASD 
        //is used for movement - Jason
        if (Input.GetKey("w"))
        {
            if (Input.GetKey("a") || Input.GetKey("d"))
            {
                this.transform.position += new Vector3(Time.deltaTime * diagonalChracterMovement, 0.0f, 0.0f);
            }
            else
            {
                this.transform.position += new Vector3(Time.deltaTime * characterMovement, 0.0f, 0.0f);
            }
        }
        if (Input.GetKey("s"))
        {
            if (Input.GetKey("a") || Input.GetKey("d"))
            {
                this.transform.position += new Vector3(-Time.deltaTime * diagonalChracterMovement, 0.0f, 0.0f);
            }
            else
            {
                this.transform.position += new Vector3(-Time.deltaTime * characterMovement, 0.0f, 0.0f);
            }
        }
        if (Input.GetKey("d"))
        {
            if (Input.GetKey("w") || Input.GetKey("s"))
            {
                this.transform.position += new Vector3(0.0f, 0.0f, -Time.deltaTime * diagonalChracterMovement);
            }
            else
            {
                this.transform.position += new Vector3(0.0f, 0.0f, -Time.deltaTime * characterMovement);
            }
        }
        if (Input.GetKey("a"))
        {
            if (Input.GetKey("w") || Input.GetKey("s"))
            {
                this.transform.position += new Vector3(0.0f, 0.0f, Time.deltaTime * diagonalChracterMovement);
            }
            else
            {
                this.transform.position += new Vector3(0.0f, 0.0f, Time.deltaTime * characterMovement);
            }
        }

        //This block handles switching between weapons 1, 2 & 3 to switch between
        //the three kinds of guns Rifle, Shotgun and Machine Gun respectively - Jason
        if (Input.GetKey("1"))
        {
            SwitchToWeapon(0);
        }
        if (Input.GetKey("2"))
        {
            SwitchToWeapon(1);
        }
        if (Input.GetKey("3"))
        {
            SwitchToWeapon(2);
        }


        //Block the handles camera angle and movement (camera between player and mouse position) - Jason
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            transform.LookAt(targetPoint);

            Vector3 camPointTemp = (transform.position + targetPoint) / 2.0f;
            Vector3 camPoint = (transform.position + camPointTemp) / 2.0f;

            PlayerCamera.transform.position = camPoint + CameraOffsetPostion;
            PlayerCamera.transform.eulerAngles = CameraEulerRotation;
        }
    }



}
