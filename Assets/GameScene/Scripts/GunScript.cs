using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    public float reloadTime;
    public float damage;
    public GameObject muzzleFlash;
    AudioSource[] audioSources;
    int ind;

    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        ind = 0;
    }

    void Update()
    {
        
        if (ind >= audioSources.Length)
        {
            ind = 0;
        }
    }

    public void playSound()
    {
        audioSources[ind].Play();
        ind++;

    }

    public void playFlash()
    {
        muzzleFlash.GetComponent<ParticleSystem>().Play();
    }
}
