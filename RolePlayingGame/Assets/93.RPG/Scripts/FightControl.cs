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
    public float VelocitySpeed = 0.1f;
    
    private Vector3 CurrentVelocity = Vector3.zero;
    private Vector3 MoveDirection = Vector3.zero;

    private CharacterController myCharacterController = null;
    private CollisionFlags myCollisionFlags = CollisionFlags.None;

    void Start()
    {
        myCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Movement Functions
    /// </summary>
    void Move()
    {
        Transform CameraTransform = Camera.main.transform;
        Vector3 forward = CameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 targetDirection = horizontal * right + vertical * forward;
    }
}
