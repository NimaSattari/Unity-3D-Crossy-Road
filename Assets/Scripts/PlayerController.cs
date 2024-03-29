﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1;
    public float moveTime = 0.4f;
    public float colliderDistCheck = 1;
    public bool isIdle = true;
    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;
    public bool jumpStart = false;
    public ParticleSystem particle = null;
    public GameObject chick = null;
    private Renderer renderer = null;
    private bool isVisible = false;
    public AudioClip audioIdle1;
    public AudioClip audioIdle2;
    public AudioClip audioHop;
    public AudioClip audioHit;
    public AudioClip audioSplash;
    public ParticleSystem splash;
    public bool parentedToObject = false;

    private void Start()
    {
        renderer = chick.GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!Manager.insance.CanPlay()) return;

        if (isDead) return;

        CanIdle();
        CanMove();
        IsVisible();
    }

    void CanIdle()
    {
        if (isIdle)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) { CheckIfIdle(270, 0, 0); }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { CheckIfIdle(270, 180, 0); }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { CheckIfIdle(270, -90, 0); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { CheckIfIdle(270, 90, 0); }
        }
    }
    void CheckIfIdle(float x,float y,float z)
    {
        chick.transform.rotation = Quaternion.Euler(x, y, z);
        CheckIfCanMove();
        int a = Random.Range(0, 12);
        if (a < 5) PlayAudioClip(audioIdle1);
    }

    void CheckIfCanMove()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, -chick.transform.up, out hit, colliderDistCheck);
        Debug.DrawRay(this.transform.position, -chick.transform.up * colliderDistCheck, Color.red, 2);
        if(hit.collider == null)
        {
            SetMove();
        }
        else
        {
            if(hit.collider.tag == "collider")
            {
                Debug.Log("Hit something with collider tag.");
                isIdle = true;
            }
            else
            {
                SetMove();
            }
        }
    }

    void SetMove()
    {
        Debug.Log("Hit noting. Keep moving");

        isIdle = false;
        isMoving = true;
        jumpStart = true;
    }

    void CanMove()
    {
        if (isMoving)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                Moving(new Vector3(transform.position.x, transform.position.y, transform.position.z + moveDistance)); SetMoveForwardState();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                Moving(new Vector3(transform.position.x, transform.position.y, transform.position.z - moveDistance)); 
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                Moving(new Vector3(transform.position.x - moveDistance, transform.position.y, transform.position.z));
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                Moving(new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z));
            }
        }
    }

    void Moving(Vector3 pos)
    {
        isIdle = false;
        isMoving = false;
        isJumping = true;
        jumpStart = false;
        PlayAudioClip(audioHop);
        LeanTween.move(this.gameObject, pos, moveTime).setOnComplete(MoveComplete);
    }

    void MoveComplete()
    {
        isJumping = false;
        isIdle = true;
        int a = Random.Range(0, 12);
        if (a < 5) PlayAudioClip(audioIdle2);
    }

    void SetMoveForwardState()
    {
        Manager.insance.UpdateDistanceCount();
    }

    void IsVisible()
    {
        if(renderer.isVisible)
        {
            isVisible = true;
        }
        if(!renderer.isVisible && isVisible)
        {
            Debug.Log("Player off screen. Apply GotHit()");

            GotHit();
        }
    }

    public void GotHit()
    {
        isDead = true;
        ParticleSystem.EmissionModule em = particle.emission;
        em.enabled = true;
        PlayAudioClip(audioHit);
        Manager.insance.GameOver();
    }
    public void GotSoaked()
    {
        isDead = true;
        ParticleSystem.EmissionModule em = splash.emission;
        em.enabled = true;
        PlayAudioClip(audioSplash);
        chick.SetActive(false);
        Manager.insance.GameOver();
    }

    void PlayAudioClip(AudioClip clip)
    {
        this.GetComponent<AudioSource>().PlayOneShot(clip);
    }
}
