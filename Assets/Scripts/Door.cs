using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite spriteOpenedDoor;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void openDoor()
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        spriteRenderer.sprite = spriteOpenedDoor;
        gameObject.layer = 2;
    }
}
