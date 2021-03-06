﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour, IFreezable, IBurnable
{
    private Vector3 startingScale;
    private bool isFrozen;
    private float thawingElapsedTime;
    private float frozenElapsedTime;
    public float frozenDuration = 5.0f;
    public float thawingDuration = 5.0f;
    private bool isThawing;
    public float inflateSpeed;
    public float deflateSpeed;
    public Spell gust;
    private Transform gustTransform;
    public float gustSpeed;
    private bool deflating;
    public float maxScale = 6;
    private Animator animator;
    public bool popping;

    // Use this for initialization
    void Start () {
        startingScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        gustTransform = transform.Find("GustExitPoint");
        isThawing = false;
        isFrozen = false;
        animator = GetComponent<Animator>();
        popping = false;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateFrozenEffects();
        if (!isFrozen && !popping && (transform.localScale.x > startingScale.x))
        {
            deflating = true;
            transform.localScale = new Vector3(transform.localScale.x * .999f, transform.localScale.y * .999f, transform.localScale.z);
        }
        else
        {
            deflating = false;
        }

        //check if Balloon is near popping or popping and update animator accordingly
        if(transform.localScale.x > (.8 * maxScale) && !popping)
        {
            animator.SetBool("NearPopping", true);
        }else if(transform.localScale.x < (.8 * maxScale) && !popping)
        {
            animator.SetBool("NearPopping", false);
        }else if(popping)
        {
            animator.SetBool("Popping", true);
            animator.SetBool("NearPopping", false);
        }

        animator.SetBool("isFrozen", isFrozen);
    }

    public void UpdateFrozenEffects()
    {
        if (isFrozen && !isThawing)
        {
            frozenElapsedTime += Time.deltaTime;
            if (frozenElapsedTime > frozenDuration)
            {
                isFrozen = false;
                frozenElapsedTime = 0;
            }
        }
        else if (isFrozen && isThawing)
        {
            thawingElapsedTime += Time.deltaTime;
            if (thawingElapsedTime > thawingDuration)
            {
                isFrozen = false;
                thawingElapsedTime = 0;
            }
        }
    }

    public void Freeze()
    {
        isFrozen = true;
        frozenElapsedTime = 0;
    }

    public void Burn()
    {
        isThawing = true;
        frozenElapsedTime = 0;
    }

    private void ShootGust()
    {
        Rigidbody2D spell = null;
        spell = Instantiate(gust.spellRigidBody, gustTransform.position, gustTransform.rotation) as Rigidbody2D;
        spell.GetComponent<SpellController>().Spell = gust;
        Vector3 spellVelocity = gustTransform.right * gustSpeed;
        spell.velocity = (transform.localScale.x > 0 ? spellVelocity : -spellVelocity) * -1;
        spell.transform.localScale = new Vector3(1, 1, 1);
    }

    public void TopTriggerShoot()
    {
        if (!isFrozen && deflating)
        {
            transform.localScale = new Vector3(transform.localScale.x * .95f, transform.localScale.y * .95f, transform.localScale.z);
            ShootGust();
        }
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    public Vector3 GetStartingScale()
    {
        return startingScale;
    }

    public void PopBalloon()
    {
        popping = false;
        transform.localScale = startingScale;
        animator.SetBool("NearPopping", false);
        animator.SetBool("Popping", false);
    }
}
