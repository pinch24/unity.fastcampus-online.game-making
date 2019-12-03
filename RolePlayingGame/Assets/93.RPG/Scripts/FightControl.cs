using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightControl : MonoBehaviour
{
    [Header("Movement Properties")]
    public float WalkSpeed = 2.0f;
    public float RunSpeed = 4.0f;
    public float TurningSpeed = 100.0f;
    public float RotateSpeed = 2.0f;

    [Range(0.01f, 5.0f)]
    public float VelocitySpeed = 0.4f;

    private Vector3 CurrentVelocity = Vector3.zero;
    private Vector3 CurrentDirection = Vector3.zero;

    private CharacterController myCharacterController = null;
    private CollisionFlags myCollisionFlags = CollisionFlags.None;
    private float gravity = 9.8f;
    private float verticalSpeed = 0.0f;
    private bool CannotMove = false;

    [Header("Animation Properties")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip WalkAnimClip = null;
    public AnimationClip RunAnimClip = null;
    public AnimationClip Attack1AnimClip = null;
    public AnimationClip Attack2AnimClip = null;
    public AnimationClip Attack3AnimClip = null;
    public AnimationClip Attack4AnimClip = null;
    public AnimationClip SkillAnimClip = null;
    
    private Animation myAnimation = null;

    public enum FighterState { None, Idle, Walk, Run, Attack, Skill }
    [Header("Character Properties")]
    public FighterState myState = FighterState.None;

    public enum FighterAttackState { Attack1, Attack2, Attack3, Attack4 }
    public FighterAttackState myAttackState = FighterAttackState.Attack1;
    public bool NextAttack = false;
    
    [Header("Battle Properties")]
    public TrailRenderer AttackTrailRenderer = null;
    public CapsuleCollider AttackCapsuleCollider = null;
    public GameObject SkillEffect = null;
    
    void Start()
    {
        myCharacterController = GetComponent<CharacterController>();

        myAnimation = GetComponent<Animation>();
        myAnimation.playAutomatically = false;
        myAnimation.Stop();

        myState = FighterState.Idle;
        myAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[WalkAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[RunAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[Attack1AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack2AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack3AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack4AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[SkillAnimClip.name].wrapMode = WrapMode.Once;
        
        AddAnimationEvent(Attack1AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack2AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack3AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack4AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(SkillAnimClip, "OnSkillAnimFinished");
    }

    void Update()
    {
        // Character Move
        Move();
        BodyDirectionChange();

        // Character Animation
        AnimationControl();
        CheckState();

        // User Control
        InputControl();

        // Physics
        ApplyGravity();
        
        // Component Control
        AttackComponentControl();
    }

    /// <summary>
    /// Movement Functions
    /// </summary>
    void Move()
    {
        if (CannotMove == true)
        {
            return;
        }

        // Move Direction
        Transform CameraTransform = Camera.main.transform;
        Vector3 forward = CameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 targetDirection = horizontal * right + vertical * forward;

        CurrentDirection = Vector3.RotateTowards(CurrentDirection, targetDirection, TurningSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        CurrentDirection = CurrentDirection.normalized;

        // Move Distance
        float speed = WalkSpeed;

        if (myState == FighterState.Run)
        {
            speed = RunSpeed;
        }

        Vector3 gravityVector = new Vector3(0.0f, verticalSpeed, 0.0f);
        Vector3 moveAmount = (CurrentDirection * speed * Time.deltaTime) + gravityVector;
        myCollisionFlags = myCharacterController.Move(moveAmount);
    }

    /// <summary>
    /// Get Current Character Move Speed
    /// </summary>
    /// <returns></returns>
    float GetVelocitySpeed()
    {
        if (myCharacterController.velocity == Vector3.zero)
        {
            CurrentVelocity = Vector3.zero;
        }
        else
        {
            Vector3 goalVelocity = myCharacterController.velocity;
            goalVelocity.y = 0.0f;
            CurrentVelocity = Vector3.Lerp(CurrentVelocity, goalVelocity, VelocitySpeed * Time.fixedDeltaTime);
        }

        return CurrentVelocity.magnitude;
    }

    /// <summary>
    /// Rotate Body to Forward Direction
    /// </summary>
    void BodyDirectionChange()
    {
        if (GetVelocitySpeed() > 0.0f)
        {
            Vector3 newForward = myCharacterController.velocity;
            newForward.y = 0.0f;
            transform.forward = Vector3.Lerp(transform.forward, newForward, RotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Animation Methods
    /// </summary>
    /// <param name="clip"></param>
    void AnimationPlay(AnimationClip clip)
    {
        myAnimation.clip = clip;
        myAnimation.CrossFade(clip.name);
    }

    void AnimationControl()
    {
        switch (myState)
        {
            case FighterState.Idle:
                AnimationPlay(IdleAnimClip);
                break;

            case FighterState.Walk:
                AnimationPlay(WalkAnimClip);
                break;

            case FighterState.Run:
                AnimationPlay(RunAnimClip);
                break;

            case FighterState.Attack:
                AttackAnimationControl();
                break;
                
            case FighterState.Skill:
                AnimationPlay(SkillAnimClip);
                break;
        }
    }

    void CheckState()
    {
        float currentSpeed = GetVelocitySpeed();

        switch (myState)
        {
            case FighterState.Idle:

                if (currentSpeed > 0.0f)
                {
                    myState = FighterState.Walk;
                }

                break;

            case FighterState.Walk:

                if (currentSpeed > 0.5f)
                {
                    myState = FighterState.Run;
                }
                else if (currentSpeed < 0.01f)
                {
                    myState = FighterState.Idle;
                }

                break;

            case FighterState.Run:

                if (currentSpeed < 0.01f)
                {
                    myState = FighterState.Idle;
                }
                else if (currentSpeed < 0.5f)
                {
                    myState = FighterState.Walk;
                }

                break;

            case FighterState.Attack:
                CannotMove = true;
                break;

            case FighterState.Skill:
                CannotMove = true;
                break;
        }
    }

    void InputControl()
    {
        if (Input.GetButton("Fire1") == true)
        {
            if (myState != FighterState.Attack)
            {
                myState = FighterState.Attack;
                myAttackState = FighterAttackState.Attack1;
            }
            else
            {
                switch (myAttackState)
                {
                    case FighterAttackState.Attack1:

                        if (myAnimation[Attack1AnimClip.name].normalizedTime > 0.1)
                        {
                            NextAttack = true;
                        }

                        break;

                    case FighterAttackState.Attack2:

                        if (myAnimation[Attack2AnimClip.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }

                        break;

                    case FighterAttackState.Attack3:

                        if (myAnimation[Attack3AnimClip.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }

                        break;

                    case FighterAttackState.Attack4:

                        if (myAnimation[Attack4AnimClip.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }

                        break;
                }
            }
        }
        
        if (Input.GetButton("Fire2") == true)
        {
            if (myState == FighterState.Attack)
            {
                myAttackState = FighterAttackState.Attack1;
                NextAttack = false;
            }
            
            myState = FighterState.Skill;
        }
    }

    void OnAttackAnimFinished()
    {
        if (NextAttack == true)
        {
            NextAttack = false;

            switch (myAttackState)
            {
                case FighterAttackState.Attack1:
                    myAttackState = FighterAttackState.Attack2;
                    break;

                case FighterAttackState.Attack2:
                    myAttackState = FighterAttackState.Attack3;
                    break;

                case FighterAttackState.Attack3:
                    myAttackState = FighterAttackState.Attack4;
                    break;

                case FighterAttackState.Attack4:
                    myAttackState = FighterAttackState.Attack1;
                    break;
            }
        }
        else
        {
            myState = FighterState.Idle;
            myAttackState = FighterAttackState.Attack1;

            CannotMove = false;
        }
    }
    
    void OnSkillAnimFinished()
    {
        Vector3 position = transform.position;
        position += transform.forward * 2.0f;
        Instantiate(SkillEffect, position, Quaternion.identity);
        myState = FighterState.Idle;
    }

    void AddAnimationEvent(AnimationClip clip, string FuncName)
    {
        AnimationEvent newEvnet = new AnimationEvent();
        newEvnet.functionName = FuncName;
        newEvnet.time = clip.length - 0.1f;
        clip.AddEvent(newEvnet);
    }

    void AttackAnimationControl()
    {
        switch (myAttackState)
        {
            case FighterAttackState.Attack1:
                AnimationPlay(Attack1AnimClip);
                break;

            case FighterAttackState.Attack2:
                AnimationPlay(Attack2AnimClip);
                break;

            case FighterAttackState.Attack3:
                AnimationPlay(Attack3AnimClip);
                break;

            case FighterAttackState.Attack4:
                AnimationPlay(Attack4AnimClip);
                break;
        }
    }

    void ApplyGravity()
    {
        if ((myCollisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            verticalSpeed = 0.0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }
    
    void AttackComponentControl()
    {
        switch (myState)
        {
            case FighterState.Attack:
            case FighterState.Skill:
                AttackTrailRenderer.enabled = true;
                AttackCapsuleCollider.enabled = true;
                break;
                
            default:
                AttackTrailRenderer.enabled = false;
                AttackCapsuleCollider.enabled = false;
                break;
        }
    }

    /// <summary>
    ///  GUI Methods
    /// </summary>
    void OnGUI()
    {
        // Collision
        GUILayout.Label("Collision: " + myCollisionFlags.ToString());

        // Velocity
        GUILayout.Label("Speed: " + GetVelocitySpeed().ToString());

        if (myCharacterController != null && myCharacterController.velocity != Vector3.zero)
        {
            GUILayout.Label("Velocity Vector: " + myCharacterController.velocity.ToString());
            GUILayout.Label("Velocity Magnitude: " + myCharacterController.velocity.magnitude.ToString());
        }
    }
}
