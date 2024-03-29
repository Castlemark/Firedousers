﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource muscicSource;
    public static SoundManager instance = null;

    public float lowPitchRange;
    public float highPitchRange; 

	// Use this for initialization
	void Awake()
    {
        if(instance == null){
            instance = this;

        }else if(instance != this)
        {
            Destroy(gameObject);
        }
        //gameObject.tag = "ToBeDestroyed";

        DontDestroyOnLoad(gameObject);
 
		
	}

    //Reprodueix un clip d'audio
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }


    //escollim un pitch i un clip random dels que passem per parametres Sepratats per "," i el reproduim
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];

        efxSource.PlayOneShot(efxSource.clip);
    }

}
