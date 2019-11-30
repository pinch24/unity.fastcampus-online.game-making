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
    private Vector3 CurrentDirection = Vector3.zero;

    private CharacterController myCharacterController = null;
    private CollisionFlags myCollisionFlags = CollisionFlags.None;

    void Start()
    {
        myCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();

        BodyDirectionChange();
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

        CurrentDirection = Vector3.RotateTowards(CurrentDirection, targetDirection, TurningSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        CurrentDirection = CurrentDirection.normalized;

        Vector3 moveAmount = (CurrentDirection * WalkSpeed * Time.deltaTime);
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
    private void OnGUI()
    {
        GUILayout.Label("Speed: " + GetVelocitySpeed().ToString());

        if (myCharacterController != null && myCharacterController.velocity != Vector3.zero)
        {
            GUILayout.Label("Velocity Vector: " + myCharacterController.velocity.ToString());
            GUILayout.Label("Velocity Magnitude: " + myCharacterController.velocity.magnitude.ToString());
        }
    }
}
