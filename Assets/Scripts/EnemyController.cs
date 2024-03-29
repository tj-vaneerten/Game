﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IFreezable, IBurnable, ICloneable, IGustable {

    public Transform flipcheck;
    public Transform groundCheck;
    public LayerMask edgeLayerMask;
    public float speed;
    public float horizontal;
    public bool isCloneable;

    private bool isFrozen;
    private bool isCloned;
    private Animator animator;
    private Rigidbody2D rigidBody;
    public float frozenElapsedTime;
    private bool isThawing;
    private float thawingElapsedTime;
    public float frozenDuration = 5.0f;
    public float thawingDuration = 2.0f;
    private Vector3 startingPosition;
    private bool inAir;
    private bool atEdge;
    private List<int> layersToIgnore;
    private bool isStuck;
    public bool burning;
    private bool gusted;
    public float frozenGustSpeed;

    private Vector3 startingPointPosition;
    private Vector3 endingPointPosition;

    public PhysicsMaterial2D frozenMaterial;



    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        startingPosition = rigidBody.transform.position;
        layersToIgnore = new List<int>
        {
            LayerMask.NameToLayer("Ladder"),
            LayerMask.NameToLayer("LadderTop"),
            LayerMask.NameToLayer("LevelEnd"),
            LayerMask.NameToLayer("Spell"),
            LayerMask.NameToLayer("Switch"),
            LayerMask.NameToLayer("DialogueTrigger")
        };
        //if its a limited pacing enemy need to find the starting and ending points for pacing
    }
	
	// Update is called once per frame
	void Update () {
        atEdge = !Physics2D.OverlapPoint(flipcheck.position, edgeLayerMask, 0);
        inAir = !Physics2D.OverlapPoint(groundCheck.position, edgeLayerMask, 0);
        //hasVelocity = (rigidBody.velocity.x == 0) || (rigidBody.velocity.y != 0);
        if(rigidBody.velocity.x == 0 && gusted)
        {
            gusted = false;
        }
        if (!isFrozen && !isCloned && !inAir && !gusted && !burning)
        {
            if (!atEdge)
            {
                transform.Translate(speed * horizontal * Time.deltaTime, 0f, 0f);
            }
            else
            {
                Flip();
            }
        }else if (isFrozen)
        {
            if (!isThawing)
            {
                frozenElapsedTime += Time.deltaTime;
                if (frozenElapsedTime > frozenDuration)
                {
                    isFrozen = false;
                    GetComponent<CapsuleCollider2D>().sharedMaterial = null;
                    frozenElapsedTime = 0;
                    thawingElapsedTime = 0;
                }
            }
            else if (isThawing)
            {
                thawingElapsedTime += Time.deltaTime;
                if (thawingElapsedTime > thawingDuration)
                {
                    isFrozen = false;
                    isThawing = false;
                    GetComponent<CapsuleCollider2D>().sharedMaterial = null;
                    thawingElapsedTime = 0;
                    frozenElapsedTime = 0;
                }
            }
        }

        animator.SetBool("IsFrozen", isFrozen);
        animator.SetBool("IsCloned", isCloned);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!inAir && !gusted && !isFrozen)
        {          
            if (layersToIgnore.IndexOf(col.gameObject.layer) == -1)
            {
                Flip();              
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        
        if (!inAir && !gusted && !isFrozen)
        {
            if (layersToIgnore.IndexOf(col.gameObject.layer) == -1)
            {
                if (!isStuck)
                {
                    isStuck = true;
                }
                else
                {
                    print("trigger stay" + LayerMask.LayerToName(col.gameObject.layer));
                    Flip();
                    isStuck = false;
                }              
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        isStuck = false;
    }

    private void Flip()
    {
        print("flip");
        transform.localScale = new Vector3(-transform.localScale.x, 1f);
        horizontal *= -1;
        transform.Translate(speed * horizontal * 2 * Time.deltaTime, 0f, 0f);
    }

    public void Freeze()
    {
        if (!burning)
        {
            isFrozen = true;
            frozenElapsedTime = 0;
            GetComponent<CapsuleCollider2D>().sharedMaterial = frozenMaterial;
        }
    }

    public void Burn()
    {
        if (!isFrozen && !burning)
        {
            burning = true;
            animator.SetTrigger("Burn");
            //ResetEnemy();
        }
        else
        {
            isThawing = true;
        }
        frozenElapsedTime = 0;
    }

    public void Clone()
    {
        /*    if (isCloneable)
            {
                isCloned = true;
            }
        */
    }

    public void Gust(Vector2 velocity)
    {
        if (velocity.x > 0)
        {
            if (isFrozen)
            {
                rigidBody.AddForce(new Vector2(frozenGustSpeed, 0), ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(new Vector2(1000, 0), ForceMode2D.Impulse);
            }
        }
        else if (velocity.x < 0)
        {
            if (isFrozen)
            {
                rigidBody.AddForce(new Vector2(-frozenGustSpeed, 0), ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(new Vector2(-1000, 0), ForceMode2D.Impulse);
            }
        }
    }

    public void ResetEnemy()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        transform.position = startingPosition;
        isFrozen = false;
        isCloned = false;
        isCloned = false;
        burning = false;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    public bool IsBurning()
    {
        return burning;
    }

}
