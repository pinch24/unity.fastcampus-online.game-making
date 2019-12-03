using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class GoblinControl : MonoBehaviour
{
    public enum GoblinState { None, Idle, Patrol, Wait, MoveToTarget, Attack, Damage, Die }

    [Header("Default Properties")]
    public GoblinState goblinState = GoblinState.None;

    public float MoveSpeed = 1.0f;
    public GameObject TargetPlayer = null;
    public Transform TargetTransform = null;
    public Vector3 TargetPosition = Vector3.zero;

    private Animation myAnimation = null;
    private Transform myTransform = null;

    [Header("Animation Clips")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AttackAnimClip = null;
    public AnimationClip DamageAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("Battle Properties")]
    public int HP = 100;
    public float AttackRange = 1.5f;
    public GameObject DamageEffect = null;
    public GameObject DieEffect = null;
    private Tweener effectTweener = null;
    private SkinnedMeshRenderer skinMeshRenderer = null;
    
    void Start()
    {
        // State
        goblinState = GoblinState.Idle;
        
        // Caching
        myAnimation = GetComponent<Animation>();
        myTransform = GetComponent<Transform>();
        
        // Animation Clips
        myAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[AttackAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimClip.name].layer = 10;
        myAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DieAnimClip.name].layer = 10;
        
        // Animation Event
        AddAnimationEvent(AttackAnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(DamageAnimClip, "OnDamageAnimFinished");
        AddAnimationEvent(DieAnimClip, "OnDieAnimFinished");
        
        // Set Tweener
        skinMeshRenderer = myTransform.Find("body").GetComponent<SkinnedMeshRenderer>();
    }
    
    void Update()
    {
        CheckState();
        
        AnimationControl();
    }
    
    void CheckState()
    {
        switch (goblinState)
        {
            case GoblinState.Idle:
                IdleUpdate();
                break;
                
            case GoblinState.MoveToTarget:
            case GoblinState.Patrol:
                MoveUpdate();
                break;
                
            case GoblinState.Attack:
                AttackUpdate();
                break;
        }
    }
    
    void IdleUpdate()
    {
        if (TargetPlayer == null)
        {
            TargetPosition = new Vector3(
                myTransform.position.x + Random.Range(-10.0f, 10.0f),
                myTransform.position.y + 1000.0f,
                myTransform.position.z + Random.Range(-10.0f, 10.0f));
                
            Ray ray = new Ray(TargetPosition, Vector3.down);
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            {
                TargetPosition.y = raycastHit.point.y;
            }
            
            goblinState = GoblinState.Patrol;
        }
        else
        {
            goblinState = GoblinState.MoveToTarget;
        }
    }
    
    void MoveUpdate()
    {
        Vector3 diff = Vector3.zero;
        Vector3 lookAtPosition = Vector3.zero;
        
        switch (goblinState)
        {
            case GoblinState.Patrol:
            
                if (TargetPosition != Vector3.zero)
                {
                    diff = TargetPosition - myTransform.position;
                    
                    if (diff.magnitude < AttackRange)
                    {
                        StartCoroutine(WaitUpdate());
                        return;
                    }
                    
                    lookAtPosition = new Vector3(
                        TargetPosition.x,
                        myTransform.position.y,
                        TargetPosition.z);
                }
                
                break;
                
            case GoblinState.MoveToTarget:
                
                if (TargetPlayer != null)
                {
                    diff = TargetPlayer.transform.position - myTransform.position;
                    
                    if (diff.magnitude < AttackRange)
                    {
                        goblinState = GoblinState.Attack;
                        return;
                    }
                    
                    lookAtPosition = new Vector3(
                        TargetPlayer.transform.position.x,
                        myTransform.position.y,
                        TargetPlayer.transform.position.z);
                }
            
                break;
        }
        
        Vector3 direction = diff.normalized;
        direction = new Vector3(direction.x, 0.0f, direction.z);
        Vector3 moveAmount = direction * MoveSpeed * Time.deltaTime;
        myTransform.Translate(moveAmount, Space.World);
        
        myTransform.LookAt(lookAtPosition);
    }
    
    IEnumerator WaitUpdate()
    {
        goblinState = GoblinState.Wait;
        
        float waitTime = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(waitTime);
        
        goblinState = GoblinState.Idle;
    }
    
    void AttackUpdate()
    {
        float distance = Vector3.Distance(TargetTransform.position, myTransform.position);
        
        if (distance > AttackRange + 0.5f)
        {
            goblinState = GoblinState.MoveToTarget;
        }
    }
    
    void AnimationControl()
    {
        switch (goblinState)
        {
            case GoblinState.Wait:
            case GoblinState.Idle:
                myAnimation.CrossFade(IdleAnimClip.name);
                break;
            case GoblinState.Patrol:
            case GoblinState.MoveToTarget:
                myAnimation.CrossFade(MoveAnimClip.name);
                break;
            case GoblinState.Attack:
                myAnimation.CrossFade(AttackAnimClip.name);
                break;
            case GoblinState.Die:
                myAnimation.CrossFade(DieAnimClip.name);
                break;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") == true)
        {
            HP -= 10;
            
            if (HP > 0)
            {
                Instantiate(DamageEffect, other.transform.position, Quaternion
                .identity);
                
                myAnimation.CrossFade(DamageAnimClip.name);
                
                DamageTweenEffect();
            }
            else
            {
                goblinState = GoblinState.Die;
            }
        }
    }
    
    void DamageTweenEffect()
    {
        if (effectTweener != null && effectTweener.isComplete == false)
        {
            return;
        }
        else
        {
            Color colorTo = Color.red;
            effectTweener = HOTween.To(skinMeshRenderer.material, 0.2f, new TweenParms().Prop("color", colorTo).Loops(1, LoopType.Yoyo).OnStepComplete(OnDamageTweenFinished));
        }
    }
    
    void OnDamageTweenFinished()
    {
        skinMeshRenderer.material.color = Color.white;
    }
    
    void OnSetTarget(GameObject target)
    {
        TargetPlayer = target;
        TargetTransform = TargetPlayer.transform;
        
        goblinState = GoblinState.MoveToTarget;
    }
    
    /// <summary>
    /// Animation Methods
    /// </summary>
    void OnAttackAnimFinished()
    {
        //Debug.Log("Attack Animation finished.");
    }
    
    void OnDamageAnimFinished()
    {
        //Debug.Log("Damage Animation finished.");
    }
    
    void OnDieAnimFinished()
    {
        //Debug.Log("Die Animation finished.");
        
        Instantiate(DieEffect, myTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    void AddAnimationEvent(AnimationClip clip, string funcName)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = funcName;
        newEvent.time = clip.length - 0.1f;
        clip.AddEvent(newEvent);
    }
}
