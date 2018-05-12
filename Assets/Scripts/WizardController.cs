﻿using UnityEngine;
using System.Collections;

public class WizardController : MonoBehaviour, IBurnable, IFreezable, ICloneable, IGustable {

    public float moveSpeed;
    public float jumpSpeed;
    public LayerMask jumpableLayerMask;
    public float groundCheckRadius;
    public float projectileSpeed;
    public Transform groundCheck;
    public Spell[] availableSpells;
    public bool onLadder;
    public bool climbInitiated;
    public float climbSpeed;
    public float climbJumpSpeed;
    public float gravityStore;
    public bool isCloneable;
    public float frozenDuration = 5.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private bool canJump;
    private int activeSpellPosition;
    private Spell activeSpell;
    private Transform horizontalSpellTransform;
    private Transform upSpellTransform;
    private Transform downSpellTransform;
    private Transform activeSpellTransform;
    private LadderController ladder;
    private bool isFrozen;
    private bool isCloned;
    private float frozenElapsedTime;


    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        horizontalSpellTransform = transform.Find("HorizontalSpellFirePosition");
        upSpellTransform = transform.Find("UpSpellFirePosition");
        downSpellTransform = transform.Find("DownSpellFirePosition");
        activeSpellPosition = 0;
        activeSpell = availableSpells.Length > 0 ? availableSpells[activeSpellPosition] : null;
        gravityStore = rigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if grounded
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, jumpableLayerMask);


        // Handle movement inputs
        if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y);
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            rigidBody.velocity = new Vector3(0f, rigidBody.velocity.y);
        }


        // Handle jumping input
        if (Input.GetButtonDown("Jump") && canJump && !climbInitiated)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed);
        }


        // Handle change spell input
        if (Input.GetButtonDown("Fire3"))
        {
            activeSpellPosition = (activeSpellPosition + 1) % availableSpells.Length;
            activeSpell = availableSpells[activeSpellPosition];
        }


        // Handle aiming input
        UpdateActiveTransform();


        // Handle shooting spell input
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return))
        {
            animator.SetTrigger(string.Format("Shoot{0}Spell", activeSpell.spellName));
        }

        animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));

        //Handle onLadder climbing
        if (onLadder)
        {
            if (!climbInitiated)
            {
                //dont want to cancel gravity until climbing is initiated by hitting up or down
                if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
                {
                    climbInitiated = true;
                }
            }

            if (climbInitiated)
            {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, climbSpeed * Input.GetAxisRaw("Vertical"));
            }
            
            //Jumping cancels climbing so gravity is restored 
            if (Input.GetButtonDown("Jump") && climbInitiated)
            {
                climbInitiated = false;
                rigidBody.gravityScale = gravityStore;
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, climbJumpSpeed);
            }

            animator.SetFloat("ClimbingSpeed", Mathf.Abs(rigidBody.velocity.y));

        }
        else
        {
            climbInitiated = false;
            rigidBody.gravityScale = gravityStore;
        }
        animator.SetBool("IsClimbing", climbInitiated);
    }

    private void UpdateActiveTransform()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            activeSpellTransform = upSpellTransform;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            activeSpellTransform = downSpellTransform;
        }
        else
        {
            activeSpellTransform = horizontalSpellTransform;
        }
    }

    private void ShootSpell()
    {
        Rigidbody2D spell = null;
        spell = Instantiate(activeSpell.spellRigidBody, activeSpellTransform.position, activeSpellTransform.rotation) as Rigidbody2D;
        spell.GetComponent<SpellController>().Spell = activeSpell;
        spell.GetComponent<Rigidbody2D>().velocity = activeSpellTransform.right * projectileSpeed;
    }

    public void Freeze()
    {
        isFrozen = true;
        frozenElapsedTime = 0;
    }

    public void Clone()
    {
        if (isCloneable)
        {
            isCloned = true;
        }
    }

    public void Burn()
    {
        if (!isFrozen)
        {
            Destroy(gameObject);
        }
        isFrozen = false;
        frozenElapsedTime = 0;

    }

    public void Gust()
    {
        //todo
    }

}